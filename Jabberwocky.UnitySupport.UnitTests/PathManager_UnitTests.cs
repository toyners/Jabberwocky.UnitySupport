
namespace Jabberwocky.UnitySupport
{
  using System;
  using NUnit.Framework;
  using FluentAssertions;
  using UnityEngine;

  [TestFixture]
  public class PathManager_UnitTests
  {
    [Test]
    public void GetNextWaypoint_NoDestination_ThrowsMeaningfulException()
    {
      var pathManager = new PathManager(null);
      Action action = () => pathManager.GetNextWaypoint();
      action.ShouldThrow<Exception>().WithMessage("No waypoints");
    }

    [Test]
    public void GetNextWaypoint_SingleDestinationInOpenTerrain_ReturnsDestination()
    {
      var pathManager = new PathManager((p, d, w) => { w.AddLast(d); });
      var position = new Vector2(0f, 0f);
      var destination = new Vector2(1f, 0f);

      pathManager.SetDestination(position, destination);

      pathManager.GetNextWaypoint().Should().Be(destination);
      pathManager.Count.Should().Be(0);
    }

    [Test]
    public void GetNextWaypoint_SingleDestinationBehindWall_ReturnsPathToDestination()
    {
      var position = new Vector2(0f, 0f);
      var destination = new Vector2(1f, 0f);
      var waypoint1 = new Vector2(0.5f, 0.5f);
      var waypoint2 = new Vector2(0.8f, 0.5f);

      var pathManager = new PathManager((p, d, e) => 
      {
        if (p == position && d == destination)
        {
          e.AddLast(waypoint1);
          e.AddLast(waypoint2);
          e.AddLast(destination);
          return;
        }

        throw new Exception("Parameters not recognised.");
      });
      
      pathManager.SetDestination(position, destination);

      pathManager.GetNextWaypoint().Should().Be(waypoint1);
      pathManager.GetNextWaypoint().Should().Be(waypoint2);
      pathManager.GetNextWaypoint().Should().Be(destination);
      pathManager.Count.Should().Be(0);
    }

    // Test for path management to a destination that is behind the second wall when the walls are
    // one behind the other with significant space between then i.e. |     |
    [Test]
    public void GetNextWaypoint_OneDestinationBehindTwoWalls_ReturnsPathToDestination()
    {
      var position = new Vector2(0f, 0f);
      var destination = new Vector2(2f, 0f);
      var waypoint1 = new Vector2(0.5f, 0.5f);
      var waypoint2 = new Vector2(0.8f, 0.5f);
      var waypoint3 = new Vector2(1.5f, 0.5f);
      var waypoint4 = new Vector2(1.8f, 0.5f);

      var pathManager = new PathManager((p, d, w) =>
      {
        if (p == position && d == destination)
        {
          w.AddLast(waypoint1);
          w.AddLast(waypoint2);
          w.AddLast(waypoint3);
          w.AddLast(waypoint4);
          w.AddLast(destination);
          return;
        }

        throw new Exception("Parameters not recognised.");
      });

      pathManager.SetDestination(position, destination);
      pathManager.GetNextWaypoint().Should().Be(waypoint1);
      pathManager.GetNextWaypoint().Should().Be(waypoint2);
      pathManager.GetNextWaypoint().Should().Be(waypoint3);
      pathManager.GetNextWaypoint().Should().Be(waypoint4);
      pathManager.GetNextWaypoint().Should().Be(destination);
      pathManager.Count.Should().Be(0);
    }

    [Test]
    public void GetNextWaypoint_TwoDestinationsBehindTwoWalls_ReturnsPathToBothDestinations()
    {
      var position = new Vector2(0f, 0f);
      var destination1 = new Vector2(1f, 0f);
      var destination2 = new Vector2(2.0f, 0.5f);
      var waypoint1 = new Vector2(0.5f, 0.5f);
      var waypoint2 = new Vector2(0.8f, 0.5f);
      var waypoint3 = new Vector2(1.5f, 0.5f);
      var waypoint4 = new Vector2(1.8f, 0.5f);

      var pathManager = new PathManager((p, d, w) =>
      {
        if (p == position && d == destination1)
        {
          w.AddLast(waypoint1);
          w.AddLast(waypoint2);
          w.AddLast(destination1);
          return;
        }

        if (p == destination1 && d == destination2)
        {
          w.AddLast(waypoint3);
          w.AddLast(waypoint4);
          w.AddLast(destination2);
          return;
        }

        throw new Exception("Parameters not recognised.");
      });

      pathManager.SetDestination(position, destination1);
      pathManager.GetNextWaypoint().Should().Be(waypoint1);

      pathManager.AddDestination(destination2);
      pathManager.GetNextWaypoint().Should().Be(waypoint2);
      pathManager.GetNextWaypoint().Should().Be(destination1);
      pathManager.GetNextWaypoint().Should().Be(waypoint3);
      pathManager.GetNextWaypoint().Should().Be(waypoint4);
      pathManager.GetNextWaypoint().Should().Be(destination2);
      pathManager.Count.Should().Be(0);
    }

    [Test]
    public void GetNextWaypoint_AddDestinationWithNoWaypoints_ThrowsMeaningfulException()
    {
      var position = new Vector2(0f, 0f);
      var destination1 = new Vector2(1f, 0f);
      var destination2 = new Vector2(2f, 0f);

      var pathManager = new PathManager((p, d, w) => 
      {
        if (p == position && d == destination1)
        {
          w.AddLast(destination1);
          return;
        }

        throw new Exception("Parameters not recognised.");
      });

      pathManager.SetDestination(position, destination1);
      pathManager.GetNextWaypoint().Should().Be(destination1);

      Action action = () => { pathManager.AddDestination(destination2); };

      action.ShouldThrow<Exception>().WithMessage("Cannot add a destination with no waypoints present. Use SetDestination instead.");
    }
  }
}
