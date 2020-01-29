using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public void GenerateMesh(WingedEdge w)
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        int aux = 0;
        for (int i = 0; i < w.Vertices.Count; i++)
        {
            for (int j = 0; j < w.Vertices[i].Edges.Count; j++)
            {
                if (w.Vertices[i].Edges[j].RightFace != null && w.Vertices[i].Edges[j].LeftFace != null)
                {
                    

                    Vector3 a = w.Vertices[i].Position;
                    Vector3 b = w.Vertices[i].Edges[j].LeftFace.FaceCircumcenter;
                    Vector3 c = w.Vertices[i].Edges[j].RightFace.FaceCircumcenter;

                    if (!IsClockwise(a, b, c))
                    {
                        Vector3 aux2 = c;
                        c = b;
                        b = aux2;
                    }

                    if (a.z > b.z)
                        b.z += 3;
                    else
                        b.z -= 3;
                    if (a.x > b.x)
                        b.x += 3;
                    else
                        b.x -= 3;
                    if (a.z > c.z)
                        c.z += 3;
                    else
                        c.z -= 3;
                    if (a.x > c.x)
                        c.x += 3;
                    else
                        c.x -= 3;

                    
                    vertices.Add(a);
                    vertices.Add(b);
                    vertices.Add(c);

                    triangles.Add(aux + 0);
                    triangles.Add(aux + 1);
                    triangles.Add(aux + 2);

                    aux+=3;
                    
                }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    public bool IsClockwise(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 AB = b - a;
        Vector3 AC = c - a;

        Vector3 crossvalue = Vector3.Cross(AB, AC);
        if (crossvalue.y > 0.0f)
        {
            return true;
        }
        return false;
    }

}
