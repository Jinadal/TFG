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

                    

                    b = PointInLine(a, b);
                    c = PointInLine(a, c);


                    if (!IsClockwise(a, b, c))
                    {
                        Vector3 aux2 = c;
                        c = b;
                        b = aux2;
                    }

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
        mesh.vertices = vertices.ToArray();                             //  y - y1      y2 - y1
        mesh.triangles = triangles.ToArray();                           //  x - x1   =  x2 - x1
    }

    Vector3 PointInLine(Vector3 A, Vector3 B)
    {
        Vector3 v = new Vector3(B.x - A.x, 0, B.z - A.z); 
        

        B.x = A.x + (float)(0.9 * v.x);
        B.z = A.z + (float)(0.9 * v.z);
        return B;
    }

   // public void GenerateMesh(WingedEdge w)
   // {
   //     
   //
   //
   //     for(int i = 0; i < w.Vertices.Count; i++)
   //     {
   //         List<Vector3> polygonVertex = new List<Vector3>();
   //         for (int j = 0; j < w.Vertices[i].Edges.Count; j++)
   //         {
   //             polygonVertex.Add(w.Vertices[i].Edges[j].LeftFace.FaceCircumcenter);
   //         }
   //         createMesh(polygonVertex);
   //     }
   //
   // }

    void createMesh(List<Vector3> p)
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;


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
