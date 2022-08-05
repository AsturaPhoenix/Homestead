using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets
{
  public interface IMatrix : IEnumerable<float>
  {
    int Rows { get; }
    int Columns { get; }
    float this[Index row, Index column] { get; set; }

    IEnumerator<float> IEnumerable<float>.GetEnumerator() {
      for (int row = 0; row < Rows; ++row) {
        for (int column = 0; column < Columns; ++column) {
          yield return this[row, column];
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static IMatrix operator *(float c, IMatrix m) => new MatrixView(m.Rows, m.Columns, (row, column) => c * m[row, column]);
    public static IMatrix operator *(IMatrix m, float c) => c * m;
    public static IMatrix operator /(IMatrix m, float c) => 1 / c * m;

    public static IMatrix operator +(IMatrix a, IMatrix b) {
      Debug.Assert(a.Rows == b.Rows && a.Columns == b.Columns);
      return new MatrixView(a.Rows, a.Columns, (row, column) => a[row, column] + b[row, column]);
    }

    public static IMatrix operator -(IMatrix a, IMatrix b) => a + -1 * b;

    public static string ToString(IMatrix m) {
      StringBuilder s = new("(");
      for (int row = 0; row < m.Rows; ++row) {
        if (row > 0) {
          s.Append("\n");
        }
        s.Append(m.Row(row));
      }
      return s.Append(")").ToString();
    }
  }
}