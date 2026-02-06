using MarchingCubes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SpineController : MonoBehaviour
{

    public Transform[] controlPoints;
    public Vector3[] BezierPoints;
    public GameObject[] spines;
    private int SEGMENT_COUNT = 50;
    public int numberSpines = 4;


    public MarchingLogic marching;
    public GameObject prefabSpine;

    public SpineController nextSegment = null;
    public SpineController previousSegment = null;


    public Vector3[] previousPositions;

    private int curveCount = 0;
    private bool redraw= false;
    private GameObject MarchingObject;
    
    


    void Start()
    {
        spines = new GameObject[numberSpines + 1];
        previousPositions = new Vector3[controlPoints.Length];
        for (int i = 0; i < controlPoints.Length; i++)
        {
            previousPositions[i] = controlPoints[i].position;
        }

        MarchingObject = GameObject.Find("MarchingCubes");
        marching = MarchingObject.GetComponent<MarchingLogic>();
        BezierPoints = new Vector3[SEGMENT_COUNT];

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

                previousPositions[i] = controlPoints[i].position;
                
                marching.needsRecalculation = true;
                redraw = true;


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

        for (int i = 1; i <= SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;
            Vector3 position = CalculateCubicBezierPoint(t, controlPoints[0].position, controlPoints[1].position, controlPoints[2].position, controlPoints[3].position);        
            BezierPoints[i-1] = position;

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


            spines = new GameObject[numberSpines+1];
            
            spines[0] = Instantiate(prefabSpine, BezierPoints[0], Quaternion.identity, MarchingObject.transform);
            spines[0].GetComponent<SpineObject>().partOf = transform.gameObject;
            for (int i = 1; i < numberSpines; i++)
            {
                spines[i] = Instantiate(prefabSpine, BezierPoints[i * multi], Quaternion.identity, MarchingObject.transform);
                spines[i].GetComponent<SpineObject>().partOf = transform.gameObject;
            }
            spines[numberSpines] = Instantiate(prefabSpine, BezierPoints[SEGMENT_COUNT - 2], Quaternion.identity, MarchingObject.transform);
            spines[numberSpines].GetComponent<SpineObject>().partOf = transform.gameObject;

            marching.GetChilds();
            marching.needsRecalculation = true;


        } else
        {
            spines[0].transform.position = BezierPoints[0];
            for (int i = 1; i < numberSpines; i++)
            {
                spines[i].transform.position = BezierPoints[(i) * multi];
            }
            spines[numberSpines].transform.position = BezierPoints[SEGMENT_COUNT - 2];
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
