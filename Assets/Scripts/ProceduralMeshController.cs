using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class ProceduralMeshController : MonoBehaviour
{

    Mesh mesh;

    public Vector3[] vertices;
    public int[] triangles;

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
        //Create an array of vertices
        vertices = new Vector3[] {
            //parte de delante
            new Vector3(0, 2, 0), new Vector3(-1,1,0), new Vector3(0, 1, -1),
            new Vector3(0, 2, 0), new Vector3(1,1,0), new Vector3(0, 1, -1),
            new Vector3(0, 0, 0), new Vector3(-1,1,0), new Vector3(0, 1, -1),
            new Vector3(0, 0, 0), new Vector3(1,1,0), new Vector3(0, 1, -1),
            //parte de detras
            new Vector3(0, 2, 0), new Vector3(-1,1,0), new Vector3(0, 1, 1),
            new Vector3(0, 2, 0), new Vector3(1,1,0), new Vector3(0, 1, 1),
            new Vector3(0, 0, 0), new Vector3(-1,1,0), new Vector3(0, 1, 1),
            new Vector3(0, 0, 0), new Vector3(1,1,0), new Vector3(0, 1, 1),
        };
        //create an array of integers
        triangles = new int[] {
            2, 1, 0,
            3, 4, 5,
            6, 7, 8,
            11, 10, 9,
            12, 13, 14,
            17, 16, 15,
            20, 19, 18,
            21, 22, 23
        };



    }

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
}
