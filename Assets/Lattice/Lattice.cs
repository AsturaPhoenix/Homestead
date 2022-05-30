using System.Collections.Generic;

namespace Assets.Lattice
{
  public class Lattice<T> {
    public Lattice(ILatticeGeometry<T> geometry) {
      Vertices = new List<T>(geometry.Vertices).AsReadOnly();
      Edges = new List<(int, int)>(geometry.Edges).AsReadOnly();
    }

    public IReadOnlyList<T> Vertices { get; private set; }
    public IReadOnlyList<(int a, int b)> Edges { get; private set; }
  }
}
