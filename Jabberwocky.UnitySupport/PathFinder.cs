
namespace Jabberwocky.UnitySupport
{
  using System;
  using System.Collections.Generic;
  using UnityEngine;

  public class PathFinder
  {
    #region Fields
    private Vector2[] verticies;

    private Single[,] distances;

    private Single distanceCoveringAllPermanentPoints;

    private HashSet<Int32> closedSet = new HashSet<Int32>();

    private HashSet<Int32> openSet = new HashSet<Int32>();

    // For each point, which point it can most efficiently be reached from.
    // If a point can be reached from many points, mostEfficientNeighbour will eventually contain the
    // most efficient previous step.
    private Dictionary<Int32, Int32> mostEfficientNeighbour = new Dictionary<Int32, Int32>();

    // For each point, the cost of getting from the start point to that point.
    private Dictionary<Int32, Single> distancesFromStartToPoint = new Dictionary<Int32, Single>();

    // For each point, the total cost of getting from the start point to the goal
    // by passing by that point. That value is partly known, partly heuristic.
    private Dictionary<Int32, Single> totalCostOfStartToGoalViaThisPoint = new Dictionary<Int32, Single>();
    private Func<Vector2, Vector2, Boolean> canTravelBetweenPoints;
    #endregion

    #region Construction
    public PathFinder(PointGraph graph, Func<Vector2, Vector2, Boolean> canTravelBetweenPoints)
    {
      this.canTravelBetweenPoints = canTravelBetweenPoints;

      this.distanceCoveringAllPermanentPoints = this.CalculateDistanceCoveringAllVerticies(graph.Distances);

      this.verticies = this.CreateVerticiesArray(graph.Verticies);

      this.distances = this.CreateDistanceMatrix(graph.Distances);
    }
    #endregion

    #region Methods
    public void CalculatePath(Vector2 start, Vector2 end, LinkedList<Vector2> points)
    {
      if (this.canTravelBetweenPoints(start, end))
      {
        // Nothing blocking the straight route to the end.
        points.AddLast(end);
        return;
      }

      // Path is blocked - build the path around the blockage.
      this.AddTemporaryPoints(start, end);
      this.CalculatePath(points);
    }

    private void AddTemporaryPoints(Vector2 start, Vector2 end)
    {
      this.verticies[this.verticies.Length - 2] = start;
      this.verticies[this.verticies.Length - 1] = end;

      var length = this.distances.GetUpperBound(0) + 1;
      for (Int32 i = length - 2; i < length; i++)
      {
        for (Int32 j = 0; j < length - 2; j++)
        {
          var distance = PointGraph.CannotBeReached;
          if (this.canTravelBetweenPoints(this.verticies[i], this.verticies[j]))
          {
            distance = Vector2.Distance(this.verticies[i], this.verticies[j]);
          }
          
          this.distances[i, j] = distance;
          this.distances[j, i] = distance;
        }
      }

      int x = length - 2;
      int y = length - 1;
      this.distances[x, x] = Single.NegativeInfinity;
      this.distances[x, y] = Single.NegativeInfinity;
      this.distances[y, x] = Single.NegativeInfinity;
      this.distances[y, y] = Single.NegativeInfinity;
    }

    private Single CalculateDistanceCoveringAllVerticies(Single[,] graphDistances)
    {
      var distanceCoveringAllVerticies = 0f;
      var length = graphDistances.GetUpperBound(0);
      for (Int32 i = 0; i <= length; i++)
      {
        var distance = graphDistances[0, i];
        if (Single.IsNegativeInfinity(distance))
        {
          continue;
        }

        distanceCoveringAllVerticies += distance;
      }

      return distanceCoveringAllVerticies;
    }

    private void CalculatePath(LinkedList<Vector2> points)
    {
      var startIndex = this.verticies.Length - 2;
      var endIndex = this.verticies.Length - 1;
      this.closedSet.Clear();
      this.openSet.Clear();
      this.openSet.Add(startIndex); // Index of first node

      this.mostEfficientNeighbour.Clear();

      // The cost of going from start to start is zero.
      this.distancesFromStartToPoint.Clear();
      this.distancesFromStartToPoint.Add(startIndex, 0f);

      // For the start point, that value is completely heuristic.
      this.totalCostOfStartToGoalViaThisPoint.Clear();
      this.totalCostOfStartToGoalViaThisPoint.Add(startIndex, this.distanceCoveringAllPermanentPoints);

      var path = new List<Vector2>();
      while (openSet.Count > 0)
      {
        var currentIndex = this.GetIndexFromOpenSetWithLowestTotalCost();
        if (currentIndex == endIndex)
        {
          this.ConstructPath(currentIndex, startIndex, points);
          return;
        }

        openSet.Remove(currentIndex);
        closedSet.Add(currentIndex);

        for (Int32 index = 0; index < this.verticies.Length; index++)
        {
          if (currentIndex == index)
          {
            continue;
          }

          if (this.closedSet.Contains(index))
          {
            continue;
          }

          var distance = this.distances[currentIndex, index];
          if (Single.IsNegativeInfinity(distance))
          {
            continue;
          }

          var workingDistance = this.distancesFromStartToPoint[currentIndex] + distance;

          if (!openSet.Contains(index))
          {
            openSet.Add(index); // Discovered a new point
          }
          else if (workingDistance >= this.distancesFromStartToPoint[index])
          {
            continue; // This is not a better path.
          }

          // This path is the best until now. Record it!
          this.mostEfficientNeighbour[index] = currentIndex;
          this.distancesFromStartToPoint[index] = workingDistance;

          var distanceOfEstimatedPathFromStartToEnd = workingDistance + this.GetHeuristicCostEstimate(index, endIndex);
          this.totalCostOfStartToGoalViaThisPoint[index] = distanceOfEstimatedPathFromStartToEnd;
        }
      }

      throw new Exception("Exception during path calculation. No path calculated.");
    }

    private void ConstructPath(Int32 currentIndex, Int32 startIndex, LinkedList<Vector2> points)
    {
      var marker = points.AddLast(this.verticies[currentIndex]);

      while (this.mostEfficientNeighbour.ContainsKey(currentIndex))
      {
        currentIndex = this.mostEfficientNeighbour[currentIndex];
        if (currentIndex == startIndex)
        {
          break;
        }

        marker = points.AddBefore(marker, this.verticies[currentIndex]);
      }
    }

    private Single[,] CreateDistanceMatrix(Single[,] graphDistances)
    {
      var length = graphDistances.GetUpperBound(0) + 3;
      var gd = new Single[length, length];

      for (Int32 i = 0; i <= graphDistances.GetUpperBound(0); i++)
      {
        for (Int32 j = 0; j <= graphDistances.GetUpperBound(1); j++)
        {
          gd[i, j] = graphDistances[i, j];
        }
      }

      return gd;
    }

    private Vector2[] CreateVerticiesArray(Vector2[] graphVertices)
    {
      var v = new Vector2[graphVertices.Length + 2];
      Array.Copy(graphVertices, v, graphVertices.Length);
      return v;
    }

    private Single GetHeuristicCostEstimate(Int32 index, Int32 endIndex)
    {
      return this.distanceCoveringAllPermanentPoints;
    }

    private Int32 GetIndexFromOpenSetWithLowestTotalCost()
    {
      var workingDistance = Single.MaxValue;
      Int32 lowestIndex = -1;
      foreach (var index in openSet)
      {
        if (this.totalCostOfStartToGoalViaThisPoint[index] < workingDistance)
        {
          lowestIndex = index;
          workingDistance = this.totalCostOfStartToGoalViaThisPoint[index];
        }
      }

      return lowestIndex;
    }
    #endregion
  }
}
