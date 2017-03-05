
namespace Jabberwocky.UnitySupport
{
  using System;
  using System.Collections.Generic;
  using UnityEngine;

  public static class GraphBaker
  {
    public static PointGraph Bake(List<Vector2> points, Func<Vector2, Vector2, Boolean> canTravelBetweenPoints)
    {
      if (points == null)
      {
        throw new NullReferenceException("Parameter 'points' is null.");
      }

      if (points.Count < 2)
      {
        throw new Exception(String.Format("Must have a minimum of two points to bake graph. Count is {0}.", points.Count));
      }

      if (canTravelBetweenPoints == null)
      {
        throw new NullReferenceException("Parameter 'canTravelBetweenPoints' is null.");
      }

      var verticies = new List<Vector2>(points);
      var distances = new Single[verticies.Count, verticies.Count];

      for (Int32 i = 0; i < verticies.Count; i++)
      {
        distances[i, i] = PointGraph.CannotBeReached;
        for (Int32 j = i + 1; j < verticies.Count; j++)
        {
          var vertex1 = verticies[i];
          var vertex2 = verticies[j];
          var distance = PointGraph.CannotBeReached;
          if (canTravelBetweenPoints(vertex1, vertex2))
          {
            distance = Vector2.Distance(vertex1, vertex2);
          }

          distances[i, j] = distance;
          distances[j, i] = distance;
        }
      }

      return new PointGraph(verticies.ToArray(), distances);
    }
  }
}
