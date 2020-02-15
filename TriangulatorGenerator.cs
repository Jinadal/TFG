using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangulatorGenerator : MonoBehaviour
{
    public void GenerateMesh(WingedEdge w)
    {
        List<Vector2> vertices2D = new List<Vector2>();
        for (int i = 0; i < w.Vertices.Count; i++)
        {
            for (int j = 0; j < w.Vertices[i].Edges.Count; j++)
            {
                if ( w.Vertices[i].Edges[j].LeftFace != null)
                {
                    float x = w.Vertices[i].Edges[j].LeftFace.FaceCircumcenter.x;
                    float y = w.Vertices[i].Edges[j].LeftFace.FaceCircumcenter.z;
                    vertices2D.Add(new Vector2(x, y));
                }
            }
            Triangulate(vertices2D.ToArray());
            vertices2D.Clear();
        }
        
        // Use the triangulator to get indices for creating triangles
    }
    private void Triangulate(Vector2[] vertices2D)
    {
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        gameObject.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
    }
}
//