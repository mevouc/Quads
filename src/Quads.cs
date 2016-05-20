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

  static private Quadtree<Square>[] Sort(Quadtree<Square>[] qts, double[] errs)
  {
    for (var i = 0; i < qts.Length; i++)
      for (var j = i; j > 0 && errs[j - 1] > errs[j]; j--)
      {
        Swap<Quadtree<Square>>(ref qts[j], ref qts[j - 1]);
        Swap<double>(ref errs[j], ref errs[j - 1]);
      }
    return qts;
  }

  static private double ErrorSort(Quadtree<Square> qt)
  {
    if (!qt.HasChilds())
      return qt.Data.Error;
    double err = 0;
    double[] errs = new double[qt.Childs.Length];
    for (var i = 0; i < qt.Childs.Length; i++)
    {
      errs[i] = ErrorSort(qt.Childs[i]);
      err += errs[i];
    }
    qt.Childs = Sort(qt.Childs, errs);
    return err;
  }

  static private bool Save(string path, Shape shape, Color color)
  {
    try
    {
      Bitmap newImg = new Bitmap(img.Width, img.Height);
      foreach (Square sqr in squares)
        sqr.Render(newImg, shape, color);
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

  static private bool Run(string path, Shape shape, Color color)
  {
    Quadtree<Square> qt = new Quadtree<Square>(new Square(img));
    if (N < 0)
      while (ErrorSort(qt) > 3000000)
        qt.Max().Subdivide();
    else
      for (; N > 0; N--, ErrorSort(qt))
        qt.Max().Subdivide();
    return Save(path, shape, color);
  }

  static private bool CheckHexChar(string str)
  {
    bool ok = true;
    for (int i = 0; ok && i < str.Length; i++)
      ok = ok && ((str[i] >= '0' && str[i] <= '9')
           || (str[i] >= 'A' && str[i] <= 'F')
           || (str[i] >= 'a' && str[i] <= 'f'));
    return ok;
  }

  static private bool HexToColor(string str, out Color color)
  {
    /* #AARRGGBB
     * AARRGGBB
     * #RRGGBB
     * RRGGBB
     */
    color = Color.Empty;
    if (str == ".")
    {
      color = Color.Black;
      return true;
    }
    switch (str.Length)
    {
    case 6:
      if (!CheckHexChar(str))
        return false;
      break;
    case 7:
      if (str[0] != '#')
        return false;
      return HexToColor(str.Substring(1), out color);
    case 8:
      string aa = str.Substring(0, 2);
      if (!CheckHexChar(aa))
        return false;
      if (HexToColor(str.Substring(2, 6), out color))
        color = Color.FromArgb(Byte.Parse(aa, NumberStyles.HexNumber), color);
      return true;
    case 9:
      if (str[0] != '#')
        return false;
      return HexToColor(str.Substring(1), out color);
    default:
      return false;
    }
    byte rr = Byte.Parse(str.Substring(0, 2), NumberStyles.HexNumber);
    byte gg = Byte.Parse(str.Substring(2, 2), NumberStyles.HexNumber);
    byte bb = Byte.Parse(str.Substring(4, 2), NumberStyles.HexNumber);
    color = Color.FromArgb(rr, gg, bb);
    return true;
  }

  static private bool TryParseBitmap(string str, out Bitmap img)
  {
    try
    {
      img = new Bitmap(str);
    }
    catch
    {
      img = null;
      return false;
    }
    return true;
  }

  static private void Help()
  {
    Console.WriteLine("Help.");
  }

  /*
   * Return if need of prompting help.
   */
  static private bool ParseArg(string[] args, ref int i, ref Bitmap img,
      ref int N, ref string outPath, ref bool circle, ref bool diamond,
      ref bool edges, ref Color color)
  {
    switch (args[i])
    {
    case "-h": case "--help":
      return true;
    case "-i": case "--image":
      return !(i + 1 < args.Length && TryParseBitmap(args[++i], out img));
    case "-n": case "--number":
      return !(i + 1 < args.Length && Int32.TryParse(args[++i], out N));
    case "-o": case "--out":
      return !(i + 1 < args.Length && (outPath = args[++i]) != null);
    case "-d": case "--diamond":
      return ((circle || edges) || !(i + 1 < args.Length))
        || !(diamond = HexToColor(args[++i], out color));
    case "-c": case "--circle":
      return ((diamond || edges) || !(i + 1 < args.Length))
        || !(circle = HexToColor(args[++i], out color));
    case "-e": case "--edges":
      return ((circle || diamond) || !(i + 1 < args.Length))
        || !(edges = HexToColor(args[++i], out color));
    }
    return true;
  }

  static private int Run(bool circle, bool diamond, bool edges, string outPath,
      Color color)
  {
    Shape shape = Shape.Square;
    if (circle)
      shape = Shape.Circle;
    else if (diamond)
      shape = Shape.Rhombus;
    else if (edges)
      shape = Shape.Edges;
    if (outPath != null)
      return Run(outPath, shape, color) ? 0 : 1;
    return Run("img/out.png", shape, color) ? 0 : 1;
  }

  static public int Main(string[] args)
  {
    bool help = args.Length < 1;
    bool circle = false;
    bool diamond = false;
    bool edges = false;
    Color color = Color.Empty;
    string outPath = null;
    N = -1;
    int i = 0;
    while (!help && i < args.Length)
    {
      help = ParseArg(args, ref i, ref img, ref N, ref outPath, ref circle,
          ref diamond, ref edges, ref color);
      i++;
    }
    if (!help)
      return Run(circle, diamond, edges, outPath, color);
    Help();
    return 0;
  }
}
