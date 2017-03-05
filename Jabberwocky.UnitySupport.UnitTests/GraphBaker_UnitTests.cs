
namespace Jabberwocky.UnitySupport.UnitTests
{
  using System;
  using System.Collections.Generic;
  using FluentAssertions;
  using NUnit.Framework;
  using UnityEngine;

  [TestFixture]
  public class GraphBaker_UnitTests
  {
    [Test]
    public void Bake_PointsListIsNull_ThrowsMeaningfulException()
    {
      // Arrange
      Action action = () => { GraphBaker.Bake(null, null); };

      // Act and Assert
      action.ShouldThrow<NullReferenceException>().WithMessage("Parameter 'points' is null.");
    }

    [Test]
    public void Bake_PointsListIsEmpty_ThrowsMeaningfulException()
    {
      // Arrange
      Action action = () => { GraphBaker.Bake(new List<Vector2>(), null); };

      // Act and Assert
      action.ShouldThrow<Exception>().WithMessage("Must have a minimum of two points to bake graph. Count is 0.");
    }

    [Test]
    public void Bake_PointsListIsLessThanTwo_ThrowsMeaningfulException()
    {
      // Arrange
      Action action = () => { GraphBaker.Bake(new List<Vector2> { Vector2.zero }, null); };

      // Act and Assert
      action.ShouldThrow<Exception>().WithMessage("Must have a minimum of two points to bake graph. Count is 1.");
    }

    [Test]
    public void Bake_RaycastFunctionIsNull_ThrowsMeaningfulException()
    {
      // Arrange
      Action action = () => { GraphBaker.Bake(new List<Vector2> { Vector2.zero, Vector2.one }, null); };

      // Act and Assert
      action.ShouldThrow<NullReferenceException>().WithMessage("Parameter 'canTravelBetweenPoints' is null.");
    }

    [Test]
    public void Bake_TwoConnectedPoints_ReturnsCorrectGraph()
    {
      // Arrange
      var point1 = Vector2.zero;
      var point2 = Vector2.one;
      var points = new List<Vector2> { point1, point2 };
      var distanceBetweenPoint1AndPoint2 = Vector2.Distance(point1, point2);

      // Act
      var graph = GraphBaker.Bake(points,
        (s, e) => 
        {
          if (s == point1 && e == point2)
          {
            return true;
          }

          throw new Exception("Parameters not recognised.");
        });

      // Assert
      graph.Should().NotBeNull();
      graph.Verticies.Should().NotBeNullOrEmpty();
      graph.Verticies.Length.Should().Be(2);
      graph.Distances.Length.Should().Be(4);
      graph.Distances[0, 0].Should().Be(PointGraph.CannotBeReached);
      graph.Distances[0, 1].Should().Be(distanceBetweenPoint1AndPoint2);
      graph.Distances[1, 0].Should().Be(distanceBetweenPoint1AndPoint2);
      graph.Distances[1, 1].Should().Be(PointGraph.CannotBeReached);
    }

    [Test]
    public void Bake_TwoUnconnectedPoints_ReturnsCorrectGraph()
    {
      // Arrange
      var point1 = Vector2.zero;
      var point2 = Vector2.one;
      var points = new List<Vector2> { point1, point2 };
      var distanceBetweenPoint1AndPoint2 = Vector2.Distance(point1, point2);

      // Act
      var graph = GraphBaker.Bake(points,
        (s, e) =>
        {
          if (s == point1 && e == point2)
          {
            return false;
          }

          throw new Exception("Parameters not recognised.");
        });

      // Assert
      graph.Should().NotBeNull();
      graph.Verticies.Should().NotBeNullOrEmpty();
      graph.Verticies.Length.Should().Be(2);
      graph.Distances.Length.Should().Be(4);
      graph.Distances[0, 0].Should().Be(PointGraph.CannotBeReached);
      graph.Distances[0, 1].Should().Be(PointGraph.CannotBeReached);
      graph.Distances[1, 0].Should().Be(PointGraph.CannotBeReached);
      graph.Distances[1, 1].Should().Be(PointGraph.CannotBeReached);
    }
  }
}
