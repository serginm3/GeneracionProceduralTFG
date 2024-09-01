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

    public SpineController nextSegment = null;
    public SpineController previousSegment = null;

    private int curveCount = 0;
    private int layerOrder = 0;
    private int SEGMENT_COUNT = 50;
    public int numberSpines = 4;
    private bool redraw= false;
    private GameObject MarchingObject;
    
    


    void Start()
    {
        previousPositions = new Vector3[controlPoints.Length];
        for (int i = 0; i < controlPoints.Length; i++)
        {
            previousPositions[i] = controlPoints[i].position;
        }
        MarchingObject = GameObject.Find("MarchingCubes");
        marching = MarchingObject.GetComponent<MarchingLogic>();
        BezierPoints = new Vector3[SEGMENT_COUNT];

        if (!lineRenderer)
        {
            lineRenderer = MarchingObject.GetComponent<LineRenderer>();
        }
        lineRenderer.sortingLayerID = layerOrder;
        curveCount = (int)controlPoints.Length / 3;
        DrawCurve();
        CreateSpines(true);
    }

     public void changePositionOfSpine(bool end,Vector3 position)
    {
        if (end)
        {
            controlPoints[0].position = position;
            Debug.Log("position changed");
        }
        else
        {
            controlPoints[controlPoints.Length - 1].position = position;
        }
        
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
                if (i == 0)
                {
                    if (nextSegment != null)
                    {
                        
                        nextSegment.changePositionOfSpine(false, controlPoints[i].position);
                    }
                } else if (i == controlPoints.Length - 1)
                {
                    if (previousSegment != null)
                    {
                        previousSegment.changePositionOfSpine(true, controlPoints[i].position);
                    }
                }
                break;
            }

        }
        if (marching.t <= 0 && redraw)
        {
            DrawCurve();
            CreateSpines(false);
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
    void CreateSpines(bool isStart)
    {
        int multi = Mathf.FloorToInt(SEGMENT_COUNT / numberSpines);
        if (isStart)
        {
            foreach (GameObject spineObj in spines)
            {
                Destroy(spineObj);

            }
            spines = new GameObject[numberSpines];
            
            spines[0] = Instantiate(prefabSpine, BezierPoints[0], Quaternion.identity, MarchingObject.transform);
            for (int i = 1; i < numberSpines - 1; i++)
            {
                spines[i] = Instantiate(prefabSpine, BezierPoints[(i + 1) * multi], Quaternion.identity, MarchingObject.transform);
            }
            spines[numberSpines - 1] = Instantiate(prefabSpine, BezierPoints[SEGMENT_COUNT - 1], Quaternion.identity, MarchingObject.transform);

            marching.GetChilds();
            marching.needsRecalculation = true;
        } else
        {
            spines[0].transform.position = BezierPoints[0];
            for (int i = 1; i < numberSpines - 1; i++)
            {
                spines[i].transform.position = BezierPoints[(i + 1) * multi];
            }
            spines[numberSpines - 1].transform.position = BezierPoints[SEGMENT_COUNT - 1];
        }
        

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
