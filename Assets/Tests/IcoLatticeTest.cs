using NUnit.Framework;
using System;
using System.Collections.Generic;
using Assets.Lattice;
using UnityEngine;

namespace Tests {
  public class IcoLatticeTest {
    private const float epsilon = 1e-5f;

    public class PolarCoordinateTest
    {
      [Test]
      public void NormalizeNorthPole() {
        var c = new IcoLattice.PolarCoordinate(0, 0, 1, 1);
        c.Normalize(3);
        Assert.AreEqual(new IcoLattice.PolarCoordinate(), c);
      }

      [Test]
      public void NormalizeNorthPoleAlias() {
        var c = new IcoLattice.PolarCoordinate(5, 3, 1, 1);
        c.Normalize(3);
        Assert.AreEqual(new IcoLattice.PolarCoordinate(), c);
      }

      [Test]
      public void NormalizeSouthPole() {
        var c = new IcoLattice.PolarCoordinate(3, 0, 1, 1);
        c.Normalize(3);
        Assert.AreEqual(new IcoLattice.PolarCoordinate(3, 0, 0, 0), c);
      }

      [Test]
      public void NormalizeSouthPoleAlias() {
        var c = new IcoLattice.PolarCoordinate(2, 3, 1, 1);
        c.Normalize(3);
        Assert.AreEqual(new IcoLattice.PolarCoordinate(3, 0, 0, 0), c);
      }

      [Test]
      public void NormalizeTropicNNE() {
        var c = new IcoLattice.PolarCoordinate(1, -1, 0, -1);
        c.Normalize(3);
        Assert.AreEqual(new IcoLattice.PolarCoordinate(0, 2, 4, 1), c);
      }

      [Test]
      public void NormalizeTropicNNW() {
        var c = new IcoLattice.PolarCoordinate(1, -1, 4, 2);
        c.Normalize(3);
        Assert.AreEqual(new IcoLattice.PolarCoordinate(0, 2, 0, 0), c);
      }

      [Test]
      public void NormalizeTropicNSE() {
        var c = new IcoLattice.PolarCoordinate(1, 1, 0, -1);
        c.Normalize(3);
        Assert.AreEqual(new IcoLattice.PolarCoordinate(1, 1, 4, 2), c);
      }

      [Test]
      public void NormalizeTropicNSW() {
        var c = new IcoLattice.PolarCoordinate(1, 1, 4, 3);
        c.Normalize(3);
        Assert.AreEqual(new IcoLattice.PolarCoordinate(1, 1, 0, 0), c);
      }

      [Test]
      public void NormalizeTropicSNE() {
        var c = new IcoLattice.PolarCoordinate(1, 2, 0, -1);
        c.Normalize(3);
        Assert.AreEqual(new IcoLattice.PolarCoordinate(1, 2, 4, 2), c);
      }

      [Test]
      public void NormalizeTropicSNW() {
        var c = new IcoLattice.PolarCoordinate(1, 2, 4, 3);
        c.Normalize(3);
        Assert.AreEqual(new IcoLattice.PolarCoordinate(1, 2, 0, 0), c);
      }

      [Test]
      public void NormalizeTropicSSE() {
        var c = new IcoLattice.PolarCoordinate(1, 3, 0, -1);
        c.Normalize(3);
        Assert.AreEqual(new IcoLattice.PolarCoordinate(2, 0, 4, 2), c);
      }

      [Test]
      public void NormalizeTropicSSW() {
        var c = new IcoLattice.PolarCoordinate(1, 3, 4, 3);
        c.Normalize(3);
        Assert.AreEqual(new IcoLattice.PolarCoordinate(2, 0, 0, 0), c);
      }

      [Test]
      public void Antipodes() {
        var lattice = new IcoLattice(5);
        foreach (var c in lattice.PolarCoordinates) {
          Assert.AreEqual(0, (lattice.ToCartesian(c) + lattice.ToCartesian(-c)).sqrMagnitude, epsilon);
        }
      }
    }

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

    [Test]
    public void Project() {
      var lattice = new IcoLattice(5);
      int expected = 0;
      foreach (var v in lattice.Vertices) {
        (int a, int b, int c, Vector2 rem) = lattice.Project(v);

        int i;
        if (rem.x < .5f && rem.y < .5f) {
          i = a;
          Assert.AreEqual(0, rem.x, epsilon);
          Assert.AreEqual(0, rem.y, epsilon);
        } else if (rem.x > .5f) {
          i = b;
          Assert.AreEqual(1, rem.x, epsilon);
          Assert.AreEqual(0, rem.y, epsilon);
        } else {
          i = c;
          Assert.AreEqual(0, rem.x, epsilon);
          Assert.AreEqual(1, rem.y, epsilon);
        }

        Assert.AreEqual(expected, i, "{0} => ({1}, {2}, {3}) + {4}", v, a, b, c, rem);
        ++expected;
      }
    }
  }
}
