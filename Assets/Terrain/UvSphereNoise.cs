using UnityEngine;

public class UvSphereNoise : MonoBehaviour {
  public Vector2 seed;
  public int resolution = 512;
  public float range = 1;

  private Texture2D texture;
  private Color[] pix;

  void Start() {
    Texture2D texture = new Texture2D(2 * resolution, resolution);
    GetComponent<Renderer>().material.mainTexture = texture;
    Color[] px = new Color[texture.width * texture.height];

    for (int y = 0; y < resolution; ++y) {
      for (int x = 0; x < 2 * resolution; ++x) {
        float lerp = Mathf.Lerp(Sample(x + 2 * resolution, y), Sample(x, y), .5f * x / resolution);
        px[y * texture.width + x] = Color((1 + Mathf.Cos(2 * lerp * Mathf.PI)) / 2);
      }
    }

    texture.SetPixels(px);
    texture.Apply();
  }

  private float Sample(int x, int y) {
    return Mathf.PerlinNoise(seed.x + range * x / resolution, seed.y + range * y / resolution);
  }

  private Color Color(float sample) {
    return new Color(Mathf.Clamp01(2 * sample), Mathf.Clamp01(2 * sample - 1), 0);
  }
}
