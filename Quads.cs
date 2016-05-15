using System;
using System.Drawing;

public class Quads
{
  static private Bitmap img;
  static private int N;

  static private bool Run()
  {
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
