
namespace Jabberwocky.UnitySupport
{
  using System.Collections.Generic;
  using UnityEngine;

  public class PathDisplay
  {
    #region Fields
    private LinkedList<GameObject> waypoints = new LinkedList<GameObject>();

    private GameObject WaypointMarkerPrefab;

    private LineRenderer line;

    private Transform transform;
    #endregion

    #region Construction
    public PathDisplay(Transform transform, GameObject waypointMarkerPrefab, LineRenderer line)
    {
      this.transform = transform;
      this.WaypointMarkerPrefab = waypointMarkerPrefab;
      this.line = line;
    }
    #endregion

    #region Methods
    public void ClearWaypointMarkers()
    {
      while (this.waypoints.Count > 0)
      {
        this.RemoveWaypointMarker();
      }

      this.line.numPositions = 0;
    }

    public void AddWaypointMarkers(LinkedListNode<Vector2> wayPoint)
    {
      do
      {
        var waypointMarker = (GameObject)UnityEngine.Object.Instantiate(this.WaypointMarkerPrefab, wayPoint.Value, Quaternion.identity);
        this.waypoints.AddLast(waypointMarker);
        wayPoint = wayPoint.Next;
      }
      while (wayPoint != null);

      this.DisplayGuideLine();
    }

    public void RemoveWaypointMarker()
    {
      UnityEngine.Object.DestroyObject(this.waypoints.First.Value);
      this.waypoints.RemoveFirst();
      this.DisplayGuideLine();
    }

    private void DisplayGuideLine()
    {
      this.line.numPositions = this.waypoints.Count + 1;
      this.line.SetPosition(0, this.transform.position);
      var waypoint = this.waypoints.First;
      var i = 1;
      while (waypoint != null)
      {
        this.line.SetPosition(i++, waypoint.Value.transform.position);
        waypoint = waypoint.Next;
      }
    }
    #endregion
  }
}
