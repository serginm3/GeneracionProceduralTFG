using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;


namespace MarchingCubes {
    [RequireComponent(typeof(MeshFilter))]
    public class MarchingLogic : MonoBehaviour
    {
        public int surfaceArea = 10;
        public int surfaceHeight = 10;
        public int surfacewidth = 10;
        public int MarchingArea = 1;
        public float offSet = 0.5f;

        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Transform> spines;

        List<Vector3> previousPositions = new List<Vector3>();

        public float[,,] weights;

        public float t = 0;

        Mesh mesh;

        public bool needsRecalculation= true;

        LineRenderer lr;
        // Start is called before the first frame update

        void Awake()
        {
            GetChilds();
            SetWeight();
            mesh = GetComponent<MeshFilter>().mesh;

            //MakeMeshData();
            CreateMesh();

            


            

            //MarchCube(new Vector3());

            Debug.Log(MarchingTable.Triangles.Length);
            //Debug.Log(edges.ToString());
        }
        void Start()
        {
            for (int x = 0; x < surfaceArea; x++) //the line the error is pointing to
            {
                for (int y = 0; y < surfaceHeight; y++) //the line the error is pointing to
                {
                    for (int z = 0; z < surfacewidth; z++) //the line the error is pointing to
                    {
                        
                        MarchCube(new Vector3(x,y,z));

                    }
                }
            }
            Debug.DrawLine(new Vector3(0, 0, 0), new Vector3(0, 5, 0));

        }

        void OnDrawGizmosSelected()
        {
            for (int x = 0; x < surfaceArea; x++) //the line the error is pointing to
            {
                for (int y = 0; y < surfaceHeight; y++) //the line the error is pointing to
                {
                    for (int z = 0; z < surfacewidth; z++) //the line the error is pointing to
                    {
                        //enemies*.active = true;	*
                        /*
                        if (weights[x, y, z] >= 1)
                        {
                            Gizmos.color = Color.yellow;
                        }
                        else
                        {
                            Gizmos.color = Color.red;
                        }
                        */
                        Gizmos.color = UnityEngine.Color.Lerp(UnityEngine.Color.black, UnityEngine.Color.white, weights[x, y, z]);


                        Gizmos.DrawSphere(new Vector3(0f+x, 0f+y, 0f+z), Mathf.Lerp(0.01f,0.2f, weights[x, y, z]));
                    }
                }
            }

            // Draw a yellow sphere at the transform's position

        }

        public void MarchCube(Vector3 position)
        {
            //GetChilds();
            int x = (int) position.x;
            int y = (int) position.y;
            int z = (int) position.z;

            int cubeIndex = 0;
            if (weights[0 + x,0 + y,0 + z] < 1) cubeIndex |= 1;
            if (weights[1 + x, 0 + y, 0 + z] < 1) cubeIndex |= 2;
            if (weights[1 + x, 1 + y, 0 + z] < 1) cubeIndex |= 4;
            if (weights[0 + x, 1 + y, 0 + z] < 1) cubeIndex |= 8;
            if (weights[0 + x, 0 + y, 1 + z] < 1) cubeIndex |= 16;
            if (weights[1 + x, 0 + y, 1 + z] < 1) cubeIndex |= 32;
            if (weights[1 + x, 1 + y, 1 + z] < 1) cubeIndex |= 64;
            if (weights[0 + x, 1 + y, 1 + z] < 1) cubeIndex |= 128;

            //int[] edges = MarchingTable.Triangles[cubeIndex];

            if (cubeIndex == 0 || cubeIndex == 255)
            {
                return;
            }

            int edgeIndex = 0;
            for (int t = 0; t < 5; t++)
            {
                for (int v = 0; v < 3; v++)
                {
                    int triTableValue = MarchingTable.Triangles[cubeIndex, edgeIndex];

                    if (triTableValue == -1)
                    {
                        return;
                    }

                    Vector3 edgeStart = position + MarchingTable.Edges[triTableValue, 0];
                    Vector3 edgeEnd = position + MarchingTable.Edges[triTableValue, 1];

                    float weightEdge = weights[(int)edgeStart.x, (int)edgeStart.y, (int)edgeStart.z] - weights[(int)edgeEnd.x, (int)edgeEnd.y, (int)edgeEnd.z];


                    

                    if (weightEdge < 0)
                    {
                        weightEdge = 1 + weightEdge;
                    }

                    

                    //Vector3 vertex = (edgeStart + edgeEnd) / 2;
                    Vector3 vertex = Vector3.Lerp(edgeEnd, edgeStart, weightEdge);
                    

                    vertices.Add(vertex);
                    triangles.Add(vertices.Count - 1);

                    edgeIndex++;
                }
            }
        }

        public void needsUpdate()
        {
            needsRecalculation = true;
        }

        // Update is called once per frame
        void Update()
        {

            for (int i = 0; i < spines.Count; i++)
            {
                if (spines[i] != null)
                {
                    if (previousPositions[i] != spines[i].position)
                    {

                        //t = 0.1f;
                        needsRecalculation = true;
                        previousPositions[i] = spines[i].position;
                        break;
                    }
                } else
                {
                    spines.RemoveAt(i);
                }
            
            }

            if (t <= 0 && needsRecalculation)
            {

                needsRecalculation = false;
                t = 0.2f;
                UpdateMesh();
            }
            if (t > 0)
            {
                t -= Time.deltaTime;
            }
           
        }
        void CreateMesh()
        {
            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();


        }
        void SetWeight()
        {
            weights = new float[surfaceArea + 1, surfaceHeight + 1, surfacewidth + 1];
            
                for (int x = 0; x < surfaceArea; x++) //the line the error is pointing to
                {
                    for (int y = 0; y < surfaceHeight; y++) //the line the error is pointing to
                    {
                        for (int z = 0; z < surfacewidth; z++) //the line the error is pointing to
                        {
                        
                        for (int i = 0; i < spines.Count; i++) {

                            Vector3 point = new Vector3(x - spines[i].position.x, y - spines[i].position.y, z - spines[i].position.z);

                            

                            



                            if (true){

                                Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(spines[i].GetComponent<SpineObject>().rotationX, spines[i].GetComponent<SpineObject>().rotationY, spines[i].GetComponent<SpineObject>().rotationZ)).inverse;
                                Vector3 point2 = rotationMatrix.MultiplyPoint3x4(point);

                                
                                Vector3 ScaledVector = new Vector3(point2.x / spines[i].GetComponent<SpineObject>().radiousX, point2.y / spines[i].GetComponent<SpineObject>().radiousY, point2.z / spines[i].GetComponent<SpineObject>().radiousZ);

                                

                                float distance = spines[i].GetComponent<SpineObject>().scale - (ScaledVector).magnitude; //radio de la esfera
                                weights[x, y, z] = Mathf.Max(Mathf.Clamp(distance / 2f, 0, 1), weights[x, y, z]);
                            }else
                            {
                                float scale = spines[i].GetComponent<SpineObject>().scale;
                                float a = spines[i].GetComponent<SpineObject>().radiousX * scale;
                                float b = spines[i].GetComponent<SpineObject>().radiousY * scale;
                                float c = spines[i].GetComponent<SpineObject>().radiousZ * scale;

                                Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(spines[i].GetComponent<SpineObject>().rotationX, spines[i].GetComponent<SpineObject>().rotationY, spines[i].GetComponent<SpineObject>().rotationZ)).inverse;
                                Vector3 point2 = rotationMatrix.MultiplyPoint3x4(point);


                                float x2 = (point2.x * point2.x) / (a * a);
                                float y2 = (point2.y * point2.y) / (b * b);
                                float z2 = (point2.z * point2.z) / (c * c);
                                //Debug.Log(new Vector3(x2, y2, z2));
                                float distance = -((x2 + y2 + z2) - 2.0f);



                                if (distance > 0)
                                {
                                    if (x >= 0 && x < weights.GetLength(0) && y >= 0 && y < weights.GetLength(1) && z >= 0 && z < weights.GetLength(2))
                                    {
                                        weights[x, y, z] = Mathf.Max(Mathf.Clamp(distance, 0, 1), weights[x, y, z]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void GetChilds()
        {
            spines.Clear();
            previousPositions.Clear();
            int children = transform.childCount;
            for (int i = 0; i < children; ++i)
            {
                spines.Add(transform.GetChild(i));
                previousPositions.Add(transform.GetChild(i).position);
            }
        }

        public void UpdateMesh()
        {
            //Debug.Log("Updating mesh");
            SetWeight();
            vertices = new List<Vector3>();
            triangles = new List<int>();
            //MakeMeshData();
            for (int x = 0; x < surfaceArea; x++) //the line the error is pointing to
            {
                for (int y = 0; y < surfaceHeight; y++) //the line the error is pointing to
                {
                    for (int z = 0; z < surfacewidth; z++) //the line the error is pointing to
                    {

                        MarchCube(new Vector3(x, y, z));

                    }
                }
            }


            CreateMesh();
        }
    }
}

    