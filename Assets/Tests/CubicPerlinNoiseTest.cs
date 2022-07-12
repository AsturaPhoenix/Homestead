using NUnit.Framework;
using Assets;
using Assets.Terrain;
using UnityEngine;

namespace Tests {
  public class CubicPerlinNoiseTest {
    private const float epsilon = 1e-5f;

    [Test]
    public void NodeGradients() {
      const int n = 4;
      var noise = new CubicPerlinNoise(n);
      foreach (var i in Math.IntCube(n)) {
        Assert.AreEqual(noise.NodeGradient(i), noise.SampleGradient(i));
      }
    }

    [Test]
    public void FirstOrderGradients() {
      const int n = 4;
      const int resolution = 4;
      const float dx = .01f;
      var noise = new CubicPerlinNoise(n);
      foreach (var i in Math.IntCube(n * resolution)) {
        var v = (Vector3)i / resolution;
        var g = noise.SampleGradient(v);
        var ds = noise.Sample(v + dx * g) - noise.Sample(v);
        Assert.LessOrEqual(ds, 2 * dx * g.magnitude);
        Assert.GreaterOrEqual(ds, 0);
      }
    }
  }
}
