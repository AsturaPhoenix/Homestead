using System.Collections.Generic;
using UnityEngine;

namespace Assets.Terrain
{
  public class CubicPerlinNoise
  {
    public readonly Vector3[,,] gradients;

    private static IEnumerable<(int x, int y, int z)> IntCube(int resolution) {
      for (int z = 0; z < resolution; ++z) {
        for (int y = 0; y < resolution; ++y) {
          for (int x = 0; x < resolution; ++x) {
            yield return (x, y, z);
          }
        }
      }
    }

    public CubicPerlinNoise(int resolution) {
      gradients = new Vector3[resolution, resolution, resolution];
      foreach (var (x, y, z) in IntCube(resolution)) { 
        gradients[x, y, z] = Random.insideUnitSphere;
      }
    }

    public float Sample(Vector3 v) {
      Vector3Int i = Vector3Int.FloorToInt(v);
      Vector3 rem = v - i;
      var resolution = gradients.GetLength(0);
      i -= resolution * Vector3Int.FloorToInt(v / resolution);

      float sx = Mathf.SmoothStep(1, 0, rem.x),
            sy = Mathf.SmoothStep(1, 0, rem.y),
            sz = Mathf.SmoothStep(1, 0, rem.z);

      float s = 0;
      foreach (var (dx, dy, dz) in IntCube(2)) {
        s += Vector3.Dot(gradients[(i.x + dx) % resolution,
                                   (i.y + dy) % resolution,
                                   (i.z + dz) % resolution], rem - new Vector3Int(dx, dy, dz))
          * (dx == 0? sx : 1 - sx)
          * (dy == 0? sy : 1 - sy)
          * (dz == 0? sz : 1 - sz);
      }

      Debug.Assert(-1 <= s && s <= 1, string.Format("{0} is outside expected range [-1, 1]", s));

      return s;
    }
  }
}