public class Quadtree<T>
  where T : ISplitable<T>
{
  T data;
  Quadtree<T>[] childs = new Quadtree<T>[4];

  public Quadtree(T data)
  {
    this.data = data;
  }

  public bool subdivide()
  {
    T[] datas = data.split();
    if (datas.Length != 4)
      return false;
    for (int i = 0; i < 4; i++)
      childs[i] = new Quadtree<T>(datas[i]);
    return true;
  }
}
