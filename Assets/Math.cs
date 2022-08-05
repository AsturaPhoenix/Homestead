using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
  public static class Math
  {
    public static int FloorMod(int a, int b) {
      int mod = a % b;
      return a >= 0 ? mod : mod + Mathf.Abs(b);
    }

    public static int DivRem(int a, int b, out int rem) {
      int q = System.Math.DivRem(a, b, out rem);
      if (rem < 0) {
        --q;
        rem += Mathf.Abs(b);
      }
      return q;
    }

    public static IEnumerable<Vector3Int> IntCube(int n) {
      for (int z = 0; z < n; ++z) {
        for (int y = 0; y < n; ++y) {
          for (int x = 0; x < n; ++x) {
            yield return new(x, y, z);
          }
        }
      }
    }

    public static Color ColorScale(float v, params Color[] colors) {
      if (v < 0) {
        return colors[0];
      }
      if (v >= colors.Length - 1) {
        return colors[^1];
      }
      return Color.Lerp(colors[(int)v], colors[(int)v + 1], v - (int)v);
    }

    public static (int, float) ArgMax(IEnumerable<float> terms) {
      int i = 0;
      var it = terms.GetEnumerator();

      if (!it.MoveNext()) {
        return (-1, float.NaN);
      }

      float candidateValue = it.Current;
      int candidateIndex = 0;

      while (it.MoveNext()) {
        ++i;
        if (it.Current > candidateValue) {
          candidateValue = it.Current;
          candidateIndex = i;
        }
      }

      return (candidateIndex, candidateValue);
    }

    public static int CheckedOffset(Index index, int length) {
      int offset = index.GetOffset(length);
      AssertIndex(offset, length);
      return offset;
    }

    [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
    public static void AssertIndex(int index, int length) {
      Debug.Assert(index >= 0 && index < length);
    }

    public static void Set(this IMatrix m, IMatrix other) {
      Debug.Assert(m.Rows == other.Rows && m.Columns == other.Columns);

      for (int row = 0; row < m.Rows; ++row) {
        for (int column = 0; column < m.Columns; ++column) {
          m[row, column] = other[row, column];
        }
      }
    }

    public static IMatrix Set(this IMatrix m, Func<IMatrix, IMatrix> f) {
      m.Set(f(m));
      return m;
    }

    public static IMatrix SubMatrix(this IMatrix m, int rowStart, int columnStart, int rowCount, int columnCount) =>
      new MatrixView(rowCount, columnCount,
        (row, column) => m[rowStart + row, columnStart + column],
        (row, column, value) => m[rowStart + row, columnStart + column] = value);

    public static IMatrix SubMatrix(this IMatrix m, Range rows, Range columns) {
      var (rowStart, rowCount) = rows.GetOffsetAndLength(m.Rows);
      var (columnStart, columnCount) = columns.GetOffsetAndLength(m.Columns);
      return m.SubMatrix(rowStart, columnStart, rowCount, columnCount);
    }

    public static IVector Row(this IMatrix m, int row) =>
      new VectorView(m.Columns, column => m[row, column], (column, value) => m[row, column] = value);
    public static IVector Row(this IMatrix m, Index row) => m.Row(row.GetOffset(m.Rows));
    public static IEnumerable<IVector> Rows(this IMatrix m, Range rows) {
      int start = rows.Start.GetOffset(m.Rows), end = rows.End.GetOffset(m.Rows);
      for (int row = start; row < end; ++row) {
        yield return m.Row(row);
      }
    }
    public static IVector Column(this IMatrix m, int column) =>
      new VectorView(m.Rows, row => m[row, column], (row, value) => m[row, column] = value);
    public static IVector Column(this IMatrix m, Index column) => m.Column(column.GetOffset(m.Columns));
    public static IEnumerable<IVector> Columns(this IMatrix m, Range columns) {
      int start = columns.Start.GetOffset(m.Columns), end = columns.End.GetOffset(m.Columns);
      for (int column = start; column < end; ++column) {
        yield return m.Column(column);
      }
    }

    public static void SetSubmatrix(this IMatrix m, int rowStart, int columnStart, IMatrix value) {
      for (int row = 0; row < value.Rows; ++row) {
        for (int column = 0; column < value.Columns; ++column) {
          m[rowStart + row, columnStart + column] = value[row, column];
        }
      }
    }

    private class SquareMatrixView : ISquareMatrix
    {
      private readonly IMatrix backing;

      public SquareMatrixView(IMatrix backing) {
        if (backing.Rows != backing.Columns) {
          throw new ArgumentException("Matrix is not square.");
        }
        this.backing = backing;
      }

      public float this[Index row, Index column] { get => backing[row, column]; set => backing[row, column] = value; }

      public int Size => backing.Rows;
    }

    public static ISquareMatrix AsSquareMatrix(this IMatrix m) => m is ISquareMatrix s ? s : new SquareMatrixView(m);

    public static IMatrix RowVector(this IVector v) => new MatrixView(1, v.Length, (row, column) => v[column], (row, column, value) => v[column] = value);
    public static IMatrix ColumnVector(this IVector v) => new MatrixView(v.Length, 1, (row, column) => v[row], (row, column, value) => v[row] = value);

    public static void Set(this IVector v, IVector other) {
      Debug.Assert(v.Length == other.Length);

      for (int i = 0; i < v.Length; ++i) {
        v[i] = other[i];
      }
    }

    public static IVector Set(this IVector v, Func<IVector, IVector> f) {
      v.Set(f(v));
      return v;
    }

    public static IVector Slice(this IVector v, int start, int count) => new VectorView(count, i => v[start + i], (i, value) => v[start + i] = value);
    public static IVector Slice(this IVector v, Range range) {
      var (start, count) = range.GetOffsetAndLength(v.Length);
      return v.Slice(start, count);
    }

    public static SymmetricMatrix SelfOuterProduct(IVector v) {
      SymmetricMatrix mat = new(v.Length);
      for (int row = 0; row < v.Length; ++row) {
        for (int col = 0; col <= row; ++col) {
          mat.SetSymmetric(row, col, v[row] * v[col]);
        }
      }

      return mat;
    }

    /// <summary>
    /// Solves a linear system of the form ax + by + ... = c. The system must be fully constrained and not overconstrained.
    /// </summary>
    /// <param name="coefficients">The coefficients of the linear terms on one side of the equation.</param>
    /// <param name="constants">The constants on the other side of the equation.</param>
    public static Vector Solve(ISquareMatrix coefficients, IVector constants) {
      Matrix scratch = new(coefficients.Rows, coefficients.Columns + 1);
      scratch.SetSubmatrix(0, 0, coefficients);
      scratch.Column(^1).Set(constants);

      void swapRows(int a, int b) {
        if (a != b) {
          // We really only need to swap the parts left of op, but go ahead and keep it simple.
          Vector tmp = new(scratch.Row(a));
          scratch.Row(a).Set(scratch.Row(b));
          scratch.Row(b).Set(tmp);
        }
      }

      for (int op = 0; op < scratch.Rows; ++op) {
        var (i, c) = ArgMax(from x in scratch.Column(op).Slice(op..) select Mathf.Abs(x));
        if (c == 0) {
          throw new ArgumentException("System is underconstrained.");
        }
        swapRows(op, op + i);
        var pivotRow = scratch.Row(op).Slice((op + 1)..).Set(v => v / scratch[op, op]);
        foreach (var row in scratch.Rows((op + 1)..)) {
          row.Slice((op + 1)..).Set(v => v - row[op] * pivotRow);
        }
      }

      Vector solution = new(scratch.Column(^1));

      for (int elim = scratch.Rows - 1; elim > 0; --elim) {
        for (int op = 0; op < elim; ++op) {
          solution[op] -= scratch[op, elim] * solution[elim];
        }
      }

      return solution;
    }
  }
}