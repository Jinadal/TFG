using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
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
                if (w.Vertices[i].Edges[j].RightFace != null && w.Vertices[i].Edges[j].LeftFace != null)
                {
                    float x = w.Vertices[i].Edges[j].LeftFace.FaceCircumcenter.x;
                    float y = w.Vertices[i].Edges[j].LeftFace.FaceCircumcenter.z;
                    if(!vertices2D.Contains(new Vector2(x,y)))
                        vertices2D.Add(new Vector2(x, y));


                    float x2 = w.Vertices[i].Edges[j].RightFace.FaceCircumcenter.x;
                    float y2 = w.Vertices[i].Edges[j].RightFace.FaceCircumcenter.z;
                    if(!vertices2D.Contains(new Vector2(x2,y2)))
                        vertices2D.Add(new Vector2(x2, y2));
                }
            }
            //Debug.Log(vertices2D.Count);
            if (vertices2D.Count == 3)
            {
                if(!IsClockwise(vertices2D[0], vertices2D[1], vertices2D[2]))
                {
                    Vector2 aux = vertices2D[1];
                    vertices2D[1] = vertices2D[2];
                    vertices2D[2] = aux;
                }
            }
            if (vertices2D.Count > 3)
                GrahamScan(vertices2D);
            //Debug.Log(vertices2D.Count);
            Triangulate(vertices2D.ToArray());
            vertices2D.Clear();
        }
        
        // Use the triangulator to get indices for creating triangles
    }



    private void GrahamScan(List<Vector2> v)
    {
        List<Vector2> auxList = v;
         
        Quicksort(auxList, 0, auxList.Count - 1);

        List<Vector2> wallDegree = DegreeSort(auxList);                //Sort the array by their angle with the lowest vector

        List<Vector2> convexHull = new List<Vector2>();

        convexHull.Add(wallDegree[0]);
        convexHull.Add(wallDegree[1]);
        convexHull.Add(wallDegree[2]);
        
        if(!IsClockwise(convexHull[0], convexHull[1], convexHull[2]))
        {
            Vector2 aux = convexHull[1];
            convexHull[1] = convexHull[2];
            convexHull[2] = aux;
        }

        //Debug.Log(v.Count);
        for (int i = 3; i < wallDegree.Count; i++)
        {
            while (orientation(wallDegree[i], convexHull[convexHull.Count - 2], convexHull[convexHull.Count - 1]) != 2)
            {
                convexHull.Remove(convexHull[convexHull.Count - 1]);
            }
            convexHull.Add(wallDegree[i]);
        }
        //Debug.Log(v.Count);
        //return convexHull;
    }
    int orientation(Vector2 C, Vector2 B, Vector2 A)
    {

        float aux = (B.x - A.x) * (C.y - A.y) - (B.y - A.y) * (C.x - A.x);
        if (aux > 0)
        {
            return 1;
        }
        if (aux < 0)
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }
    List<Vector2> DegreeSort(List<Vector2> points)
    {
        Vector2[] aux = new Vector2[points.Count];
        float[] degree = new float[points.Count];
        degree[0] = 0.0f;
        aux[0] = points[0];
        for (int i = 1; i < points.Count; i++)
        {
            degree[i] = calculateDegree(points[0], points[i]);
        }

        points = DegreeQuicksort(degree, 0, degree.Length - 1, points);

        return points;
    }
    List<Vector2> DegreeQuicksort(float[] list, int first, int last, List<Vector2> points)
    {
        int i, j, central;
        float pivote;
        central = (first + last) / 2;
        pivote = list[central];
        i = first;
        j = last;
        do
        {
            while (list[i] < pivote) i++;
            while (list[j] > pivote) j--;
            if (i <= j)
            {
                float temp;
                Vector2 temp3;
                temp = list[i];
                list[i] = list[j];
                list[j] = temp;
                temp3 = points[i];
                points[i] = points[j];
                points[j] = temp3;

                i++;
                j--;
            }
        } while (i <= j);

        if (first < j)
        {
            DegreeQuicksort(list, first, j, points);
        }
        if (i < last)
        {
            DegreeQuicksort(list, i, last, points);
        }
        return points;
    }
    float calculateDegree(Vector3 A, Vector3 B)
    {
        Vector3 C = new Vector3(A.x + 1, 0, A.z);

        return Vector3.Angle(C - A, B - A);
    }


    public void Quicksort(List<Vector2> list, int primero, int ultimo)
    {
        int i, j, central;
        float pivote;
        central = (primero + ultimo) / 2;
        pivote = list[central].x;
        i = primero;
        j = ultimo;
        do
        {
            while (list[i].x < pivote) i++;
            while (list[j].x > pivote) j--;
            if (i <= j)
            {
                Vector2 temp;
                temp = list[i];
                list[i] = list[j];
                list[j] = temp;
                i++;
                j--;
            }
        } while (i <= j);

        if (primero < j)
        {
            Quicksort(list, primero, j);
        }
        if (i < ultimo)
        {
            Quicksort(list, i, ultimo);
        }
    }
    private bool IsClockwise(Vector2 a, Vector2 b, Vector2 c)
    {
        Vector2 AB = b - a;
        Vector2 AC = c - a;

        Vector3 crossvalue = Vector3.Cross(AB, AC);
        if (crossvalue.z > 0.0f)
        {
            return true;
        }
        return false;
    }

    private void Triangulate(Vector2[] vertices2D)
    {
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, 0, vertices2D[i].y);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        GameObject gameObject = new GameObject();
        gameObject.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
    }
}
//