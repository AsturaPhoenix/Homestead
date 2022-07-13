using System;

namespace Assets
{
  public class SquareMatrix : Matrix, ISquareMatrix
  {
    public SquareMatrix(int size): base(size, size) {
    }

    public SquareMatrix(ISquareMatrix source): this(source.Size) {
      this.Set(source);
    }

    public SquareMatrix(float[,] data): base(data) {
      if (Rows != Columns) {
        throw new ArgumentException("Array is not square.");
      }
    }

    public int Size => Rows;

    public override string ToString() => IMatrix.ToString(this);
  }
}