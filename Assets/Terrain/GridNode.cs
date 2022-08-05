public class GridNode
{
  private readonly ITerrainTreeNode[,] children;

  public int Resolution { get; private set; }
  public float Deformation { get; set; }

  public ITerrainTreeNode this[int x, int y] {
    get {
      return children[x, y];
    }
  }

  public GridNode(int resolution) {
    children = new ITerrainTreeNode[resolution, resolution];
    Resolution = resolution;
  }
}
