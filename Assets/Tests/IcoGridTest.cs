using NUnit.Framework;
using System.Collections.Generic;

namespace Tests {
  public class IcoGridTest {
    [Test]
    public void EnumerateEdges() {
      var grid = new IcoGrid<ISet<int>>(5);
      for (int i = 0; i < grid.vertices.Length; ++i) {
        grid.vertices[i] = new HashSet<int>();
      }

      foreach (var edge in grid.Edges) {
        Assert.IsTrue(edge.A.Add(edge.b), "Duplicate edge between vertices {0} and {1}.", edge.a, edge.b);
        Assert.IsTrue(edge.B.Add(edge.a), "Duplicate edge between vertices {0} and {1}.", edge.b, edge.a);
      }

      int fives = 0, sixes = 0;
      for (int i = 0; i < grid.vertices.Length; ++i) {
        var edges = grid.vertices[i];
        int edgeCount = edges.Count;
        if (edgeCount == 5) ++fives;
        else if (edgeCount == 6) ++sixes;
        else Assert.Fail("Edge count of vertex {0} should be 5 or 6; was {1}. Connected vertices: {2}", i, edgeCount, edges);
      }
      Assert.AreEqual(12, fives, "Expected exactly 12 vertices with 5 edges.");
      Assert.AreEqual(grid.vertices.Length - 12, sixes, "Expected all other vertices with 6 edges.");
    }
  }
}
