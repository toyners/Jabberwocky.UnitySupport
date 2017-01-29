
namespace Jabberwocky.UnitySupport
{
  using System;
  using System.Collections.Generic;
  using UnityEngine;

  public struct PointGraph
  {
    #region Fields
    public const Single CannotBeReached = Single.NegativeInfinity;

    public Vector2[] Verticies;

    public Single[,] Distances;
    #endregion

    #region Construction
    public PointGraph(Vector2[] verticies, Single[,] distances)
    {
      this.Distances = distances;
      this.Verticies = verticies;
    }
    #endregion

    public static Boolean PointCannotBeReached(Single distance)
    {
      return float.IsNegativeInfinity(distance);
    }
  }
}
