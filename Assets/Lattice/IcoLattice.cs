using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Lattice
{
  /// <summary>
  /// Represents an icosahedral spherical lattice starting from Vector3.up, descending towards Vector3.forward, circling left-handed up.
  /// </summary>
  public class IcoLattice : ILatticeGeometry<Vector3>
  {
    public struct SubdividedCoordinate
    {
      public int face, div;

      public SubdividedCoordinate(int face, int div) {
        this.face = face;
        this.div = div;
      }
      public override string ToString() {
        return String.Format("({0}, {1})", face, div);
      }


      /// <summary>
      /// Normalizes a subdivided coordinate to the range ([0, faces), [0, subdivisions)). It is worth noting that this handles negative signs differently
      /// from naive integer arithmetic.
      /// </summary>
      public void Normalize(int faces, int subdivisions) {
        face += div / subdivisions;
        div %= subdivisions;

        if (div < 0) {
          --face;
          div += subdivisions;
        }

        face %= faces;
        if (face < 0) {
          face += faces;
        }
      }
    }

    public struct PolarCoordinate
    {
      /// <summary>
      /// lat down, lon left
      /// 
      /// In the tropics (lat face 1), longitudinal faces represent triangle pairs where the left triangle points down and the right points up.
      /// 
      /// Normalized form is up/left-inclusive, where the south pole is at latitude 3.0
      /// </summary>
      public SubdividedCoordinate lat, lon;

      public PolarCoordinate(int latFace, int latDiv, int lonFace, int lonDiv) {
        lat = new SubdividedCoordinate(latFace, latDiv);
        lon = new SubdividedCoordinate(lonFace, lonDiv);
      }

      public override string ToString() {
        return String.Format("({0}, {1})", lat, lon);
      }

      public void Normalize(int subdivisions) {
        lat.Normalize(6, subdivisions);

        if (lat.div == 0 && (lat.face == 0 || lat.face == 3)) { // poles
          lon.face = lon.div = 0;
        } else {
          if (lat.face >= 3) {
            lat.face = 5 - lat.face;
            lat.div = subdivisions - lat.div;
            Debug.Assert(lat.face >= 0 && lat.face < 3);

            // To wrap latitude to the opposite side of the sphere, we need to flip things one way or another. One useful
            // interpretation of this is to flip the face alignment as well, so that for any polar coordinate, its antipode is
            // at lat.face + 3.
            switch (lat.face) {
              case 0:
                lon.face -= 2;
                break;
              case 1:
                lon.face -= 2;
                lon.div -= lat.div;
                break;
              case 2:
                lon.face += 2;
                break;
            }
          }

          lon.Normalize(5, lat.face == 0 ? lat.div : lat.face == 1 ? subdivisions : subdivisions - lat.div);
        }
      }

      public static PolarCoordinate operator +(PolarCoordinate c, (int latDiv, int lonDiv) delta) {
        c.lat.div += delta.latDiv;
        c.lon.div += delta.lonDiv;
        return c;
      }

      /// <summary>
      /// Flips this polar coordinate to its antipode. The result is not normalized; normalization requires a subdivision
      /// parameter.
      /// </summary>
      public static PolarCoordinate operator -(PolarCoordinate c) {
        c.lat.face += 3;
        return c;
      }
    }

    // There are a bunch of ways to mesh the normals; let's do normalized linear interpolation for simplicity.
    // Since we're interpolating linearly, each latitude is truly flat and increments linearly, so only the tropic Y is of consequence.
    private static readonly float tropicY = 1 / Mathf.Sqrt(5);
    // The rest of the normals will be (x, z), and we'll omit the poles since they'll be origin.
    private static readonly Vector2[] icoNormals = new Vector2[10];
    static IcoLattice() {
      float sqrt5 = 1 / tropicY;
      float tropicR = 2 * tropicY;
      var lonVec = new Vector2(Mathf.Sqrt((5 + sqrt5) / 8), (sqrt5 - 1) / 4);

      icoNormals[0] = new Vector2(0, tropicR);
      icoNormals[1] = tropicR * lonVec;
      icoNormals[2] = new Vector2(
        icoNormals[1].x * lonVec.y + icoNormals[1].y * lonVec.x,
        icoNormals[1].y * lonVec.y - icoNormals[1].x * lonVec.x);
      icoNormals[3] = new Vector2(-icoNormals[2].x, icoNormals[2].y);
      icoNormals[4] = new Vector2(-icoNormals[1].x, icoNormals[1].y);

      for (int i = 5; i < 10; ++i) {
        icoNormals[i] = -icoNormals[(i - 2) % 5];
      }
    }

    public readonly int subdivisions;

    public IcoLattice(int subdivisions) {
      if (subdivisions < 1) {
        throw new ArgumentOutOfRangeException("subdivisions", subdivisions, "subdivisions must be >= 1");
      }

      this.subdivisions = subdivisions;
    }

    public Vector3 ToCartesian(PolarCoordinate c) {
      c.Normalize(subdivisions);

      // Some of these calculations could be factored out and memoized for use cases such as vertex iteration, but that makes the code less reusable for use cases like projection.
      switch (c.lat.face) {
        case 0: { // north pole
            if (c.lat.div == 0) {
              return Vector3.up;
            }

            Vector2 a = icoNormals[c.lon.face], b = icoNormals[(c.lon.face + 1) % 5];
            Vector2 v = Vector2.Lerp(a, b, (float)c.lon.div / c.lat.div);
            float scale = (float)c.lat.div / subdivisions;
            return (Vector3.up + new Vector3(v.x, tropicY - 1, v.y) * scale).normalized;
          }
        case 1: { // tropics
            Vector2 t;
            t.y = (float)c.lat.div / subdivisions;
            int next = (c.lon.face + 1) % 5;
            Vector2 qa = icoNormals[c.lon.face], qb = icoNormals[next], qc = icoNormals[c.lon.face + 5], qd = icoNormals[next + 5];

            Vector2 a, b;
            int boundary = subdivisions - c.lat.div;
            if (c.lon.div < boundary) {
              a = Vector2.Lerp(qa, qc, t.y);
              b = Vector2.Lerp(qb, qc, t.y);
              t.x = (float)c.lon.div / boundary;
            } else {
              a = Vector2.Lerp(qb, qc, t.y);
              b = Vector2.Lerp(qb, qd, t.y);
              t.x = (float)(c.lon.div - boundary) / c.lat.div;
            }

            var v = Vector2.Lerp(a, b, t.x);
            return new Vector3(v.x, Mathf.Lerp(tropicY, -tropicY, t.y), v.y).normalized;
          }
        case 2: { //south pole
            Vector2 a = icoNormals[c.lon.face + 5], b = icoNormals[(c.lon.face + 1) % 5 + 5];
            int len = subdivisions - c.lat.div;
            Vector2 v = Vector2.Lerp(a, b, (float)c.lon.div / len);
            float scale = (float)len / subdivisions;
            return (Vector3.down + new Vector3(v.x, 1 - tropicY, v.y) * scale).normalized;
          }
        default:
          return Vector3.down;
      }
    }

    public IEnumerable<PolarCoordinate> PolarCoordinates {
      get {
        {
          PolarCoordinate c = new PolarCoordinate();
          // north pole
          yield return c;

          for (c.lat.div = 1; c.lat.div < subdivisions; ++c.lat.div) {
            for (c.lon.face = 0; c.lon.face < 5; ++c.lon.face) {
              for (c.lon.div = 0; c.lon.div < c.lat.div; ++c.lon.div) {
                yield return c;
              }
            }
          }

          //tropics
          c.lat.face = 1;
          for (c.lat.div = 0; c.lat.div < subdivisions; ++c.lat.div) {
            for (c.lon.face = 0; c.lon.face < 5; ++c.lon.face) {
              for (c.lon.div = 0; c.lon.div < subdivisions; ++c.lon.div) {
                yield return c;
              }
            }
          }

          // south pole
          c.lat.face = 2;
          for (c.lat.div = 0; c.lat.div < subdivisions; ++c.lat.div) {
            for (c.lon.face = 0; c.lon.face < 5; ++c.lon.face) {
              int len = subdivisions - c.lat.div;
              for (c.lon.div = 0; c.lon.div < len; ++c.lon.div) {
                yield return c;
              }
            }
          }

          yield return new PolarCoordinate(3, 0, 0, 0);
        }
      }
    }

    /// <summary>
    /// Calculates the number of vertices in the equilateral pentagonal pyramid of the given height.
    /// </summary>
    private static int PyramidSize(int height) => 1 + 5 * height * (height - 1) / 2;

    public int ToIndex(PolarCoordinate c) {
      c.Normalize(subdivisions);

      // There's a lot that can be memoized here if we need to.

      switch (c.lat.face) {
        case 0:
          return c.lat.div == 0? 0 : PyramidSize(c.lat.div) + c.lon.face * c.lat.div + c.lon.div;
        case 1:
          return PyramidSize(subdivisions) + c.lat.div * 5 * subdivisions + c.lon.face * subdivisions + c.lon.div;
        case 2:
          int latLen = subdivisions - c.lat.div;
          return 2 * PyramidSize(subdivisions) + 6 * subdivisions * subdivisions - PyramidSize(latLen + 1) + c.lon.face * latLen + c.lon.div;
        default:
          return 2 * PyramidSize(subdivisions) + 6 * subdivisions * subdivisions - 1;
      }
    }

    public IEnumerable<Vector3> Vertices {
      get {
        foreach (var polar in PolarCoordinates) {
          yield return ToCartesian(polar);
        }
      }
    }

    public IEnumerable<(int a, int b)> Edges {
      get {
        // We'll mostly be doing south-facing triangles.

        PolarCoordinate p = new PolarCoordinate();

        // north pole
        {
          for (p.lat.div = 0; p.lat.div < subdivisions; ++p.lat.div) {
            for (p.lon.face = 0; p.lon.face < 5; ++p.lon.face) {
              p.lon.div = 0;
              yield return (ToIndex(p), ToIndex(p + (1, 0)));

              for (; p.lon.div < p.lat.div; ++p.lon.div) {
                int a = ToIndex(p),
                    b = ToIndex(p + (0, 1)),
                    c = ToIndex(p + (1, 1));
                yield return (a, b);
                yield return (a, c);
                yield return (b, c);
              }
            }
          }
        }

        // tropics
        {
          p.lat.face = 1;

          for (p.lat.div = 0; p.lat.div < subdivisions; ++p.lat.div) {
            for (p.lon.face = 0; p.lon.face < 5; ++p.lon.face) {
              for (p.lon.div = 0; p.lon.div < subdivisions; ++p.lon.div) {
                int a = ToIndex(p),
                    b = ToIndex(p + (0, 1)),
                    c = ToIndex(p + (1, 0));
                yield return (a, b);
                yield return (a, c);
                yield return (b, c);
              }
            }
          }
        }

        // south pole
        {
          p.lat.face = 2;

          for (p.lat.div = 0; p.lat.div < subdivisions; ++p.lat.div) {
            for (p.lon.face = 0; p.lon.face < 5; ++p.lon.face) {
              for (p.lon.div = 0; p.lon.div < subdivisions - p.lat.div; ++p.lon.div) {
                int a = ToIndex(p),
                    b = ToIndex(p + (0, 1)),
                    c = ToIndex(p + (1, 0));
                yield return (a, b);
                if (p.lon.div > 0)
                  yield return (a, c);
                yield return (b, c);
              }
            }
          }
        }
      }
    }
  }
}
