
namespace Jabberwocky.UnitySupport
{
  using System;
  using System.Collections.Generic;
  using UnityEngine;

  public class PathManager
  {
    #region Members
    private LinkedList<Vector2> waypoints;

    private Action<Vector2, Vector2, LinkedList<Vector2>> pathFinder;

    private PathDisplay pathDisplay;
    #endregion

    #region Construction
    public PathManager(PathDisplay pathDisplay, Action<Vector2, Vector2, LinkedList<Vector2>> pathFinder)
    {
      this.waypoints = new LinkedList<Vector2>();
      this.pathFinder = pathFinder;
      this.pathDisplay = pathDisplay;
    }

    public PathManager(Action<Vector2, Vector2, LinkedList<Vector2>> pathFinder) : this(null, pathFinder)
    {
    }
    #endregion

    #region Properties
    public Int32 Count
    {
      get { return this.waypoints.Count; }
    }
    #endregion

    #region Methods
    public void AddDestination(Vector2 position)
    {
      if (this.waypoints.Count == 0)
      {
        throw new Exception("Cannot add a destination with no waypoints present. Use SetDestination instead.");
      }

      this.FinalisePathToDestination(this.waypoints.Last.Value, position);
    }

    public void AtDestination()
    {
      this.pathDisplay?.RemoveWaypointMarker();
    }

    public void Clear()
    {
      this.waypoints.Clear();
      this.pathDisplay?.ClearWaypointMarkers();
    }

    public Vector2 GetNextWaypoint()
    {
      if (this.waypoints.Count == 0)
      {
        throw new Exception("No waypoints");
      }

      var waypoint = this.waypoints.First.Value;
      this.waypoints.RemoveFirst();
      return waypoint;
    }

    public Vector2 PeekNextWaypoint()
    {
      throw new NotImplementedException();
    }

    public void SetDestination(Vector2 position, Vector2 destination)
    {
      this.Clear();

      this.FinalisePathToDestination(position, destination);
    }

    private void FinalisePathToDestination(Vector2 position, Vector2 destination)
    {
      var lastWayPoint = this.waypoints.Last;

      this.pathFinder(position, destination, this.waypoints);

      LinkedListNode<Vector2> firstWaypointToMark = (lastWayPoint == null ? this.waypoints.First : lastWayPoint.Next); 
      this.pathDisplay?.AddWaypointMarkers(firstWaypointToMark);
    }
    #endregion
  }
}
