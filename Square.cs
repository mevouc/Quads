using System.Drawing;

public class Square : ISplitable<Square>
{
  private int x;
  private int y;
  private int halfWidth;
  private Color color;
  static private Bitmap img;

  public Square(int x, int y, int halfWidth, Color color)
  {
    this.x = x;
    this.y = y;
    this.halfWidth = halfWidth;
    this.color = color;
  }

  public Square(Bitmap img)
  {
    Square.img = img;
  }

  public Square[] split()
  {
    return null;
  }
}
