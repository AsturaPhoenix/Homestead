using NUnit.Framework;
using System.Collections.Generic;
using Assets.Lattice;
using UnityEngine;

namespace Tests {
  public class IcoGridTest {
    [Test]
    public void EnumerateEdges() {
      var lattice = new Lattice<Vector3>(new IcoLattice(5));
      var adjacency = new ISet<int>[lattice.Vertices.Count];
      for (int i = 0; i < adjacency.Length; ++i) {
        adjacency[i] = new HashSet<int>();
      }

      foreach (var edge in lattice.Edges) {
        Assert.IsTrue(adjacency[edge.a].Add(edge.b), "Duplicate edge between vertices {0} and {1}.", edge.a, edge.b);
        Assert.IsTrue(adjacency[edge.b].Add(edge.a), "Duplicate edge between vertices {0} and {1}.", edge.b, edge.a);
      }

      int fives = 0, sixes = 0;
      for (int i = 0; i < adjacency.Length; ++i) {
        var edges = adjacency[i];
        int edgeCount = edges.Count;
        if (edgeCount == 5) ++fives;
        else if (edgeCount == 6) ++sixes;
        else Assert.Fail("Edge count of vertex {0} should be 5 or 6; was {1}. Connected vertices: {2}", i, edgeCount, edges);
      }
      Assert.AreEqual(12, fives, "Expected exactly 12 vertices with 5 edges.");
      Assert.AreEqual(adjacency.Length - 12, sixes, "Expected all other vertices with 6 edges.");
    }

    [Test]
    public void VertexNormals() {
      const int n = 5;
      const float tolerance = .25f;

      var lattice = new Lattice<Vector3>(new IcoLattice(n));
      var delta = 2 * Mathf.PI / (5 * n);
      var dsq = delta * delta;

      foreach (var edge in lattice.Edges) {
        Assert.AreEqual(dsq, (lattice.Vertices[edge.a] - lattice.Vertices[edge.b]).sqrMagnitude, (1 - (1 - tolerance) * (1 - tolerance))  * dsq);
      }
    }
  }
}
