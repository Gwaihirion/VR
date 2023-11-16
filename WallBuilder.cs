using UnityEngine;
using System.Collections.Generic;

public class WallBuilder : MonoBehaviour
{
    public GameObject wallPrefab; // Assign the wall segment prefab in the inspector
    public float pointSpacing = 1.0f; // Spacing between points on the spline

    private List<Vector3> points = new List<Vector3>(); // Stores points on the spline
    private LineRenderer lineRenderer; // Optional: for visualizing the spline

    void Start()
    {
        // Initialize LineRenderer if you want to visualize the spline
        lineRenderer = gameObject.AddComponent<LineRenderer>();
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) // Replace with appropriate input check
        {
            // Start drawing
            points.Clear();
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger)) // Replace with appropriate input check
        {
            // Finish drawing and instantiate walls
            CreateWallsAlongSpline();
        }

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) // Replace with appropriate input check
        {
            AddPointToSpline();
        }
    }

    void AddPointToSpline()
    {
        Ray ray = new Ray(transform.position, transform.forward); // Replace with actual controller position and direction
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Add point if it's far enough from the last point
            if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], hit.point) > pointSpacing)
            {
                points.Add(hit.point);
                UpdateLineRenderer(); // Optional: Update the visual representation
            }
        }
    }

    void CreateWallsAlongSpline()
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 point = points[i];
            Vector3 nextPoint = points[i + 1];
            Vector3 midPoint = (point + nextPoint) / 2;

            GameObject wallSegment = Instantiate(wallPrefab, midPoint, Quaternion.LookRotation(nextPoint - point));
            wallSegment.transform.up = (nextPoint - point).normalized; // Align with spline
        }
    }

    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
