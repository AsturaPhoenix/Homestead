using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets
{
  public interface IVector : IEnumerable<float>
  {
    int Length { get; }
    float this[Index n] { get; set; }

    IEnumerator<float> IEnumerable<float>.GetEnumerator() {
      for (int i = 0; i < Length; ++i) {
        yield return this[i];
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static IVector operator *(float c, IVector v) => new VectorView(v.Length, i => c * v[i]);
    public static IVector operator *(IVector v, float c) => c * v;
    public static IVector operator /(IVector v, float c) => 1 / c * v;

    public static IVector operator +(IVector a, IVector b) {
      Debug.Assert(a.Length == b.Length);
      return new VectorView(a.Length, i => a[i] + b[i]);
    }

    public static IVector operator -(IVector a, IVector b) => a + -1 * b;

    public static string ToString(IVector v) {
      StringBuilder s = new("(");
      for (int i = 0; i < v.Length; ++i) {
        if (i > 0) {
          s.Append(", ");
        }
        s.Append(v[i]);
      }
      return s.Append(")").ToString();
    }
  }
}