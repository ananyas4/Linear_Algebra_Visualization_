using UnityEngine;

public class LinearTransformationVisualizer : MonoBehaviour
{
    public int markerCount = 10;  // Number of markers per axis
    public float axisLength = 5f;  // Length of each axis
    public GameObject targetObject; // The object to be transformed

    private GameObject xAxis, yAxis, zAxis;

    void Start()
    {
        // Create axes
        xAxis = CreateAxis(Vector3.right, Color.red);   // X-axis (Red)
        yAxis = CreateAxis(Vector3.up, Color.green);    // Y-axis (Green)
        zAxis = CreateAxis(Vector3.forward, Color.blue); // Z-axis (Blue)
    }

    void Update()
    {
        // Define a transformation matrix
        Matrix4x4 transformationMatrix = Matrix4x4.identity;

        // Example: Scaling
        transformationMatrix.m00 = 1.5f; // Scale X by 1.5
        transformationMatrix.m11 = 0.5f; // Scale Y by 0.5
        transformationMatrix.m22 = 1.0f; // Scale Z by 1.0

        // Example: Shearing
        transformationMatrix.m01 = 0.5f; // Shear Y based on X
        transformationMatrix.m10 = 0.2f; // Shear X based on Y

        // Apply transformation to the axes
        ApplyTransformationToAxis(xAxis, transformationMatrix);
        ApplyTransformationToAxis(yAxis, transformationMatrix);
        ApplyTransformationToAxis(zAxis, transformationMatrix);

        // Apply transformation to the target object
        if (targetObject != null)
        {
            targetObject.transform.position = transformationMatrix.MultiplyPoint3x4(targetObject.transform.position);
        }
    }

    private GameObject CreateAxis(Vector3 direction, Color color)
    {
        // Create a parent object for the axis
        GameObject axis = new GameObject(direction + " Axis");

        // Create Line Renderer for the axis line
        GameObject axisLine = new GameObject("Axis Line");
        axisLine.transform.parent = axis.transform;
        LineRenderer lineRenderer = axisLine.AddComponent<LineRenderer>();

        // Set up the Line Renderer
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, direction * axisLength);

        // Create markers along the axis
        for (int i = 1; i <= markerCount; i++)
        {
            Vector3 markerPosition = Vector3.Lerp(Vector3.zero, direction * axisLength, i / (float)markerCount);
            CreateMarker(markerPosition, color, axis.transform);
        }

        return axis;
    }

    private void CreateMarker(Vector3 position, Color color, Transform parent)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = position;
        marker.transform.localScale = Vector3.one * 0.1f;
        marker.transform.parent = parent;

        Renderer markerRenderer = marker.GetComponent<Renderer>();
        markerRenderer.material.color = color;
    }

    private void ApplyTransformationToAxis(GameObject axis, Matrix4x4 matrix)
    {
        foreach (Transform child in axis.transform)
        {
            child.position = matrix.MultiplyPoint3x4(child.position);
        }
    }
}
