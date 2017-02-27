
namespace Jabberwocky.UnitySupport.UnitTests
{
  using System;
  using System.Collections.Generic;
  using NUnit.Framework;
  using FluentAssertions;
  using UnityEngine;

  [TestFixture]
  public class PathFinder_UnitTests
  {
    private const Single Unreachable = Single.NegativeInfinity;

    private const Single SamePosition = Single.NegativeInfinity;

    [Test]
    public void CalculatePath_NoCollision_DestinationAddedToWaypoints()
    {
      // Arrange
      var start = new Vector2(-1, 0);
      var end = new Vector2(2, 0);
      var verticies = new Vector2[0];
      var distances = new Single[0, 0];

      var pointGraph = new PointGraph(verticies, distances);

      var points = new LinkedList<Vector2>();

      var pathFinder = new PathFinder(
        pointGraph, 
        (s, e) => { throw new Exception("Should not call this"); }, 
        (s, e) => { return true; });

      // Act
      pathFinder.CalculatePath(start, end, points);

      // Assert
      points.Should().NotBeNull();
      points.Count.Should().Be(1);
      points.First.Value.Should().Be(end);
    }

    [Test]
    public void CalculatePath_MovingAlongBaseOfThreePointIsoscelesTriangle_PathAlongBaseAddedToWaypoints()
    {
      // Arrange
      var start = new Vector2(-1, 0);
      var end = new Vector2(2, 0);
      var vertexA = Vector2.zero;
      var vertexB = new Vector2(1, 0);
      var vertexC = new Vector2(0.5f, 4);
      var verticies = new Vector2[] { vertexA, vertexB, vertexC };

      var distanceFromAToB = 1;
      var distanceFromAToC = 10;
      var distanceFromBToA = 1;
      var distanceFromBToC = 10;
      var distanceFromCToA = 10;
      var distanceFromCToB = 10;
      var distances = new Single[,]
      {
        { SamePosition, distanceFromAToB, distanceFromAToC },
        { distanceFromBToA, SamePosition, distanceFromBToC },
        { distanceFromCToA, distanceFromCToB, SamePosition }
      };

      var pointGraph = new PointGraph(verticies, distances);

      Func<Vector2, Vector2, Single> distanceFinder = (s, e) =>
      {
        if (s == start && e == vertexA)
        {
          return 1;
        }

        if (s == start && e == vertexB)
        {
          return Unreachable;
        }

        if (s == start && e == vertexC)
        {
          return 10;
        }

        if (s == end && e == vertexA)
        {
          return Unreachable;
        }

        if (s == end && e == vertexB)
        {
          return 1;
        }

        if (s == end && e == vertexC)
        {
          return 10;
        }

        throw new Exception("Parameters not recognised.");
      };

      var points = new LinkedList<Vector2>();

      var pathFinder = new PathFinder(pointGraph, distanceFinder, (s, e) => { return false; });

      // Act
      pathFinder.CalculatePath(start, end, points);

      // Assert
      points.Should().NotBeNull();
      points.Count.Should().Be(3);
      points.First.Value.Should().Be(vertexA);
      points.RemoveFirst();
      points.First.Value.Should().Be(vertexB);
      points.RemoveFirst();
      points.First.Value.Should().Be(end);
    }

    [Test]
    public void CalculatePath_MovingAlongBaseOfFourPointIsoscelesTriangle_PathAlongBaseAddedToWaypoints()
    {
      // Arrange
      var vertexA = Vector2.zero;
      var vertexB = new Vector2(1, 0);
      var vertexC = new Vector2(2, 0);
      var vertexD = new Vector2(1, 9);
      var verticies = new Vector2[] { vertexA, vertexB, vertexC, vertexD };

      var distanceFromAToB = 1;
      var distanceFromAToD = 10;
      var distanceFromBToA = 1;
      var distanceFromBToC = 1;
      var distanceFromBToD = 10;
      var distanceFromCToB = 1;
      var distanceFromCToD = 10;
      var distanceFromDToA = 10;
      var distanceFromDToB = 10;
      var distanceFromDToC = 10;
      var distances = new Single[,]
      {
        { SamePosition, distanceFromAToB, Unreachable, distanceFromAToD },
        { distanceFromBToA, SamePosition, distanceFromBToC, distanceFromBToD },
        { Unreachable, distanceFromCToB, SamePosition, distanceFromCToD },
        { distanceFromDToA, distanceFromDToB, distanceFromDToC, SamePosition }
      };

      var pointGraph = new PointGraph(verticies, distances);

      var start = new Vector2(-1, 0);
      var end = new Vector2(3, 0);
      var distanceFromStartToA = 1;
      var distanceFromStartToB = Unreachable;
      var distanceFromStartToC = Unreachable;
      var distanceFromStartToD = 10;
      var distanceFromEndToA = Unreachable;
      var distanceFromEndToB = Unreachable;
      var distanceFromEndToC = 1;
      var distanceFromEndToD = 10;

      Func<Vector2, Vector2, Single> distanceFinder = (s, e) =>
      {
        if (s == start && e == vertexA)
        {
          return distanceFromStartToA;
        }

        if (s == start && e == vertexB)
        {
          return distanceFromStartToB;
        }

        if (s == start && e == vertexC)
        {
          return distanceFromStartToC;
        }

        if (s == start && e == vertexD)
        {
          return distanceFromStartToD;
        }

        if (s == end && e == vertexA)
        {
          return distanceFromEndToA;
        }

        if (s == end && e == vertexB)
        {
          return distanceFromEndToB;
        }

        if (s == end && e == vertexC)
        {
          return distanceFromEndToC;
        }

        if (s == end && e == vertexD)
        {
          return distanceFromEndToD;
        }

        throw new Exception("Parameters not recognised.");
      };

      var points = new LinkedList<Vector2>();

      var pathFinder = new PathFinder(pointGraph, distanceFinder, (s, e) => { return false; });

      // Act
      pathFinder.CalculatePath(start, end, points);

      // Assert
      points.Should().NotBeNull();
      points.Count.Should().Be(4);
      points.First.Value.Should().Be(vertexA);
      points.RemoveFirst();
      points.First.Value.Should().Be(vertexB);
      points.RemoveFirst();
      points.First.Value.Should().Be(vertexC);
      points.RemoveFirst();
      points.First.Value.Should().Be(end);
    }
  }
}
