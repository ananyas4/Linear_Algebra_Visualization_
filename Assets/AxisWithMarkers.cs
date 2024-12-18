using UnityEngine;
using System.Collections;

public class AxisWithMarkers : MonoBehaviour
{
    public int markerCount = 10;  // Number of markers per axis
    public float axisLength = 5f;  // Length of each axis
    public float transformationInterval = 5f; // Time between transformations
    public float transitionDuration = 2f; // Duration of the transformation

    private GameObject xAxis, yAxis, zAxis;
    private Vector3[] originalPositionsX, originalPositionsY, originalPositionsZ;
    private bool isTransformed = false;

    private void Start()
    {
        // Create axes
        xAxis = CreateAxis(Vector3.right, Color.red);   // X-axis (Red)
        yAxis = CreateAxis(Vector3.up, Color.green);    // Y-axis (Green)
        zAxis = CreateAxis(Vector3.forward, Color.blue); // Z-axis (Blue)

        // Store original positions of markers
        originalPositionsX = StoreOriginalPositions(xAxis);
        originalPositionsY = StoreOriginalPositions(yAxis);
        originalPositionsZ = StoreOriginalPositions(zAxis);

        // Start the transformation cycle
        StartCoroutine(TransformationCycle());
    }

    private GameObject CreateAxis(Vector3 direction, Color color)
    {
        // Create a parent object for the axis
        GameObject axisParent = new GameObject(direction + " Axis");

        // Create Line Renderer for the axis
        GameObject axisLine = new GameObject("Axis Line");
        axisLine.transform.parent = axisParent.transform;
        LineRenderer lineRenderer = axisLine.AddComponent<LineRenderer>();

        // Set up the Line Renderer
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // Set positions for the axis line
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Vector3.zero);           // Start at origin
        lineRenderer.SetPosition(1, direction * axisLength); // Extend in the specified direction

        // Create markers along the axis
        for (int i = 1; i <= markerCount; i++)
        {
            Vector3 markerPosition = Vector3.Lerp(Vector3.zero, direction * axisLength, i / (float)markerCount);
            CreateMarker(markerPosition, color, axisParent.transform);
        }

        return axisParent;
    }

    private void CreateMarker(Vector3 position, Color color, Transform parent)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere); // Create a sphere as a marker
        marker.transform.position = position; // Set its position
        marker.transform.localScale = Vector3.one * 0.1f; // Scale it down for visibility
        marker.transform.parent = parent; // Set parent to keep hierarchy clean

        // Change the marker color
        Renderer markerRenderer = marker.GetComponent<Renderer>();
        markerRenderer.material.color = color;
    }

    private Vector3[] StoreOriginalPositions(GameObject axis)
    {
        Transform[] markers = axis.GetComponentsInChildren<Transform>();
        Vector3[] positions = new Vector3[markers.Length - 1]; // Exclude the parent itself
        for (int i = 1; i < markers.Length; i++)
        {
            positions[i - 1] = markers[i].position;
        }
        return positions;
    }

    private IEnumerator TransformationCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(transformationInterval);
            if (isTransformed)
            {
                // Revert to original positions
                StartCoroutine(AnimateTransformation(xAxis, originalPositionsX, transitionDuration));
                StartCoroutine(AnimateTransformation(yAxis, originalPositionsY, transitionDuration));
                StartCoroutine(AnimateTransformation(zAxis, originalPositionsZ, transitionDuration));
            }
            else
            {
                // Apply transformation
                Matrix4x4 transformationMatrix = Matrix4x4.Scale(new Vector3(2, 0.5f, 1)); // Example transformation
                StartCoroutine(AnimateTransformation(xAxis, TransformPositions(originalPositionsX, transformationMatrix), transitionDuration));
                StartCoroutine(AnimateTransformation(yAxis, TransformPositions(originalPositionsY, transformationMatrix), transitionDuration));
                StartCoroutine(AnimateTransformation(zAxis, TransformPositions(originalPositionsZ, transformationMatrix), transitionDuration));
            }
            isTransformed = !isTransformed;
        }
    }

    private Vector3[] TransformPositions(Vector3[] originalPositions, Matrix4x4 matrix)
    {
        Vector3[] transformedPositions = new Vector3[originalPositions.Length];
        for (int i = 0; i < originalPositions.Length; i++)
        {
            transformedPositions[i] = matrix.MultiplyPoint3x4(originalPositions[i]);
        }
        return transformedPositions;
    }

    private IEnumerator AnimateTransformation(GameObject axis, Vector3[] targetPositions, float duration)
    {
        Transform[] markers = axis.GetComponentsInChildren<Transform>();
        Vector3[] startPositions = new Vector3[markers.Length - 1];
        for (int i = 1; i < markers.Length; i++)
        {
            startPositions[i - 1] = markers[i].position;
        }

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            for (int i = 1; i < markers.Length; i++)
            {
                markers[i].position = Vector3.Lerp(startPositions[i - 1], targetPositions[i - 1], elapsedTime / duration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final positions are set
        for (int i = 1; i < markers.Length; i++)
        {
            markers[i].position = targetPositions[i - 1];
        }
    }
}
