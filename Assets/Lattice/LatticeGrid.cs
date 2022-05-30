using System.Collections.Generic;
using System.Linq;

namespace Assets.Lattice
{
  public class LatticeGrid<T, U> {
    public class Node
    {
      public T Vertex;
      public U Value;
    }

    public LatticeGrid(ILatticeGeometry<T> geometry) {
      Vertices = new List<Node>(from vertex in geometry.Vertices select new Node { Vertex = vertex }).AsReadOnly();
      Edges = new List<(Node, Node)>(from edge in geometry.Edges select (Vertices[edge.a], Vertices[edge.b])).AsReadOnly();
    }

    public IReadOnlyList<Node> Vertices { get; private set; }
    public IReadOnlyList<(Node a, Node b)> Edges { get; private set; }
  }
}
