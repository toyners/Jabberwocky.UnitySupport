
namespace Jabberwocky.UnitySupport
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using UnityEngine;

  [DebuggerDisplay("Name: {Name}, x: {Coordinates.x}, y: {Coordinates.y}")]
  public class Node
  {
    #region Fields
    public String Name;

    public List<Path> TemporaryPaths;

    public Vector2 Coordinates;

    private List<Path> permanentPaths;
    #endregion

    #region Construction
    public Node(String name, Vector2 coordinates, Int32 pathCount)
    {
      this.Name = name;
      this.Coordinates = coordinates;
      this.permanentPaths = new List<Path>(pathCount);
      this.TemporaryPaths = new List<Path>(2);
    }

    public Node(Vector2 coordinates, Int32 pathCount) : this("", coordinates, pathCount)
    {
    }
    #endregion

    #region Properties
    public IEnumerable<Path> NeighbourPaths
    {
      get
      {
        for (Int32 index = 0; index < this.permanentPaths.Count; index++)
        {
          yield return this.permanentPaths[index];
        }

        for (Int32 index = 0; index < this.TemporaryPaths.Count; index++)
        {
          yield return this.TemporaryPaths[index];
        }
      }
    }
    #endregion

    #region Methods
    public void AddPathToNeighbour(Node node)
    {
      this.AddPathToNeighbour(this.permanentPaths, node);
    }

    public void AddPathToTemporaryNeighbour(Node node)
    {
      this.AddPathToNeighbour(this.TemporaryPaths, node);
    }

    private void AddPathToNeighbour(List<Path> paths, Node node)
    {
      var path = new Path
      {
        Node = node,
        Distance = Vector2.Distance(this.Coordinates, node.Coordinates)
      };

      paths.Add(path);
    }
    #endregion
  }
}