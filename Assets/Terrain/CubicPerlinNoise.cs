using System.Collections.Generic;
using UnityEngine;

namespace Assets.Terrain
{
  public class CubicPerlinNoise
  {

    private readonly Vector3[,,] gradients;

    public Vector3 NodeGradient(Vector3Int i) {
      return gradients[Math.FloorMod(i.x, gradients.GetLength(0)),
                       Math.FloorMod(i.y, gradients.GetLength(1)),
                       Math.FloorMod(i.z, gradients.GetLength(2))];
    }

    public CubicPerlinNoise(int n) {
      gradients = new Vector3[n, n, n];
      foreach (var i in Math.IntCube(n)) { 
        gradients[i.x, i.y, i.z] = Random.insideUnitSphere;
      }
    }

    public float Sample(Vector3 v) {
      Vector3Int i = Vector3Int.FloorToInt(v);
      Vector3 rem = v - i;

      float sx = Mathf.SmoothStep(1, 0, rem.x),
            sy = Mathf.SmoothStep(1, 0, rem.y),
            sz = Mathf.SmoothStep(1, 0, rem.z);

      float s = 0;
      foreach (var d in Math.IntCube(2)) {
        s += Vector3.Dot(NodeGradient(i + d), rem - d)
          * (d.x == 0? sx : 1 - sx)
          * (d.y == 0? sy : 1 - sy)
          * (d.z == 0? sz : 1 - sz);
      }

      Debug.Assert(-1 <= s && s <= 1, string.Format("{0} is outside expected range [-1, 1]", s));

      return s;
    }

    public Vector3 SampleGradient(Vector3 v) {
      Vector3Int i = Vector3Int.FloorToInt(v);
      Vector3 rem = v - i;

      float sx = Mathf.SmoothStep(1, 0, rem.x),
            sy = Mathf.SmoothStep(1, 0, rem.y),
            sz = Mathf.SmoothStep(1, 0, rem.z);

      Vector3 g = new Vector3();
      foreach (var d in Math.IntCube(2)) {
        Vector3 node = NodeGradient(i + d);
        float dot = Vector3.Dot(node, rem - d);
        Vector3 s = new Vector3(d.x == 0 ? sx : 1 - sx,
                                d.y == 0 ? sy : 1 - sy,
                                d.z == 0 ? sz : 1 - sz);

        // This assumes Unity SmoothStep is 3x^2 - 2x^3
        // Each of these should also be multiplied by the non-varying s dimensions, but we can factor that out into the
        // subsequent product rule step.
        Vector3 ds = 6 * new Vector3((d.x == 0? -1 : 1) * rem.x * (1 - rem.x),
                                     (d.y == 0? -1 : 1) * rem.y * (1 - rem.y),
                                     (d.z == 0? -1 : 1) * rem.z * (1 - rem.z));
        g.x += (dot * ds.x + node.x * s.x) * s.y * s.z;
        g.y += (dot * ds.y + node.y * s.y) * s.x * s.z;
        g.z += (dot * ds.z + node.z * s.z) * s.x * s.y;
      }

      return g;
    }
  }
}