
namespace Jabberwocky.UnitySupport
{
  using System;
  using System.Diagnostics;

  [DebuggerDisplay("Distance to {Node.Name} is {Distance}")]
  public struct Path
  {
    public Single Distance;

    public Node Node;
  }
}