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

  static private double FractOfSquares(double a, double b, double c)
  {
    return ((a - b) * (a - b)) / (c * c);
  }

  static private bool InEllipse(double x, double y, double xc, double yc,
      double rx, double ry)
  {
    return (FractOfSquares(x, xc, rx) + FractOfSquares(y, yc, ry)) <= 1;
  }

  static private void Render(Square sqr, Bitmap img, bool circle)
  {
    if (circle)
    {
      for (int i = sqr.X; i < sqr.X + sqr.W; i++)
      {
        for (int j = sqr.Y; j < sqr.Y + sqr.H; j++)
        {
          double xc = (double)sqr.X + (double)sqr.W / 2.0;
          double yc = (double)sqr.Y + (double)sqr.H / 2.0;
          if (InEllipse(i, j, xc, yc, (double)sqr.W / 2.0, (double)sqr.H / 2.0))
            img.SetPixel(i, j, sqr.Color);
        }
      }
    }
    else
    {
      for (int i = sqr.X; i < sqr.X + sqr.W; i++)
      {
        for (int j = sqr.Y; j < sqr.Y + sqr.H; j++)
          img.SetPixel(i, j, sqr.Color);
      }
    }
  }

  static private bool Save(string path, bool circle)
  {
    try
    {
      Bitmap newImg = new Bitmap(img.Width, img.Height);
      foreach (Square sqr in squares)
      {
        Render(sqr, newImg, circle);
      }
      newImg.Save("img/" + path + ".png");
    }
    catch
    {
      return false;
    }
    return true;
  }

  static private bool Run(string path, bool circle)
  {
    Quadtree<Square> qt = new Quadtree<Square>(new Square(img));
    if (N == -1)
    {
      while (ErrorSort(qt) > 3000000)
        qt.Max().Subdivide();
    }
    else
    {
      for (; N > 0; N--, ErrorSort(qt))
      {
        qt.Max().Subdivide();
      }
    }
    return Save(path, circle);
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
    bool circle = true;
    if (args.Length == 3)
      return Run(args[2], circle) ? 0 : 1;
    return Run("out", circle) ? 0 : 1;
  }
}
