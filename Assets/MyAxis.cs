using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MyAxis : MonoBehaviour
{
    bool isLinearTransformation;
    double axisLength = 1;
    int initialTallyCt = 10;

    public Material myMaterial;

    Vector3 xTransformation;
    Vector3 yTransformation;
    Vector3 zTransformation;

    Vector3 userVector;
    Vector3 newUserVector;

    GameObject sphere;

    Vector3 iHat = new Vector3(1, 0, 0); //unit vector for x axis
    Vector3 jHat = new Vector3(0, 1, 0); //unit vector for y axis
    Vector3 kHat = new Vector3(0, 0, 1); //unit vector for z axis

    Vector3 newiHat;
    Vector3 newjHat;
    Vector3 newkHat;

    LineRenderer[,] xyLineRenderers;
    LineRenderer[,] xzLineRenderers;
    LineRenderer[,] zyLineRenderers;

    LineRep[,] xyPlaneLines;
    LineRep[,] xzPlaneLines;
    LineRep[,] zyPlaneLines;

    LineRep[,] newxyPlaneLines;
    LineRep[,] newxzPlaneLines;
    LineRep[,] newzyPlaneLines;

    float t = 0;
    float transformationTime = 5;
    Vector3 ogVectors;

    Vector3[] oldxPts = new Vector3[10];
    Vector3[] oldyPts = new Vector3[10];
    Vector3[] oldzPts = new Vector3[10];

    Vector3[] newxPts = new Vector3[10];
    Vector3[] newyPts = new Vector3[10];
    Vector3[] newzPts = new Vector3[10];

    public struct LineRep
    {
        public Vector3[] endpoints { get; set; }
        public Vector3 startPt { get; set; }
        public Vector3 endPt { get; set; }

        public LineRep(Vector3 startPt, Vector3 endPt)
        {
            this.startPt = startPt;
            this.endPt = endPt;
            endpoints = new Vector3[] { startPt, endPt };
        }
    }

    void Start()
    {
        myMaterial = new Material(Shader.Find("Sprites/Default"));

        float step = (float)(axisLength / initialTallyCt);
        userVector = new Vector3(3, 4, 2);
        Vector3 scaledPosition = new Vector3(userVector.x * step, userVector.y * step, userVector.z * step);

        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = scaledPosition;
        sphere.GetComponent<MeshRenderer>().material.color = Color.yellow;
        sphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // Initialize array of original xAxis, yAxis, zAxis positions 
        for (int tally = 0; tally < initialTallyCt; tally++)
        {
            oldxPts[tally] = iHat * ((float)(tally * (axisLength / initialTallyCt)));
            oldyPts[tally] = jHat * ((float)(tally * (axisLength / initialTallyCt)));
            oldzPts[tally] = kHat * ((float)(tally * (axisLength / initialTallyCt)));
        }

        // Initialize arrays of line renderers
        xyLineRenderers = new LineRenderer[initialTallyCt, initialTallyCt];
        xzLineRenderers = new LineRenderer[initialTallyCt, initialTallyCt];
        zyLineRenderers = new LineRenderer[initialTallyCt, initialTallyCt];

        // Create LineRenderers for the grids
        for (int row = 0; row < initialTallyCt; row++)
        {
            for (int columns = 0; columns < initialTallyCt; columns++)
            {
                xyLineRenderers[row, columns] = new GameObject("xyGridLine").AddComponent<LineRenderer>();
                xzLineRenderers[row, columns] = new GameObject("xzGridLine").AddComponent<LineRenderer>();
                zyLineRenderers[row, columns] = new GameObject("zyGridLine").AddComponent<LineRenderer>();
            }
        }

        xyPlaneLines = new LineRep[initialTallyCt, initialTallyCt];
        xzPlaneLines = new LineRep[initialTallyCt, initialTallyCt];
        zyPlaneLines = new LineRep[initialTallyCt, initialTallyCt];

        newxyPlaneLines = new LineRep[initialTallyCt, initialTallyCt];
        newxzPlaneLines = new LineRep[initialTallyCt, initialTallyCt];
        newzyPlaneLines = new LineRep[initialTallyCt, initialTallyCt];

        // Setting initial start and end positions for each line coming out of xy, xz, zy planes
        for (int row = 0; row < initialTallyCt; row++)
        {
            for (int columns = 0; columns < initialTallyCt; columns++)
            {
                //Populating xyPlaneLine start and end positions 
                xyPlaneLines[row, columns] =
                new LineRep(
                    new Vector3(oldxPts[columns].x, (float)(row * (axisLength / initialTallyCt)), 0),
                    new Vector3(oldxPts[columns].x, (float)(row * (axisLength / initialTallyCt)), oldzPts[initialTallyCt - 1].z)
                );

                //Populating xzPlaneLine start and end positions 
                xzPlaneLines[row, columns] =
                new LineRep(
                    new Vector3((float)(row * (axisLength / initialTallyCt)), 0, oldzPts[columns].z),
                    new Vector3((float)(row * (axisLength / initialTallyCt)), oldyPts[initialTallyCt - 1].y, oldzPts[columns].z)
                );

                //Populating zyPlaneLine start and end positions 
                zyPlaneLines[row, columns] =
                new LineRep(
                    new Vector3(0, oldyPts[columns].y, (float)(row * (axisLength / initialTallyCt))),
                    new Vector3(oldxPts[initialTallyCt - 1].x, oldyPts[columns].y, (float)(row * (axisLength / initialTallyCt)))
                );
            }
        }

        // Show the initial grid lines immediately
        for (int row = 0; row < initialTallyCt; row++)
        {
            for (int columns = 0; columns < initialTallyCt; columns++)
            {
                xyLineRenderers[row, columns].SetPositions(xyPlaneLines[row, columns].endpoints);
                xyLineRenderers[row, columns].startColor = Color.blue;
                xyLineRenderers[row, columns].endColor = Color.black;

                xzLineRenderers[row, columns].SetPositions(xzPlaneLines[row, columns].endpoints);
                xzLineRenderers[row, columns].startColor = Color.green;
                xzLineRenderers[row, columns].endColor = Color.black;

                zyLineRenderers[row, columns].SetPositions(zyPlaneLines[row, columns].endpoints);
                zyLineRenderers[row, columns].startColor = Color.red;
                zyLineRenderers[row, columns].endColor = Color.black;

                float width = (float)(axisLength * 0.003f);
                xyLineRenderers[row, columns].startWidth = xzLineRenderers[row, columns].startWidth = zyLineRenderers[row, columns].startWidth = width;
                xyLineRenderers[row, columns].endWidth = xzLineRenderers[row, columns].endWidth = zyLineRenderers[row, columns].endWidth = width;

                xyLineRenderers[row, columns].material = xzLineRenderers[row, columns].material = zyLineRenderers[row, columns].material = myMaterial;
            }
        }
    }
    public void TransformationButtonPressed()
    {
        // First transformation: Rotate about x-axis by 45 degrees
        float sqrt2over2 = Mathf.Sqrt(2f) / 2f;

        xTransformation = new Vector3(1f, 0f, 0f);
        yTransformation = new Vector3(0f, sqrt2over2, sqrt2over2);
        zTransformation = new Vector3(0f, -sqrt2over2, sqrt2over2);

        isLinearTransformation = true;
    }

    public void SecondTransformationButtonPressed()
    {
        // Second transformation: Projection onto xy-plane
        xTransformation = new Vector3(1f, 0f, 0f);
        yTransformation = new Vector3(0f, 1f, 0f);
        zTransformation = new Vector3(0f, 0f, 0f);

        isLinearTransformation = true;
    }

    void Update()
    {
        if (isLinearTransformation)
        {
            // After computing newUserVector
            sphere.transform.position = newUserVector * (float)(axisLength / initialTallyCt);

            //Applies linear transformation on the vector space
            LinearTransformation(
                Vector3.Lerp(iHat, xTransformation, t / transformationTime),
                Vector3.Lerp(jHat, yTransformation, t / transformationTime),
                Vector3.Lerp(kHat, zTransformation, t / transformationTime)
            );

            // Update the grid lines after transformation
            for (int row = 0; row < initialTallyCt; row++)
            {
                for (int columns = 0; columns < initialTallyCt; columns++)
                {
                    xyLineRenderers[row, columns].SetPositions(newxyPlaneLines[row, columns].endpoints);
                    xyLineRenderers[row, columns].startColor = Color.blue;
                    xyLineRenderers[row, columns].endColor = Color.black;

                    xzLineRenderers[row, columns].SetPositions(newxzPlaneLines[row, columns].endpoints);
                    xzLineRenderers[row, columns].startColor = Color.green;
                    xzLineRenderers[row, columns].endColor = Color.black;

                    zyLineRenderers[row, columns].SetPositions(newzyPlaneLines[row, columns].endpoints);
                    zyLineRenderers[row, columns].startColor = Color.red;
                    zyLineRenderers[row, columns].endColor = Color.black;

                    float width = (float)(axisLength * 0.003f);
                    xyLineRenderers[row, columns].startWidth = xzLineRenderers[row, columns].startWidth = zyLineRenderers[row, columns].startWidth = width;
                    xyLineRenderers[row, columns].endWidth = xzLineRenderers[row, columns].endWidth = zyLineRenderers[row, columns].endWidth = width;

                    xyLineRenderers[row, columns].material = xzLineRenderers[row, columns].material = zyLineRenderers[row, columns].material = myMaterial;
                }
            }

            if (t > 10)
            {
                isLinearTransformation = false;
                for (int i = 0; i < initialTallyCt; i++)
                {
                    oldxPts[i] = newxPts[i];
                    oldyPts[i] = newyPts[i];
                    oldzPts[i] = newzPts[i];

                    iHat = newiHat;
                    jHat = newjHat;
                    kHat = newkHat;
                }
                t = 0;
            }

            t += Time.deltaTime;
        }
    }

    void LinearTransformation(Vector3 ihatTransform, Vector3 jhatTransform, Vector3 khatTransform)
    {
        newiHat = ihatTransform;
        newjHat = jhatTransform;
        newkHat = khatTransform;

        //Transform the points on the axes to scale appropriately 
        for (int tally = 0; tally < oldxPts.Length; tally++)
        {
            newxPts[tally] = (oldxPts[tally].x * ihatTransform) + (oldxPts[tally].y * jhatTransform) + (oldxPts[tally].z * khatTransform);
            newyPts[tally] = (oldyPts[tally].x * ihatTransform) + (oldyPts[tally].y * jhatTransform) + (oldyPts[tally].z * khatTransform);
            newzPts[tally] = (oldzPts[tally].x * ihatTransform) + (oldzPts[tally].y * jhatTransform) + (oldzPts[tally].z * khatTransform);
        }

        newUserVector = userVector.x * ihatTransform + userVector.y * jhatTransform + userVector.z * khatTransform;

        // Transforming grid lines
        for (int row = 0; row < initialTallyCt; row++)
        {
            for (int columns = 0; columns < initialTallyCt; columns++)
            {
                newxyPlaneLines[row, columns] =
                    new LineRep(newzPts[0] + newxPts[row] + newyPts[columns],
                                newzPts[initialTallyCt - 1] + newxPts[row] + newyPts[columns]);

                newxzPlaneLines[row, columns] =
                    new LineRep(newzPts[columns] + newxPts[row] + newyPts[0],
                                newzPts[columns] + newxPts[row] + newyPts[initialTallyCt - 1]);

                newzyPlaneLines[row, columns] =
                    new LineRep(newzPts[columns] + newxPts[0] + newyPts[row],
                                newzPts[columns] + newxPts[initialTallyCt - 1] + newyPts[row]);
            }
        }
    }
}
