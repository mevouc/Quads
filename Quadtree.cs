public class Quadtree<T>
  where T : ISplitable<T>
{
  private T data;
  private Quadtree<T>[] childs;

  public T Data
  {
    get { return this.data; }
    private set { this.data = value; }
  }

  public Quadtree<T>[] Childs
  {
    get { return this.childs; }
    set { this.childs = value; }
  }

  public Quadtree(T data)
  {
    this.data = data;
    this.childs = null;
  }

  public bool hasChilds()
  {
    return (this.childs != null);
  }

  public Quadtree<T> max()
  {
    if (!this.hasChilds())
      return this;
    return this.childs[3].max();
  }

  public bool subdivide()
  {
    T[] datas = data.split();
    if (datas == null || datas.Length != 4)
      return false;
    this.childs = new Quadtree<T>[4];
    for (var i = 0; i < 4; i++)
      this.childs[i] = new Quadtree<T>(datas[i]);
    return true;
  }
}
