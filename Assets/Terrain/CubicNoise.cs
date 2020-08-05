using UnityEngine;

public class CubicNoise : MonoBehaviour {
  public Vector2 seed;
  public int resolution = 512;
  public float range = 1;

  private Texture2D texture;
  private Color[] pix;

  void Start() {
    Texture2D texture = new Texture2D(4 * resolution, 3 * resolution);
    GetComponent<Renderer>().material.mainTexture = texture;
    Color[] px = new Color[texture.width * texture.height];

    for (int y = 0; y < resolution; ++y) {
      for (int x = 0; x < 4 * resolution; ++x) {
        px[(y + resolution) * texture.width + x] = RawColor(x, y + resolution);
      }
      for (int x = 0; x < resolution; ++x) {
        px[y * texture.width + x + resolution] = RawColor(x + resolution, y);
        px[(y + 2 * resolution) * texture.width + x + resolution] = RawColor(x + resolution, y + 2 * resolution);
      }
    }

    texture.SetPixels(px);
    texture.Apply();
  }

  private Color RawColor(int x, int y) {
    return Color(Mathf.PerlinNoise(seed.x + range * x / resolution, seed.y + range * y / resolution));
  }

  private Color Color(float sample) {
    return new Color(Mathf.Clamp01(2 * sample), Mathf.Clamp01(2 * sample - 1), 0);
  }
}
