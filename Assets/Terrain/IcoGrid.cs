using System;
using System.Collections.Generic;

public class IcoGrid<T> {
  public class Edge {
    private readonly T[] vertices;
    public readonly int a, b;

    public Edge(IcoGrid<T> grid, int a, int b) {
      vertices = grid.vertices;
      this.a = a;
      this.b = b;
    }

    public T A {
      get => vertices[a];
      set => vertices[a] = value;
    }
    public T B {
      get => vertices[b];
      set => vertices[b] = value;
    }
  }

  public readonly int subdivisions;

  public readonly T[] vertices;

  public IcoGrid(int subdivisions) {
    if (subdivisions < 1) {
      throw new ArgumentOutOfRangeException("subdivisions", subdivisions, "subdivisions must be >= 1");
    }

    this.subdivisions = subdivisions;
    vertices = new T[2 * subdivisions * 5 * subdivisions + 2];
  }

  public IEnumerable<Edge> Edges {
    get {
      // We'll mostly be doing south-facing triangles.

      // north pole
      {
        int vertex(int lat, int face, int div) {
          if (lat == 0) return 0;
          // latLength = lat
          return 1 + 5 * lat * (lat - 1) / 2 + (face * lat + div) % (5 * lat);
        }

        for (int lat = 0; lat < subdivisions; ++lat) {
          for (int face = 0; face < 5; ++face) {
            yield return new Edge(this, vertex(lat, face, 0), vertex(lat + 1, face, 0));

            for (int div = 0; div < lat; ++div) {
              int a = vertex(lat, face, div),
                  b = vertex(lat, face, div + 1),
                  c = vertex(lat + 1, face, div + 1);
              yield return new Edge(this, a, b);
              yield return new Edge(this, a, c);
              yield return new Edge(this, b, c);
            }
          }
        }
      }

      // tropics
      int tropicsStart = 1 + 5 * subdivisions * (subdivisions - 1) / 2;
      {
        int latLength = 5 * subdivisions;
        // restart at lat = 0 for simplicity; faces omitted since each face and latitude is functionally identical until the next tropic
        int vertex(int lat, int div) => tropicsStart + lat * latLength + div % latLength;

        for (int lat = 0; lat < subdivisions; ++lat) {
          for (int div = 0; div < latLength; ++div) {
            int a = vertex(lat, div),
                b = vertex(lat, div + 1),
                c = vertex(lat + 1, div);
            yield return new Edge(this, a, b);
            yield return new Edge(this, a, c);
            yield return new Edge(this, b, c);
          }
        }
      }

      // south pole
      {
        int start = tropicsStart + 5 * subdivisions * subdivisions;
        int vertex(int lat, int face, int div) {
          int latLength = subdivisions - lat;
          int latStart = start + 5 * lat * (subdivisions + latLength + 1) / 2;
          return lat == subdivisions ? latStart : latStart + (face * latLength + div) % (5 * latLength);
        }

        for (int lat = 0; lat < subdivisions; ++lat) {
          for (int face = 0; face < 5; ++face) {
            for (int div = 0; div < subdivisions - lat; ++div) {
              int a = vertex(lat, face, div),
                  b = vertex(lat, face, div + 1),
                  c = vertex(lat + 1, face, div);
              yield return new Edge(this, a, b);
              if (div > 0)
                yield return new Edge(this, a, c);
              yield return new Edge(this, b, c);
            }
          }
        }
      }
    }
  }
}
