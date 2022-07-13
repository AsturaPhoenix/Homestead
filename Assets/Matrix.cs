using System;

namespace Assets
{
  public class Matrix : IMatrix
  {
    private readonly float[,] data;

    public Matrix(int rows, int columns) {
      data = new float[rows, columns];
    }

    public Matrix(IMatrix source): this(source.Rows, source.Columns) {
      this.Set(source);
    }

    public Matrix(float[,] data) {
      this.data = data;
    }

    private ref float Resolve(Index row, Index column) => ref data[row.GetOffset(Rows), column.GetOffset(Columns)];

    /// <summary>
    /// Gets or sets the designated value and its transposition.
    /// </summary>
    public float this[Index row, Index column] {
      get => Resolve(row, column);
      set => Resolve(row, column) = value;
    }

    public int Rows => data.GetLength(0);
    public int Columns => data.GetLength(1);

    public override string ToString() => IMatrix.ToString(this);
  }
}