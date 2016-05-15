using System;
using System.Drawing;

public class Square : ISplitable<Square>
{
  private int x;
  private int y;
  private int w;
  private int h;
  private Color color;
  private int error;
  static private Bitmap img;

  public int X
  {
    get { return this.x; }
    private set { this.x = value; }
  }

  public int Y
  {
    get { return this.y; }
    private set { this.y = value; }
  }

  public int W
  {
    get { return this.w; }
    private set { this.w = value; }
  }

  public int H
  {
    get { return this.h; }
    private set { this.h = value; }
  }

  public Color Color
  {
    get { return this.color; }
    private set { this.color = value; }
  }

  public int Error
  {
    get { return this.error; }
    private set { this.error = value; }
  }

  private Color Average()
  {
    int sumA = 0;
    int sumR = 0;
    int sumG = 0;
    int sumB = 0;
    for (int i = x - w / 2; i < x + w / 2; i++)
    {
      for (int j = y - h / 2; j < y + h / 2; j++)
      {
        Color pixelColor = img.GetPixel(i, j);
        sumA += pixelColor.A;
        sumR += pixelColor.R;
        sumG += pixelColor.G;
        sumB += pixelColor.B;
      }
    }
    int nbPixels = w * h;
    int avgA = sumA / nbPixels;
    int avgR = sumR / nbPixels;
    int avgG = sumG / nbPixels;
    int avgB = sumB / nbPixels;
    return Color.FromArgb(avgA, avgR, avgG, avgB);
  }

  private int Err()
  {
    int err = 0;
    for (int i = x - w / 2; i < img.Width && i < x + w / 2; i++)
    {
      for (int j = y - h / 2; j < img.Height && j < y + h / 2; j++)
      {
        err += Math.Abs(color.A - img.GetPixel(i, j).A);
        err += Math.Abs(color.R - img.GetPixel(i, j).R);
        err += Math.Abs(color.G - img.GetPixel(i, j).G);
        err += Math.Abs(color.B - img.GetPixel(i, j).B);
      }
    }
    return err;
  }

  public Square(int x, int y, int w, int h)
  {
    this.x = x;
    this.y = y;
    this.w = w;
    this.h = h;
    this.color = this.Average();
    this.Error = this.Err();
    Quads.AddSquare(this);
  }

  public Square(Bitmap img)
  {
    Square.img = img;
    this.w = img.Width;
    this.h = img.Height;
    this.x = this.w / 2;
    this.y = this.h / 2;
    this.color = this.Average();
    this.Error = this.Err();
    Quads.AddSquare(this);
  }

  public Square[] split()
  {
    int halfW = w / 2;
    int halfH = h / 2;
    if (halfW * halfH == 0)
      return null;
    Square[] smallSquares = new Square[4];
    smallSquares[0] = new Square(x + halfW / 2, y + halfH / 2, halfW, halfH);
    smallSquares[1] = new Square(x + halfW / 2, y - halfH / 2, halfW, halfH);
    smallSquares[2] = new Square(x - halfW / 2, y - halfH / 2, halfW, halfH);
    smallSquares[3] = new Square(x - halfW / 2, y + halfH / 2, halfW, halfH);
    Quads.RmSquare(this);
    return smallSquares;
  }
}
