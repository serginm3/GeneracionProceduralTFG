using MarchingCubes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SpineController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform[] controlPoints;
    public Vector3[] previousPositions;
    public LineRenderer lineRenderer;
    public Vector3[] BezierPoints;
    public GameObject prefabSpine;
    public GameObject[] spines;
    public MarchingLogic marching;

    private int curveCount = 0;
    private int layerOrder = 0;
    private int SEGMENT_COUNT = 50;
    public int numberSpines = 4;
    private bool redraw= false;
    
    


    void Start()
    {
        previousPositions = new Vector3[controlPoints.Length];
        for (int i = 0; i < controlPoints.Length; i++)
        {
            previousPositions[i] = controlPoints[i].position;
        }
        marching = GetComponent<MarchingLogic>();
        BezierPoints = new Vector3[SEGMENT_COUNT];

        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.sortingLayerID = layerOrder;
        curveCount = (int)controlPoints.Length / 3;
        DrawCurve();
        CreateSpines();
    }

    void Update()
    {

        for (int i = 0; i < controlPoints.Length; i++)
        {
            if (previousPositions[i] != controlPoints[i].position)
            {

                //t = 0.1f;
                
                previousPositions[i] = controlPoints[i].position;
                
                marching.needsRecalculation = true;
                redraw = true;
                //marching.GetChilds();
                break;
            }

        }
        if (marching.t <= 0 && redraw)
        {
            DrawCurve();
            CreateSpines();
            redraw = false;
        }
            

        //CreateSpines();
    }

    void DrawCurve()
    {
        for (int j = 0; j < curveCount; j++)
        {
            for (int i = 1; i <= SEGMENT_COUNT; i++)
            {
                float t = i / (float)SEGMENT_COUNT;
                int nodeIndex = j * 3;
                Vector3 pixel = CalculateCubicBezierPoint(t, controlPoints[nodeIndex].position, controlPoints[nodeIndex + 1].position, controlPoints[nodeIndex + 2].position, controlPoints[nodeIndex + 3].position);
                
                BezierPoints[i-1] = pixel;
                lineRenderer.SetVertexCount(((j * SEGMENT_COUNT) + i));
                lineRenderer.SetPosition((j * SEGMENT_COUNT) + (i - 1), pixel);
            }

        }
    }
    void CreateSpines()
    {
        foreach (GameObject spineObj in spines)
        {
            Destroy(spineObj);
            
        }
        spines = new GameObject[numberSpines];
        int multi = Mathf.FloorToInt(SEGMENT_COUNT / numberSpines);
        spines[0] = Instantiate(prefabSpine, BezierPoints[0], Quaternion.identity,transform);
        for (int i = 1; i < numberSpines-1; i++)
        {
            spines[i] = Instantiate(prefabSpine, BezierPoints[(i+1)*multi], Quaternion.identity, transform);
        }
        spines[numberSpines-1] = Instantiate(prefabSpine, BezierPoints[SEGMENT_COUNT-1], Quaternion.identity, transform);

        marching.GetChilds();
        marching.needsRecalculation = true;

    }

        Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
}
