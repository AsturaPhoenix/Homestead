using System;

namespace Assets
{
  public class MatrixView : IMatrix
  {
    private readonly int rows, columns;
    private readonly Func<int, int, float> getter;
    private readonly Action<int, int, float> setter;

    public MatrixView(int rows, int columns, Func<int, int, float> getter, Action<int, int, float> setter) {
      this.rows = rows;
      this.columns = columns;
      this.getter = getter;
      this.setter = setter;
    }

    public MatrixView(int rows, int columns, Func<int, int, float> getter): this(rows, columns, getter, null) { }

    public float this[Index row, Index column] {
      get => getter(Math.CheckedOffset(row, rows), Math.CheckedOffset(column, columns));
      set => setter(Math.CheckedOffset(row, rows), Math.CheckedOffset(column, columns), value);
    }

    public int Rows => rows;
    public int Columns => columns;

    public override string ToString() => IMatrix.ToString(this);
  }
}