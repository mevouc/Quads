using System;
using System.Drawing;
using System.Collections.Generic;

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

  static private Quadtree<Square>[] sort(Quadtree<Square>[] qts, int[] errs)
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

  static private int Err(Quadtree<Square> qt)
  {
    if (!qt.hasChilds())
      return qt.Data.Error;
    int err = 0;
    int[] errs = new int[qt.Childs.Length];
    for (var i = 0; i < qt.Childs.Length; i++)
    {
      errs[i] = Err(qt.Childs[i]);
      err += errs[i];
    }
    qt.Childs = sort(qt.Childs, errs);
    return err;
  }

  static void Save(string path)
  {
    Bitmap newImg = new Bitmap(img.Width, img.Height);
    foreach (Square sqr in squares)
    {
      for (int i = sqr.X; i < sqr.X + sqr.W; i++)
      {
        for (int j = sqr.Y; j < sqr.Y + sqr.H; j++)
          newImg.SetPixel(i, j, sqr.Color);
      }
    }
    newImg.Save("img/" + path + ".png");
  }

  static private bool Run(string path)
  {
    Quadtree<Square> qt = new Quadtree<Square>(new Square(img));
    if (N == -1)
    {
      while (Err(qt) > 3000000)
        qt.max().subdivide();
    }
    else
    {
      for (; N > 0; N--)
      {
        Err(qt);
        qt.max().subdivide();
      }
    }
    Save(path);
    return true;
  }

  static public int Main(string[] args)
  {
    if (args.Length < 1)
    {
      Console.Error.WriteLine("This program takes 2 arguments.");
      return 1;
    }
    img = new Bitmap(args[0]);
    if (args.Length == 1)
      N = -1;
    else if (!Int32.TryParse(args[1], out N))
    {
      Console.Error.WriteLine("The last argument should be an integer.");
      return 1;
    }
    if (args.Length == 3)
      return Run(args[2]) ? 0 : 1;
    return Run("out") ? 0 : 1;
  }
}
