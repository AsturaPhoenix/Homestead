using System.Collections.Generic;

namespace Assets.Lattice
{
  public interface ILatticeGeometry<T>
  {
    public IEnumerable<T> Vertices { get; }
    public IEnumerable<(int a, int b)> Edges { get; }
  }
}
