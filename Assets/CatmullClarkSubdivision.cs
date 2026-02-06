using UnityEngine;
using System.Collections.Generic;

public class CatmullClarkSubdivision : MonoBehaviour
{
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> faces;
    private Dictionary<Edge, Vector3> edgeMidpoints;
    private List<Vector3> facePoints;

    void Start()
    {
        
    }

    public void Subdivide()
    {

        mesh = GetComponent<MeshFilter>().mesh;
        vertices = new List<Vector3>(mesh.vertices);
        faces = new List<int>(mesh.triangles);

        
        edgeMidpoints = new Dictionary<Edge, Vector3>();
        facePoints = new List<Vector3>();

        List<Vector3> newVertices = new List<Vector3>(vertices);
        List<int> newFaces = new List<int>();

        // 1. Compute face points (centroid of each face)
        for (int i = 0; i < faces.Count; i += 3)
        {
            int v1 = faces[i];
            int v2 = faces[i + 1];
            int v3 = faces[i + 2];

            Vector3 facePoint = (vertices[v1] + vertices[v2] + vertices[v3]) / 3.0f;
            facePoints.Add(facePoint);
            newVertices.Add(facePoint);
        }

        // 2. Compute edge midpoints
        for (int i = 0; i < faces.Count; i += 3)
        {
            int v1 = faces[i];
            int v2 = faces[i + 1];
            int v3 = faces[i + 2];

            AddEdgeMidpoint(newVertices, v1, v2);
            AddEdgeMidpoint(newVertices, v2, v3);
            AddEdgeMidpoint(newVertices, v3, v1);
        }

        // 3. Create new faces
        for (int i = 0; i < faces.Count; i += 3)
        {
            int v1 = faces[i];
            int v2 = faces[i + 1];
            int v3 = faces[i + 2];

            Vector3 facePoint = facePoints[i / 3];

            Vector3 e1 = edgeMidpoints[new Edge(v1, v2)];
            Vector3 e2 = edgeMidpoints[new Edge(v2, v3)];
            Vector3 e3 = edgeMidpoints[new Edge(v3, v1)];

            int fpIndex = vertices.Count + i / 3;
            int e1Index = newVertices.IndexOf(e1);
            int e2Index = newVertices.IndexOf(e2);
            int e3Index = newVertices.IndexOf(e3);

            // Create new faces
            newFaces.Add(v1); newFaces.Add(e1Index); newFaces.Add(fpIndex);
            newFaces.Add(e1Index); newFaces.Add(v2); newFaces.Add(fpIndex);
            newFaces.Add(v2); newFaces.Add(e2Index); newFaces.Add(fpIndex);
            newFaces.Add(e2Index); newFaces.Add(v3); newFaces.Add(fpIndex);
            newFaces.Add(v3); newFaces.Add(e3Index); newFaces.Add(fpIndex);
            newFaces.Add(e3Index); newFaces.Add(v1); newFaces.Add(fpIndex);
        }

        // Update the mesh with the new vertices and faces
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newFaces.ToArray();
        mesh.RecalculateNormals();
    }

    void AddEdgeMidpoint(List<Vector3> newVertices, int v1, int v2)
    {
        Edge edge = new Edge(v1, v2);
        if (!edgeMidpoints.ContainsKey(edge))
        {
            Vector3 midpoint = (vertices[v1] + vertices[v2]) / 2.0f;
            edgeMidpoints[edge] = midpoint;
            newVertices.Add(midpoint);
        }
    }
}

struct Edge
{
    public int v1, v2;

    public Edge(int vertex1, int vertex2)
    {
        v1 = Mathf.Min(vertex1, vertex2);
        v2 = Mathf.Max(vertex1, vertex2);
    }

    public override int GetHashCode()
    {
        return v1.GetHashCode() ^ v2.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Edge)) return false;
        Edge other = (Edge)obj;
        return (v1 == other.v1 && v2 == other.v2);
    }
}