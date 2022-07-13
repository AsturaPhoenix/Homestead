using System;

namespace Assets
{
  public class VectorView : IVector
  {
    private readonly int length;
    private readonly Func<int, float> getter;
    private readonly Action<int, float> setter;

    public VectorView(int length, Func<int, float> getter, Action<int, float> setter) {
      this.length = length;
      this.getter = getter;
      this.setter = setter;
    }

    public VectorView(int length, Func<int, float> getter) : this(length, getter, null) { }

    public float this[Index n] {
      get => getter(Math.CheckedOffset(n, length));
      set => setter(Math.CheckedOffset(n, length), value);
    }

    public int Length => length;

    public override string ToString() => IVector.ToString(this);
  }
}