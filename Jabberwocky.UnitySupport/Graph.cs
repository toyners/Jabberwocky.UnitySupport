
namespace Jabberwocky.UnitySupport
{
  using UnityEngine;
  using System;
  using System.Collections.Generic;

  public class Graph
  {
    #region Fields
    public List<Node> permanentNodes;

    public List<Node> temporaryNodes;

    private Single distanceCoveringAllPermanentGuidePoints = 0f;

    private HashSet<Node> closedSet = new HashSet<Node>();

    private HashSet<Node> openSet = new HashSet<Node>();

    // For each node, which node it can most efficiently be reached from.
    // If a node can be reached from many nodes, mostEfficientNeighbourNode will eventually contain the
    // most efficient previous step.
    private Dictionary<Node, Node> mostEfficientNeighbourNode = new Dictionary<Node, Node>();

    // For each node, the cost of getting from the start node to that node.
    private Dictionary<Node, Single> distancesFromStartToNode = new Dictionary<Node, Single>();

    // For each node, the total cost of getting from the start node to the goal
    // by passing by that node. That value is partly known, partly heuristic.
    private Dictionary<Node, float> totalCostOfStartToEndViaThisNode = new Dictionary<Node, float>();

    private Func<Vector2, Vector2, Boolean> raycastFunc;
    #endregion

    #region Construction
    public Graph(Int32 nodeCount, Func<Vector2, Vector2, Boolean> raycastFunc)
    {
      this.permanentNodes = new List<Node>(nodeCount);
      this.temporaryNodes = new List<Node>(2);
      this.raycastFunc = raycastFunc;
    }
    #endregion

    #region Methods
    public void Add(Node node, params Node[] neighbourNodes)
    {
      foreach (var neighbourNode in neighbourNodes)
      {
        node.AddPathToNeighbour(neighbourNode);
      }

      if (this.permanentNodes.Count > 0)
      {
        var lastNode = this.permanentNodes[this.permanentNodes.Count - 1];
        this.distanceCoveringAllPermanentGuidePoints += Vector2.Distance(node.Coordinates, lastNode.Coordinates);
      }

      this.permanentNodes.Add(node);
    }

    public List<Vector2> GetPath(Vector2 start, Vector2 end)
    {
      var startNode = this.AddTemporaryNodeToGraph(start);
      startNode.Name = "start";
      var endNode = this.AddTemporaryNodeToGraph(end);
      endNode.Name = "end";

      this.closedSet.Clear();
      this.openSet.Clear();
      this.openSet.Add(startNode);

      // For each node, which node it can most efficiently be reached from.
      // If a node can be reached from many nodes, mostEfficientNeighbourNode will eventually contain the
      // most efficient previous step.
      this.mostEfficientNeighbourNode.Clear();

      // For each node, the cost of getting from the start node to that node.
      this.distancesFromStartToNode.Clear();

      // The cost of going from start to start is zero.
      this.distancesFromStartToNode.Add(startNode, 0f);

      // For each node, the total cost of getting from the start node to the goal
      // by passing by that node. That value is partly known, partly heuristic.
      this.totalCostOfStartToEndViaThisNode.Clear();

      // For the start node, that value is completely heuristic.
      this.totalCostOfStartToEndViaThisNode.Add(startNode, this.distanceCoveringAllPermanentGuidePoints);

      List<Vector2> path = null;
      while (openSet.Count > 0)
      {
        var currentNode = this.GetNodeFromOpenSetWithLowestTotalCost();

        if (currentNode == endNode)
        {
          path = this.ConstructPath(currentNode, startNode);
          break;
        }

        openSet.Remove(currentNode);
        closedSet.Add(currentNode);

        foreach (var pathToNeighbour in currentNode.NeighbourPaths)
        {
          var neighbourNode = pathToNeighbour.Node;
          if (closedSet.Contains(neighbourNode))
          {
            continue;
          }

          var workingDistance = this.distancesFromStartToNode[currentNode] + pathToNeighbour.Distance;

          if (!openSet.Contains(neighbourNode))
          {
            openSet.Add(neighbourNode); // Discovered a new node
          }
          else if (workingDistance >= this.distancesFromStartToNode[neighbourNode])
          {
            continue; // This is not a better path.
          }

          // This path is the best until now. Record it!
          this.mostEfficientNeighbourNode[neighbourNode] = currentNode;
          this.distancesFromStartToNode[neighbourNode] = workingDistance;

          var distanceOfEstimatedPathFromStartToEnd = workingDistance + this.GetHeuristicCostEstimate(neighbourNode, endNode);
          this.totalCostOfStartToEndViaThisNode[neighbourNode] = distanceOfEstimatedPathFromStartToEnd;
        }
      }

      this.ClearTemporaryNodes();

      return path;
    }

    private Node AddTemporaryNodeToGraph(Vector2 point)
    {
      var tempNode = new Node(point, 2);

      for (Int32 index = 0; index < this.permanentNodes.Count; index++)
      {
        var node = this.permanentNodes[index];

        if (this.raycastFunc(point, node.Coordinates))
        {
          // Cannot move from the start point to this guide point so there is no edge between these points
          continue;
        }

        node.AddPathToTemporaryNeighbour(tempNode);
        tempNode.AddPathToTemporaryNeighbour(node);
      }

      if (tempNode.TemporaryPaths.Count == 0)
      {
        throw new Exception("No neighbours for temp guide.");
      }

      this.temporaryNodes.Add(tempNode);
      return tempNode;
    }

    private void ClearTemporaryNodes()
    {
      this.temporaryNodes.Clear();
      for (Int32 index = 0; index < this.permanentNodes.Count; index++)
      {
        this.permanentNodes[index].TemporaryPaths.Clear();
      }
    }

    private List<Vector2> ConstructPath(Node currentNode, Node startNode)
    {
      var path = new List<Vector2>();
      //path.Add(currentNode.Coordinates);

      while (this.mostEfficientNeighbourNode.ContainsKey(currentNode))
      {
        currentNode = this.mostEfficientNeighbourNode[currentNode];
        if (currentNode == startNode)
        {
          break;
        }

        path.Add(currentNode.Coordinates);
      }

      return path;
    }

    private Node GetNodeFromOpenSetWithLowestTotalCost()
    {
      var workingDistance = Single.MaxValue;
      Node lowestNode = null;
      foreach (var node in openSet)
      {
        if (this.totalCostOfStartToEndViaThisNode[node] < workingDistance)
        {
          lowestNode = node;
          workingDistance = this.totalCostOfStartToEndViaThisNode[node];
        }
      }

      return lowestNode;
    }

    private Single GetHeuristicCostEstimate(Node currentNode, Node endNode)
    {
      return this.distanceCoveringAllPermanentGuidePoints;
    }
    #endregion
  }
}