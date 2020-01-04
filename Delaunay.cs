using System.Collections;   
using System.Collections.Generic;
using UnityEngine;


public class Delaunay : MonoBehaviour
{
    public float x;
    public float y;
    public int points = 3;
    public bool Delaunay_Vertices = true;
    public bool Delaunay_Triangulation = true;
    public bool Voronoi_Vertices;
    public bool Voronoi_Diagram;
    public bool Wall;
    public bool Circunferations;
    
    WingedEdge wingededge;
    List<Vector3> convexHull;
    List<Vector3> pointlist = new List<Vector3>();
    void Start()
    {
        Initializate();   
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pointlist.Add(new Vector3(Random.Range(0.0f, x), 0.0f, Random.Range(0.0f, y)));
            Continue();
        }
    }

    void Initializate()
    {
        wingededge = ScriptableObject.CreateInstance("WingedEdge") as WingedEdge;

        for(int i = 0; i < points; i++)
        {
            pointlist.Add(new Vector3(Random.Range(0.0f,x),0.0f,Random.Range(0.0f,y)));
        }
        
        wingededge.AddVertex(0.0f,0.0f,0.0f);
        wingededge.AddVertex(0.0f,0.0f,y);
        wingededge.AddVertex(x,0.0f,y);
        wingededge.AddVertex(x,0.0f,0.0f);
        
        wingededge.AddFace(wingededge.Vertices[0], wingededge.Vertices[1], wingededge.Vertices[2]);
        wingededge.AddFace(wingededge.Vertices[0], wingededge.Vertices[2], wingededge.Vertices[3]);

        Continue();
    }

    public void Continue()
    {
        while (pointlist.Count > 0)
        {
            VertexWE v = wingededge.AddVertex(pointlist[0].x, pointlist[0].y, pointlist[0].z);
            pointlist.RemoveAt(0);

            FaceWE currentFace = wingededge.PointInTriangle(v);
            List<FaceWE> newFaces = wingededge.AddVertex(currentFace, v);
            

        }

        CheckEdge(wingededge);

        List<FaceWE> faceToRemove = new List<FaceWE>();

        for (int i = wingededge.Faces.Count - 1; i >= 0; i--)
        {
            if (wingededge.IsFaceBorder(wingededge.Faces[i]))
            {
                faceToRemove.Add(wingededge.Faces[i]);
            }
        }

        for (int i = 0; i < faceToRemove.Count; i++)
        {
            wingededge.RemoveFace(faceToRemove[i]);
        }

        List<Vector3> VoronoiVertices = new List<Vector3>();

        for (int i = 0; i < wingededge.Faces.Count; i++)
        {
            List<VertexWE> vertices = wingededge.GetFaceVertices(wingededge.Faces[i]);

            wingededge.Faces[i].FaceCircumcenter = CalculateCircumscribedCircumference(vertices[0], vertices[1], vertices[2]);
        }

        GrahamScan();
    }
    public void GrahamScan()
    {
        Vector3[] auxList = new Vector3[wingededge.Faces.Count]; 
        Vector3[] wall;
        int aux = 0;
        int add = 0;
        for (int i = 0; i < auxList.Length; i++)
        {
            auxList[i] = wingededge.Faces[i].FaceCircumcenter;  //Vornoi Vertexs
        }
        Quicksort(auxList, 0, auxList.Length - 1); 

        for (int j = 0; j < auxList.Length; j++)
        {
            if (auxList[j].z > -15 && auxList[j].z < y + 15 && auxList[j].x > -15 && auxList[j].x < x + 15) //Limit de vertex we want 
            {                                                                                               //to a distance
                aux++;
            }
        }
        wall = new Vector3[aux];                                //Valid vertexs inside the wall
        for (int i = 0; i < auxList.Length; i++)
        {
            if (auxList[i].z > -15 && auxList[i].z < y + 15 && auxList[i].x > -15 && auxList[i].x < x + 15)
            {
                wall[add] = auxList[i];             
                add++;
            }
        }

        Vector3[] wallDegree = DegreeSort(wall);                //Sort the array by their angle with the lowest vector
        
        convexHull = new List<Vector3>();
    
        convexHull.Add(wallDegree[0]);
        convexHull.Add(wallDegree[1]);
        convexHull.Add(wallDegree[2]);
    
        for(int i = 3; i < wallDegree.Length; i++)
        {
            while(orientation(wallDegree[i], convexHull[convexHull.Count - 2], convexHull[convexHull.Count - 1] ) != 2)
            { 
                convexHull.Remove(convexHull[convexHull.Count - 1]);
            }
            convexHull.Add(wallDegree[i]);
        }
    }
    int orientation(Vector3 C, Vector3 B, Vector3 A)
    {
        float aux = (B.x - A.x) * (C.z - A.z) - (B.z - A.z) * (C.x - A.x);
        if(aux > 0)
        {
            return 1;
        }
        if(aux < 0)
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }
    Vector3[] DegreeSort(Vector3[] points)
    {
        Vector3[] aux = new Vector3[points.Length];
        float[] degree = new float[points.Length];
        degree[0] = 0.0f;
        aux[0] = points[0];
        for(int i = 1; i < points.Length; i++)
        {
            degree[i] = calculateDegree(points[0],points[i]);
        }
        for (int m = 0; m < points.Length; m++)
        {
            Debug.Log("vector "+points[m].x + " , " + points[m].z);
            Debug.Log("angulo "+degree[m]);

        }
        points = DegreeQuicksort(degree, 0, degree.Length - 1, points);
        
        return points;
    }
    Vector3[] DegreeQuicksort(float[] list, int first, int last, Vector3[] points)
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
                Vector3 temp3;
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

    public void Quicksort(Vector3[] list, int first, int last)
    {
        int i, j, central;
        float pivote;
        central = (first + last) / 2;
        pivote = list[central].z;
        i = first;
        j = last;
        do
        {
            while (list[i].z < pivote) i++;
            while (list[j].z > pivote) j--;
            if (i <= j)
            {
                Vector3 temp;
                temp = list[i];
                list[i] = list[j];
                list[j] = temp;
                i++;
                j--;
            }
        } while (i <= j);

        if (first < j)
        {
            Quicksort(list, first, j);
        }
        if (i < last)
        {
            Quicksort(list, i, last);
        }
    }

    public void CheckEdge(WingedEdge w)
    {
        bool aux = false;
        foreach (EdgeWE e in w.Edges)
        {
            if (e.LeftFace != null && e.RightFace != null)
            {
                List<VertexWE> v1 = w.GetFaceVertices(e.RightFace);
                List<VertexWE> v2 = w.GetFaceVertices(e.LeftFace);

                VertexWE RightOppositeVertex = null;
                VertexWE LeftOppositeVertex = null;


                for (int i = 0; i < v1.Count; i++)
                {
                    if (!Equals(v1[i], e.Vertex1) && !Equals(v1[i], e.Vertex2))
                    {
                        RightOppositeVertex = v1[i];
                    }
                    if (!Equals(v2[i], e.Vertex1) && !Equals(v2[i], e.Vertex2))
                    {
                        LeftOppositeVertex = v2[i];
                    }
                }
                Vector3 Rightcc = CalculateCircumscribedCircumference(v1[0], v1[1], v1[2]);
                Vector3 Leftcc = CalculateCircumscribedCircumference(v2[0], v2[1], v2[2]);
                double RightccRadius = CalculateRadius(Rightcc, v1[0], v1[1], v1[2]);
                double LeftccRadius = CalculateRadius(Leftcc, v2[0], v2[1], v2[2]);
                if (InsideCC(Rightcc, RightccRadius, LeftOppositeVertex) || InsideCC(Leftcc, LeftccRadius, RightOppositeVertex))
                {
                    List<FaceWE> newfaces = w.FlipEdge(e);
                    aux = true;
                    break;
                }
            }
        }
        if (aux)
        {
            CheckEdge(w);
        }
    }

    public bool InsideCC(Vector3 c, double r, VertexWE p)
    {
        if (Vector3.Distance(c, p.Position) < r)
        {
            return true;
        }
        return false;
    }

    public Vector3 CalculateCircumscribedCircumference(VertexWE a, VertexWE b, VertexWE c)
    {
        float rABa, rABb, rABc, rACa, rACb, rACc, mABx, mABy, mACx, mACy, prAB, prAC;

        Vector3 v1 = a.Position;
        Vector3 v2 = b.Position;
        Vector3 v3 = c.Position;

        rABa = v2.z - v1.z;
        rABb = v1.x - v2.x;
        rABc = rABa * v1.x + rABb * v1.z;

        rACa = v3.z - v1.z;
        rACb = v1.x - v3.x;
        rACc = rABa * v1.x + rABb * v1.z;

        mABx = (v1.x + v2.x) / 2;
        mABy = (v1.z + v2.z) / 2;

        mACx = (v1.x + v3.x) / 2;
        mACy = (v1.z + v3.z) / 2;

        //c     b     x     a     y
        prAB = -rABb * mABx + rABa * mABy;
        prAC = -rACb * mACx + rACa * mACy;

        float determinant = (-rABb) * rACa - (-rACb) * rABa;

        float x = (rACa * prAB - rABa * prAC) / determinant;
        float y = ((-rABb) * prAC - (-rACb) * prAB) / determinant;

        return new Vector3(x, 0, y);
    }


    public double CalculateRadius(Vector3 cc, VertexWE A, VertexWE B, VertexWE C)
    {
        double maxradius = 0;

        if (Vector3.Distance(cc, A.Position) > maxradius)
            maxradius = Vector3.Distance(cc, A.Position);

        if (Vector3.Distance(cc, B.Position) > maxradius)
            maxradius = Vector3.Distance(cc, B.Position);

        if (Vector3.Distance(cc, C.Position) > maxradius)
            maxradius = Vector3.Distance(cc, C.Position);
        return maxradius;
    }

    public void OnDrawGizmos()
    {
        if (Circunferations)
        {
            for (int i = 0; i < wingededge.Faces.Count; i++)
            {
                if (i % 2 == 0)
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = Color.magenta;
                }
                List<VertexWE> v = wingededge.GetFaceVertices(wingededge.Faces[i]);
                Vector3 cc = wingededge.Faces[i].FaceCircumcenter;
                float r = (float)CalculateRadius(cc, v[0], v[1], v[2]);
                Gizmos.DrawWireSphere(cc, r);
            }
        }
        Gizmos.color = Color.green;
        if (Delaunay_Triangulation)
        {
            for (int i = 0; i < wingededge.Edges.Count; i++)
            {
                Gizmos.DrawLine(wingededge.Edges[i].Vertex1.Position, wingededge.Edges[i].Vertex2.Position);
            }
        }
        Gizmos.color = Color.red;
        if (Delaunay_Vertices)
        {
            for (int i = 0; i < wingededge.Vertices.Count; i++)
            {
                Gizmos.DrawSphere(wingededge.Vertices[i].Position, 1f);
            }
        }
        if (Voronoi_Diagram)
        {
            for (int i = 0; i < wingededge.Faces.Count; i++)
            {
                for (int j = 0; j < wingededge.Faces[i].Edges.Count; j++)
                {
                    if (wingededge.Faces[i].Edges[j].LeftFace != null && wingededge.Faces[i].Edges[j].LeftFace.FaceCircumcenter != wingededge.Faces[i].FaceCircumcenter)
                    {
                        Gizmos.DrawLine(wingededge.Faces[i].FaceCircumcenter, wingededge.Faces[i].Edges[j].LeftFace.FaceCircumcenter);
                    }
                    if (wingededge.Faces[i].Edges[j].RightFace != null && wingededge.Faces[i].Edges[j].RightFace.FaceCircumcenter != wingededge.Faces[i].FaceCircumcenter)
                    {
                        Gizmos.DrawLine(wingededge.Faces[i].FaceCircumcenter, wingededge.Faces[i].Edges[j].RightFace.FaceCircumcenter);
                    }
                }
            }
        }
        Gizmos.color = Color.black;
        if (Voronoi_Vertices)
        {
            for (int i = 0; i < wingededge.Faces.Count; i++)
            {
                Gizmos.DrawSphere(wingededge.Faces[i].FaceCircumcenter, 1f);
            }
        }
        //Gizmos.DrawSphere(wingededge.Vertices[wingededge.Vertices.Count-1].Position, 1f);
        if(Wall)
        {
            Gizmos.color = Color.black;
            for(int i = 0; i < convexHull.Count; i++)
            {
                if(i != convexHull.Count - 1)
                    Gizmos.DrawLine(convexHull[i], convexHull[i + 1]);
                else
                {
                    Gizmos.DrawLine(convexHull[i], convexHull[0]);
                }
            }
        }
    }
}