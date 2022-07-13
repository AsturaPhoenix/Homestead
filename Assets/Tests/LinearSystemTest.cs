using NUnit.Framework;
using Assets;
using System;
using Math = Assets.Math;

namespace Tests
{
  public class LinearSystemTest
  {
    [Test]
    public void Trivial() {
      Assert.AreEqual(new Vector(1, 2, 3), Math.Solve(
        new SquareMatrix(new float[,]{ { 1, 0, 0 },
                                       { 0, 1, 0 },
                                       { 0, 0, 1 } }),
        new Vector(1, 2, 3)));
    }

    [Test]
    public void NeedSwap() {
      Assert.AreEqual(new Vector(1, 2, 3), Math.Solve(
        new SquareMatrix(new float[,]{ { 1, 0, 0 },
                                       { 0, 0, 1 },
                                       { 0, 1, 0 } }),
        new Vector(1, 3, 2)));
    }

    [Test]
    public void Solve() {
      Assert.AreEqual(new Vector(-1, 0, 1), Math.Solve(
        new SquareMatrix(new float[,]{ { 3, 1, 4},
                                        { 1, 5, 9 },
                                        { 2, 6, 5 } }),
        new Vector(1, 8, 3)));
    }

    [Test]
    public void Underconstrained() {
      Assert.Throws<ArgumentException>(() => Math.Solve(
        new SquareMatrix(new float[,]{ { 1, 2, 3 },
                                       { 2, 4, 6 },
                                       { 3, 6, 9 } }),
        new Vector(4, 8, 12)));
    }
  }
}
