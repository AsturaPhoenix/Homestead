
namespace Assets
{
  public interface ISquareMatrix : IMatrix
  {
    /// <summary>
    /// The side length of the matrix.
    /// </summary>
    int Size { get; }
    int IMatrix.Rows => Size;
    int IMatrix.Columns => Size;
  }
}