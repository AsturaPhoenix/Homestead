using System;

namespace Assets
{
  // This does not implement IMutableMatrix since its setter is not consistent with matrix operations.
  public class SymmetricMatrix : ISquareMatrix
  {
    private readonly int size;
    private readonly float[] data;

    public SymmetricMatrix(int size) {
      this.size = size;
      data = new float[size * (size + 1) / 2];
    }

    private int ResolveIndex(int row, int column) {
      if (column <= row) {
        return row * (row + 1) / 2 + column;
      } else {
        return column * (column + 1) / 2 + row;
      }
    }

    private ref float Resolve(Index row, Index column) => ref data[ResolveIndex(row.GetOffset(Size), column.GetOffset(Size))];

    public float this[Index row, Index column] {
      get => Resolve(row, column);
      // Mutating a symmetric matrix is inconsistent with math operations, so require an explicit method call.
      set => throw new NotImplementedException("Use SetSymmetric to mutate a symmetric matrix.");
    }

    public void SetSymmetric(Index row, Index column, float value) {
      Resolve(row, column) = value;
    }

    // We could calculate this but we expect this to be used for loops, so recalculating each time would produce unexpected
    // inefficiency.
    public int Size => size;

    public override string ToString() => IMatrix.ToString(this);
  }
}