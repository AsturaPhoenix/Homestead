using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
  public static class Math
  {
    public static int FloorMod(int a, int b) {
      int mod = a % b;
      return a >= 0 ? mod : mod + Mathf.Abs(b);
    }

    public static int DivRem(int a, int b, out int rem) {
      int q = System.Math.DivRem(a, b, out rem);
      if (rem < 0) {
        --q;
        rem += Mathf.Abs(b);
      }
      return q;
    }

    public static IEnumerable<Vector3Int> IntCube(int n) {
      for (int z = 0; z < n; ++z) {
        for (int y = 0; y < n; ++y) {
          for (int x = 0; x < n; ++x) {
            yield return new Vector3Int(x, y, z);
          }
        }
      }
    }

    public static Color ColorScale(float v, params Color[] colors) {
      if (v < 0) {
        return colors[0];
      }
      if (v >= colors.Length - 1) {
        return colors[^1];
      }
      return Color.Lerp(colors[(int)v], colors[(int)v + 1], v - (int)v);
    }
  }
}