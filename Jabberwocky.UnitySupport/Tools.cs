
namespace Jabberwocky.UnitySupport
{
  using UnityEngine;

  public static class Tools
  {
    public static LineRenderer CreateLineRenderer(Material material, Color color, float width)
    {
      GameObject lineDrawer = new GameObject("Line Renderer Object");
      lineDrawer.AddComponent<LineRenderer>();
      LineRenderer lineRenderer = lineDrawer.GetComponent<LineRenderer>();
      lineRenderer.material = material;
      lineRenderer.startColor = lineRenderer.endColor = color;
      lineRenderer.startWidth = lineRenderer.endWidth = width;
      return lineRenderer;  
    } 
  }
}
