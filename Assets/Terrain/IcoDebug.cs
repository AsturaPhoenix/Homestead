using UnityEngine;
using Assets.Lattice;

namespace Assets.Terrain
{
  public class IcoDebug : MonoBehaviour
  {
    void Start() {
      var noise = new CubicPerlinNoise(4);
      const int res = 512, ovr = 16;
      var tex = new Texture2D(4 * res, 3 * res);
      tex.wrapMode = TextureWrapMode.Clamp;

      Color Sample(float x, float y, float z) {
        var v = 2 * (new Vector3(x, y, z).normalized + Vector3.one);
        var rem = noise.Sample(v);
        return new Color(rem + 1, rem, 0);
      }

      for (int y = 0; y < res; ++y) {
        for (int x = 0; x < res; ++x) {
          var v = 2 * new Vector2(x, y) / res - new Vector2(1, 1);
          tex.SetPixel(res + x, y, Sample(-v.x, -1, v.y));
          tex.SetPixel(res + x, 2 * res + y, Sample(-v.x, 1, -v.y));
          tex.SetPixel(x, res + y, Sample(1, v.y, v.x));
          tex.SetPixel(res + x, res + y, Sample(-v.x, v.y, 1));
          tex.SetPixel(2 * res + x, res + y, Sample(-1, v.y, -v.x));
          tex.SetPixel(3 * res + x, res + y, Sample(v.x, v.y, -1));
        }
      }

      for (int y = 0; y < ovr; ++y) {
        for (int x = 3 * res; x < 4 * res; ++x) {
          tex.SetPixel(x, res - 1 - y, tex.GetPixel(x, res));
          tex.SetPixel(x, 2 * res + y, tex.GetPixel(x, 2 * res - 1));
        }
        for (int x = 0; x < res; ++x) {
          {
            tex.SetPixel(res - 1 - y, x, tex.GetPixel(res, x));
            tex.SetPixel(2 * res + y, x, tex.GetPixel(2 * res - 1, x));
          }
          {
            int ly = 3 * res - 1 - x;
            tex.SetPixel(res - 1 - y, ly, tex.GetPixel(res, ly));
            tex.SetPixel(2 * res + y, ly, tex.GetPixel(2 * res - 1, ly));
          }
          {
            tex.SetPixel(x, res - 1 - y, tex.GetPixel(x, res));
            tex.SetPixel(x, 2 * res + y, tex.GetPixel(x, 2 * res - 1));
          }
          {
            int lx = 2 * res + x + ovr;
            tex.SetPixel(lx, res - 1 - y, tex.GetPixel(lx, res));
            tex.SetPixel(lx, 2 * res + y, tex.GetPixel(lx, 2 * res - 1));
          }
        }
      }

      tex.Apply();

      GetComponent<Renderer>().material.mainTexture = tex;
    }
  }
}
