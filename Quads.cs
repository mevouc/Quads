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

  static public Quadtree<Square>[] sort(Quadtree<Square>[] qts, int[] errs)
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

  static void Save()
  {
    Bitmap newImg = new Bitmap(img);
    foreach (Square sqr in squares)
    {
      for (int i = sqr.X - sqr.W / 2; i < sqr.X + sqr.W / 2; i++)
      {
        for (int j = sqr.Y - sqr.H / 2; j < sqr.Y + sqr.H / 2; j++)
          newImg.SetPixel(i, j, sqr.Color);
      }
    }
    newImg.Save("img/out.png");
  }

  static private bool Run()
  {
    Quadtree<Square> qt = new Quadtree<Square>(new Square(img));
    for (; N > 0; N--)
    {
      Err(qt);
      qt.max().subdivide();
    }
    Save();
    return true;
  }

  static public int Main(string[] args)
  {
    if (args.Length != 2)
    {
      Console.Error.WriteLine("This program takes 2 arguments.");
      return 1;
    }
    img = new Bitmap(args[0]);
    if (!Int32.TryParse(args[1], out N))
    {
      Console.Error.WriteLine("The last argument should be an integer.");
      return 1;
    }
    return Run() ? 0 : 1;
  }
}
