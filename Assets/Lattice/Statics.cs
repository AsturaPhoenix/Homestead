using System.Collections.Generic;
using UnityEngine;

namespace Assets.Lattice
{
  public class Statics
  {
    public static float[] DistributeForces(IEnumerable<Vector3> normals, Vector3 force, float k) {
      int n = 0;
      SymmetricMatrix m = new(3);
      foreach (Vector3 normal in normals) {
        ++n;
        m += Math.SelfOuterProduct(new Vector(normal));
      }
      m *= -k;

      var dx = Math.Solve(m, new Vector(force)).ToVector3();
      float[] forces = new float[n];
      int i = 0;
      foreach (Vector3 normal in normals) {
        forces[i] = k * Vector3.Dot(dx, normal);
        ++i;
      }

      return forces;
    }
  }
}