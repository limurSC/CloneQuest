using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WaypointLineRenderer : MonoBehaviour
{
    public LineRenderer Renderer => _lineRenderer;
    [SerializeField] private LineRenderer _lineRenderer;

    public void SetPoints(Vector2 pivot, IEnumerable<Vector2> waypoints)
    {
        var points = waypoints.Select(point => (Vector3)(pivot + point)).ToArray();
        _lineRenderer.positionCount = points.Length;
        _lineRenderer.SetPositions(points);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_lineRenderer == null) { _lineRenderer = GetComponent<LineRenderer>(); }
    }
#endif
}