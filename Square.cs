using System;
using System.Drawing;

public class Square : ISplitable<Square>
{
  private int x;
  private int y;
  private int width;
  private int height;
  private Color color;
  private int error;
  static private Bitmap img;

  public int Error { get; private set; }

  private Color average()
  {
    int sumA = 0;
    int sumR = 0;
    int sumG = 0;
    int sumB = 0;
    for (int i = x - width / 2; i < x + width / 2; i++)
    {
      for (int j = y - height / 2; j < y + height / 2; j++)
      {
        Color pixelColor = img.GetPixel(i, j);
        sumA += pixelColor.A;
        sumR += pixelColor.R;
        sumG += pixelColor.G;
        sumB += pixelColor.B;
      }
    }
    int nbPixels = width * height;
    byte avgA = (byte)(sumA / nbPixels);
    byte avgR = (byte)(sumR / nbPixels);
    byte avgG = (byte)(sumG / nbPixels);
    byte avgB = (byte)(sumB / nbPixels);
    return Color.FromArgb(avgA, avgR, avgG, avgB);
  }

  private int err()
  {
    int err = 0;
    for (int i = x - width / 2; i < x + width / 2; i++)
    {
      for (int j = y - height / 2; j < y + height / 2; j++)
      {
        err += Math.Abs(color.A - img.GetPixel(i, j).A);
        err += Math.Abs(color.R - img.GetPixel(i, j).R);
        err += Math.Abs(color.G - img.GetPixel(i, j).G);
        err += Math.Abs(color.B - img.GetPixel(i, j).B);
      }
    }
    return err;
  }

  public Square(int x, int y, int width, int height)
  {
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;
    this.color = this.average();
    this.Error = this.err();
  }

  public Square(Bitmap img)
  {
    Square.img = img;
    this.width = img.Width;
    this.height = img.Height;
    this.x = this.width / 2;
    this.y = this.height / 2;
    this.color = this.average();
    this.Error = this.err();
  }

  public Square[] split()
  {
    Square[] arr = new Square[4];
    int halfWidth = width / 2;
    int halfHeight = height / 2;
    arr[0] = new Square(x + halfWidth, y + halfHeight, halfWidth, halfHeight);
    arr[1] = new Square(x + halfWidth, y - halfHeight, halfWidth, halfHeight);
    arr[2] = new Square(x - halfWidth, y - halfHeight, halfWidth, halfHeight);
    arr[3] = new Square(x - halfWidth, y + halfHeight, halfWidth, halfHeight);
    return arr;
  }
}
