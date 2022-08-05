using System;
using UnityEngine;

namespace Assets
{
  public class Vector : IVector
  {
    private readonly float[] data;

    public Vector(int length) {
      data = new float[length];
    }

    public Vector(params float[] data) {
      this.data = data;
    }

    public Vector(Vector3 source) : this(source.x, source.y, source.z) { }

    public Vector(IVector source) {
      data = new float[source.Length];
      this.Set(source);
    }

    public int Length => data.Length;
    public float this[Index n] {
      get => data[n];
      set => data[n] = value;
    }
    public override string ToString() => IVector.ToString(this);
  }
}