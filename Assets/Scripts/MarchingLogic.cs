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
        public int surfaceLength = 40;
        public int surfaceHeight = 40;
        public int surfaceWidth = 40;
        public float[,,] weights;

        public List<Vector3> vertices;
        public List<int> triangles;

        Mesh mesh;
        MeshCollider meshCollider;

        public List<Transform> spines;

        List<Vector3> previousPositions = new List<Vector3>();
        Dictionary<Vector3, int> weldMap = new Dictionary<Vector3, int>();

        MeshFilter meshFilter;

        public float t = 0;

        public CatmullClarkSubdivision subdivision;

        public bool needsRecalculation= true;

        Vector3 point;
        Matrix4x4 rotationMatrix;
        Vector3 point2;
        Vector3 ScaledVector;
        float distance;

        void Awake()
        {
            

            //Creamos la mesh a partir de las weights asignadas antes
            //CreateMesh();

            Debug.Log(MarchingTable.Triangles.Length);
        }
        void Start()
        {
            //Guardamos el meshFilter en una variable
            mesh = new Mesh();
            

            //Guardamos el componente meshCollider en una variable
            meshCollider = gameObject.AddComponent<MeshCollider>();
            //Conseguimos todos los hijos del objeto
            GetChilds();
            //Asignamos los valores al espacio de puntos de Marching cubes
            SetWeight();
            //Asignamos la mesh procedural al collider
            meshCollider.sharedMesh = mesh;
            //Iteramos todos los puntos dentro del espacio de marching cubes
            for (int x = 0; x < surfaceLength; x++) 
            {
                for (int y = 0; y < surfaceHeight; y++) 
                {
                    for (int z = 0; z < surfaceWidth; z++)
                    {
                        //Hacemos un paso del cubo para generar la mesh
                        MarchCube(new Vector3(x,y,z));

                    }
                }
            }

        }

        void OnDrawGizmosSelected()
        {
            for (int x = 0; x < surfaceLength; x++) //the line the error is pointing to
            {
                for (int y = 0; y < surfaceHeight; y++) //the line the error is pointing to
                {
                    for (int z = 0; z < surfaceWidth; z++) //the line the error is pointing to
                    {
                        Gizmos.color = UnityEngine.Color.Lerp(UnityEngine.Color.black, UnityEngine.Color.white, weights[x, y, z]);
                        Gizmos.DrawSphere(new Vector3(0f+x, 0f+y, 0f+z), Mathf.Lerp(0.01f,0.2f, weights[x, y, z]));
                    }
                }
            }

        }

        public void MarchCube(Vector3 position)
        {
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


                    Vector3 vertex = Vector3.Lerp(edgeEnd, edgeStart, weightEdge);
                    //Vector3 vertex = (edgeStart + edgeEnd) / 2;
                    int index;
                    
                    if (weldMap.TryGetValue(edgeEnd + edgeStart, out index))
                    {
                        
                        triangles.Add(index);
                    }
                    else
                    {
                        index = vertices.Count;
                        weldMap.Add(edgeEnd + edgeStart, index);
                        vertices.Add(vertex);
                        triangles.Add(index);
                        
                    }

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
                t = 0.1f;
                UpdateMesh();
            }

            if (t > 0)
            {
                t -= Time.deltaTime;
            }
           
        }
        void CreateMesh()
        {
            if(meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }
            
            //mesh.Clear();
            mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;
        }

        void SetWeight()
        {
            //Creamos una array con las dimensiones asignadas al principio
            weights = new float[surfaceLength + 1, surfaceHeight + 1, surfaceWidth + 1];
            
            SpineObject spineComponent = null;
            //Iteramos cada punto dentro de weights por cada objeto hijo
            for (int x = 0; x < surfaceLength; x++)
            {
                for (int y = 0; y < surfaceHeight; y++)
                {
                    for (int z = 0; z < surfaceWidth; z++)
                    {
                        
                    for (int i = 0; i < spines.Count; i++) {

                        spineComponent = spines[i].GetComponent<SpineObject>();

                        point = new Vector3(x - spines[i].position.x, y - spines[i].position.y, z - spines[i].position.z);

                        if (spineComponent.rotationX != 0 || spineComponent.rotationY != 0 || spineComponent.rotationZ != 0)
                        {

                            rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(spineComponent.rotationX, spineComponent.rotationY, spineComponent.rotationZ)).inverse;
                            point2 = rotationMatrix.MultiplyPoint3x4(point);

                        } else
                        {
                            point2 = point;
                        }

                        ScaledVector = new Vector3(point2.x / spineComponent.radiousX, point2.y / spineComponent.radiousY, point2.z / spineComponent.radiousZ);

                        float radius = spineComponent.scale;
                        float distanceToSurface = Mathf.Abs((ScaledVector).magnitude - radius);

                        float weight = Mathf.Clamp01(spineComponent.scale - distanceToSurface);

                        weights[x, y, z] = Mathf.Max(weight, weights[x, y, z]);

                        
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
            weldMap = new Dictionary<Vector3, int>();
            vertices = new List<Vector3>();
            triangles = new List<int>();

            SetWeight();

            for (int x = 0; x < surfaceLength; x++) //the line the error is pointing to
            {
                for (int y = 0; y < surfaceHeight; y++) //the line the error is pointing to
                {
                    for (int z = 0; z < surfaceWidth; z++) //the line the error is pointing to
                    {

                        MarchCube(new Vector3(x, y, z));

                    }
                }
            }

            CreateMesh();
        }
        void calculateNormalsManaged(Vector3[] verts, Vector3[] normals, int[] tris)
        {
            for (int i = 0; i < tris.Length; i += 3)
            {
                int tri0 = tris[i];
                int tri1 = tris[i + 1];
                int tri2 = tris[i + 2];
                Vector3 vert0 = verts[tri0];
                Vector3 vert1 = verts[tri1];
                Vector3 vert2 = verts[tri2];
                // Vector3 normal = Vector3.Cross(vert1 - vert0, vert2 - vert0);
                Vector3 normal = new Vector3()
                {
                    x = vert0.y * vert1.z - vert0.y * vert2.z - vert1.y * vert0.z + vert1.y * vert2.z + vert2.y * vert0.z - vert2.y * vert1.z,
                    y = -vert0.x * vert1.z + vert0.x * vert2.z + vert1.x * vert0.z - vert1.x * vert2.z - vert2.x * vert0.z + vert2.x * vert1.z,
                    z = vert0.x * vert1.y - vert0.x * vert2.y - vert1.x * vert0.y + vert1.x * vert2.y + vert2.x * vert0.y - vert2.x * vert1.y
                };
                normals[tri0] += normal;
                normals[tri1] += normal;
                normals[tri2] += normal;
            }

            for (int i = 0; i < normals.Length; i++)
            {
                // normals [i] = Vector3.Normalize (normals [i]);
                Vector3 norm = normals[i];
                float invlength = 1.0f / (float)System.Math.Sqrt(norm.x * norm.x + norm.y * norm.y + norm.z * norm.z);
                normals[i].x = norm.x * invlength;
                normals[i].y = norm.y * invlength;
                normals[i].z = norm.z * invlength;
            }
        }

    }
    
}

    