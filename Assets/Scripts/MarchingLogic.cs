using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private float t = 0;

        Mesh mesh;

        bool needsRecalculation= true;

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
                        Gizmos.color = Color.Lerp(Color.black, Color.white, weights[x, y, z]);


                        Gizmos.DrawSphere(new Vector3(0f+x, 0f+y, 0f+z), Mathf.Lerp(0,0.2f, weights[x, y, z]));
                    }
                }
            }

            // Draw a yellow sphere at the transform's position

        }

        private void MarchCube(Vector3 position)
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

                    Debug.Log(weightEdge);
                   

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



        // Update is called once per frame
        void Update()
        {

            for (int i = 0; i < spines.Count; i++)
            {
                if (previousPositions[i] != spines[i].position)
                {
                    
                    //t = 0.1f;
                    needsRecalculation = true;
                    previousPositions[i] = spines[i].position;
                    break;
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
                            float distance = 5 - (new Vector3(x, y, z) - spines[i].position).magnitude; //radio de la esfera
                            weights[x, y, z] = Mathf.Max(Mathf.Clamp(distance/2f,0,1), weights[x, y, z]);
                            
                        }
                    }
                }
            }
        }
        

        void GetChilds()
        {
            int children = transform.childCount;
            for (int i = 0; i < children; ++i)
            {
                spines.Add(transform.GetChild(i));
                previousPositions.Add(transform.GetChild(i).position);
            }
        }

        private void UpdateMesh()
        {
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

    