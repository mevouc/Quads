using System;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;

public class Quads
{
  static private Bitmap img;
  static private int N;
  static private List<Square> squares;

  static Quads()
  {
    squares = new List<Square>();
  }

  static public void AddSquare(Square square)
  {
    squares.Add(square);
  }

  static public void RmSquare(Square square)
  {
    squares.Remove(square);
  }

  static private void Swap<T>(ref T a, ref T b)
  {
    T tmp = a;
    a = b;
    b = tmp;
  }

  static private Quadtree<Square>[] Sort(Quadtree<Square>[] qts, int[] errs)
  {
    for (var i = 0; i < qts.Length; i++)
    {
      for (var j = i; j > 0 && errs[j - 1] > errs[j]; j--)
      {
        Swap<Quadtree<Square>>(ref qts[j], ref qts[j - 1]);
        Swap<int>(ref errs[j], ref errs[j - 1]);
      }
    }
    return qts;
  }

  static private int ErrorSort(Quadtree<Square> qt)
  {
    if (!qt.HasChilds())
      return qt.Data.Error;
    int err = 0;
    int[] errs = new int[qt.Childs.Length];
    for (var i = 0; i < qt.Childs.Length; i++)
    {
      errs[i] = ErrorSort(qt.Childs[i]);
      err += errs[i];
    }
    qt.Childs = Sort(qt.Childs, errs);
    return err;
  }

  static private bool Save(string path, Color circle)
  {
    try
    {
      Bitmap newImg = new Bitmap(img.Width, img.Height);
      foreach (Square sqr in squares)
        sqr.Render(newImg, circle);
      newImg.Save(path);
    }
    catch
    {
      Console.Error.Write("Can't save the generated image. Please check the");
      Console.Error.WriteLine(" writing rights of the destination folder.");
      return false;
    }
    return true;
  }

  static private bool Run(string path, Color circle)
  {
    Quadtree<Square> qt = new Quadtree<Square>(new Square(img));
    if (N < 0)
    {
      while (ErrorSort(qt) > 3000000)
        qt.Max().Subdivide();
    }
    else
    {
      for (; N > 0; N--, ErrorSort(qt))
        qt.Max().Subdivide();
    }
    return Save(path, circle);
  }

  static private bool CheckHexChar(string str)
  {
    bool ok = true;
    for (int i = 0; ok && i < str.Length; i++)
    {
      ok = ok && ((str[i] >= '0' && str[i] <= '9')
          || (str[i] >= 'A' && str[i] <= 'F')
          || (str[i] >= 'a' && str[i] <= 'f'));
    }
    return ok;
  }

  static private Color HexToColor(string str)
  {
    /* #AARRGGBB
     * AARRGGBB
     * #RRGGBB
     * RRGGBB
     */
    if (str == ".")
      return Color.Black;
    switch (str.Length)
    {
    case 6:
      if (!CheckHexChar(str))
        return Color.Empty;
      break;
    case 7:
      if (str[0] != '#')
        return Color.Empty;
      return HexToColor(str.Substring(1));
    case 8:
      string aa = str.Substring(0, 2);
      if (!CheckHexChar(aa))
        return Color.Empty;
      return Color.FromArgb(Byte.Parse(aa, NumberStyles.HexNumber),
          HexToColor(str.Substring(2, 6)));
    case 9:
      if (str[0] != '#')
        return Color.Empty;
      return HexToColor(str.Substring(1));
    default:
      return Color.Empty;
    }
    byte rr = Byte.Parse(str.Substring(0, 2), NumberStyles.HexNumber);
    byte gg = Byte.Parse(str.Substring(2, 2), NumberStyles.HexNumber);
    byte bb = Byte.Parse(str.Substring(4, 2), NumberStyles.HexNumber);
    return Color.FromArgb(rr, gg, bb);
  }

  static private void Help()
  {
    Console.WriteLine("Help.");
  }

  static public int Main(string[] args)
  {
    bool help = args.Length < 1;
    Color circle = Color.Empty;
    string outPath = null;
    N = -1;
    int i = 0;
    while (!help && i < args.Length)
    {
      switch (args[i])
      {
      case "-h":
        help = true;
        break;
      case "-i":
        if (i + 1 < args.Length)
          img = new Bitmap(args[++i]);
        else
          help = true;
        break;
      case "-n":
        help = !(i + 1 < args.Length && Int32.TryParse(args[++i], out N));
        break;
      case "-o":
        if (i + 1 < args.Length)
          outPath = args[++i];
        else
          help = true;
        break;
      case "-c":
        if (i + 1 < args.Length)
          help = (circle = HexToColor(args[++i])) == Color.Empty;
        else
          help = true;
        break;
      default:
        help = true;
        break;
      }
      i++;
    }
    if (!help)
    {
      if (outPath != null)
        return Run(outPath, circle) ? 0 : 1;
      return Run("img/out.png", circle) ? 0 : 1;
    }
    Help();
    return 0;
  }
}
