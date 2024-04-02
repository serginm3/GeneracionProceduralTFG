using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class CreatureGeneratorController : MonoBehaviour
{

    Mesh mesh;

    public Vector3[] vertices = new Vector3[] {};
    public int[] triangles = new int[] { };
    public int sizeHead = 1;
    public int horizontalLines = 10;
    public int verticalLines = 10;
    public int radius = 1;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;


    }

    // Start is called before the first frame update
    void Start()
    {
        MakeMeshData();
        CreateMesh();
    }

    // Update is called once per frame
    void MakeMeshData()
    {
        vertices = new Vector3[horizontalLines * verticalLines];

        triangles = new int[horizontalLines * verticalLines];

        int index = 0;
        for (int m = 0; m < horizontalLines; m++)
        {
            for (int n = 0; n < verticalLines - 1; n++)
            {
                float x = Mathf.Sin(Mathf.PI * m / horizontalLines) * Mathf.Cos(2 * Mathf.PI * n / verticalLines);
                float y = Mathf.Sin(Mathf.PI * m / horizontalLines) * Mathf.Sin(2 * Mathf.PI * n / verticalLines);
                float z = Mathf.Cos(Mathf.PI * m / horizontalLines);
                vertices[index++] = new Vector3(x, y, z) * radius;


                

            }
        }
        mesh.vertices = vertices;
        
        int indexTriangle = 0;
        for (int m = 0; m < horizontalLines; m++)
        {
            for (int n = 0; n < verticalLines - 1;)
            {

                triangles[indexTriangle] = n + 1;
                triangles[indexTriangle + 1] = n;
                triangles[indexTriangle + 2] = (n*verticalLines) + 2 ;
                

                n = n + 2;
                indexTriangle = indexTriangle + 3;
            }
        }
        

    }
    //Create an array of vertices




    //create an array of integers





    void Update()
    {
        MakeMeshData();
        CreateMesh();

        
    }

    void CreateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        
    }

    void OnDrawGizmos()
    {
        if (mesh == null)
        {
            return;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.02f);
        }
    }
}
