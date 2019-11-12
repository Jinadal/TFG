using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingedEdge : ScriptableObject
{
    public List<VertexWE> Vertices = new List<VertexWE>();
    public List<EdgeWE> Edges = new List<EdgeWE>();
    public List<FaceWE> Faces = new List<FaceWE>();

    public WingedEdge() { }

    public VertexWE AddVertex(float x, float y, float z)
    {
        VertexWE v = ScriptableObject.CreateInstance("VertexWE") as VertexWE;
        v.Init(x, y, z);
        v.id = Vertices.Count;
        Vertices.Add(v);
        return v;
    }

    public void RemoveFace(FaceWE f)
    {
        foreach (EdgeWE e in f.Edges)
        {
            if (Equals(e.RightFace,f))
            {
                e.RightFace = null;
            }

            if (Equals(e.LeftFace,f))
            {
                e.LeftFace = null;
            }

            if (e.LeftFace == null && e.RightFace == null)
            {
                e.Vertex1.Edges.Remove(e);
                e.Vertex2.Edges.Remove(e);
                Edges.Remove(e);
            }
        }
        Faces.Remove(f);
    }

    public List<FaceWE> AddVertex(FaceWE f, VertexWE v)
    {
        List<VertexWE> vertices = GetFaceVertices(f);
        List<FaceWE> faces = new List<FaceWE>();
        RemoveFace(f);

        faces.Add(AddFace(vertices[0], v, vertices[1]));
        faces.Add(AddFace(vertices[1], v, vertices[2]));
        faces.Add(AddFace(vertices[2], v, vertices[0]));
        return faces;
    }

    public List<VertexWE> GetFaceVertices(FaceWE f)
    {
        bool v1founded, v2founded;
        List<VertexWE> vertices = new List<VertexWE>();

        for (int i = 0; i < f.Edges.Count; i++)
        {
            v1founded = false;
            v2founded = false;

            VertexWE v1 = f.Edges[i].Vertex1;
            VertexWE v2 = f.Edges[i].Vertex2;

            if (f.Edges[i].LeftFace == f)
            {
                v1 = f.Edges[i].Vertex2;
                v2 = f.Edges[i].Vertex1;
            }

            for (int j = 0; j < vertices.Count; j++)
            {
                if (vertices[j].id == v1.id)
                {
                    v1founded = true;
                }
                if (vertices[j].id == v2.id)
                {
                    v2founded = true;
                }
            }
            if (!v1founded)
            {
                vertices.Add(v1);
            }
            if (!v2founded)
            {
                vertices.Add(v2);
            }
        }
        return vertices;
    }

    public FaceWE PointInTriangle(VertexWE p)
    {
        foreach (FaceWE f in Faces)
        {
        
            List<VertexWE> vrts = GetFaceVertices(f);
        
            double A = TriangleArea(vrts[0], vrts[1], vrts[2]);
            double A1 = TriangleArea(p, vrts[0], vrts[1]);
            A1 += TriangleArea(vrts[1], p, vrts[2]);
            A1 += TriangleArea(vrts[2], vrts[0], p);

            double In = A1 - A;

            if (In < 0.01)
            {
                return f;
            }
        }
        return null;
    }

    public float TriangleArea(VertexWE a, VertexWE b, VertexWE c)
    {
        return Mathf.Abs((float)a.Position.x * (b.Position.z - c.Position.z) +
                         b.Position.x * (c.Position.z - a.Position.z) +
                         c.Position.x * (a.Position.z - b.Position.z))/2.0f;
    }

    public FaceWE AddFace(VertexWE a, VertexWE b, VertexWE c)
    {
        if(!FaceAlreadyExist(a,b,c))
        {
            if (!IsClockwise(a,b,c))
            {
                VertexWE aux = a;
                a = b;
                b = aux;
            }

            FaceWE f = ScriptableObject.CreateInstance("FaceWE") as FaceWE;

            EdgeWE e1 = EdgeAlreadyExist(a, b);
            EdgeWE e2 = EdgeAlreadyExist(b, c);
            EdgeWE e3 = EdgeAlreadyExist(c, a);
            
            if(e1 == null)
            {
                EdgeWE aux1 = ScriptableObject.CreateInstance("EdgeWE") as EdgeWE;
                aux1.Init(a, b);
                a.Edges.Add(aux1);
                b.Edges.Add(aux1);
                Edges.Add(aux1);
                f.Edges.Add(aux1);
                aux1.RightFace = f;
            }
            else
            {
                if(e1.RightFace == null)
                {
                    e1.RightFace = f;
                }
                else
                {
                    e1.LeftFace = f;
                }
                f.Edges.Add(e1);
            }
            if (e2 == null)
            {
                EdgeWE aux2 = ScriptableObject.CreateInstance("EdgeWE") as EdgeWE;
                aux2.Init(b, c);
                b.Edges.Add(aux2);
                c.Edges.Add(aux2);
                Edges.Add(aux2);
                aux2.RightFace = f;
                f.Edges.Add(aux2);
            }
            else
            {
                if (e2.RightFace == null)
                {
                    e2.RightFace = f;
                }
                else
                {
                    e2.LeftFace = f;
                }
                f.Edges.Add(e2);
            }
            if (e3 == null)
            {
                EdgeWE aux3 = ScriptableObject.CreateInstance("EdgeWE") as EdgeWE;
                aux3.Init(c, a);
                c.Edges.Add(aux3);
                a.Edges.Add(aux3);
                Edges.Add(aux3);
                aux3.RightFace = f;
                f.Edges.Add(aux3);
            }
            else
            { 
                if (e3.RightFace == null)
                {
                    e3.RightFace = f;
                }
                else
                {
                    e3.LeftFace = f;
                }
                f.Edges.Add(e3);
            }

            Faces.Add(f);

            return f;
        }
        return null;
    }

    public EdgeWE EdgeAlreadyExist(VertexWE v1, VertexWE v2)
    {
        foreach(EdgeWE e in Edges)
        {
            if((v1 == e.Vertex1 && v2 == e.Vertex2) || (v1 == e.Vertex2 && v2 == e.Vertex1))
            {
                return e;
            }
        }
        return null;
    }

    public bool IsClockwise(VertexWE a, VertexWE b, VertexWE c)
    {
        Vector3 AB = b.Position - a.Position;
        Vector3 AC = c.Position - a.Position;

        Vector3 crossvalue = Vector3.Cross(AB, AC);
        if (crossvalue.y > 0.0f)
        {
            return true;
        }
        return false;
    }

    public bool FaceAlreadyExist(VertexWE a, VertexWE b, VertexWE c)
    {
        foreach(FaceWE f in Faces)
        {
            foreach(EdgeWE e in f.Edges)
            {
                bool Afounded = false;
                bool Bfounded = false;
                bool Cfounded = false;
                if(Equals(a,e.Vertex1) || Equals(a,e.Vertex2))
                {
                    Afounded = true;
                }
                if (Equals(b, e.Vertex1) || Equals(b, e.Vertex2))
                {
                    Bfounded = true;
                }
                if (Equals(c, e.Vertex1) || Equals(c, e.Vertex2))
                {
                    Cfounded = true;
                }
                if(Afounded && Bfounded && Cfounded)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool Equals(VertexWE a, VertexWE b)
    {
        if (a != null && b != null)
        {
            if (a.id == b.id)
            {
                if (a.Position == b.Position)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void CheckEdge()
     {
        bool aux = false;
         foreach (EdgeWE e in Edges)
         {
             if (e.LeftFace != null && e.RightFace != null)
             {
                 List<VertexWE> v1 = GetFaceVertices(e.RightFace);
                 List<VertexWE> v2 = GetFaceVertices(e.LeftFace);
     
                 VertexWE RightOppositeVertex = null;
                 VertexWE LeftOppositeVertex = null;
     
     
                 for(int i = 0; i < v1.Count; i++)
                 {
                     if(!Equals(v1[i],e.Vertex1) && !Equals(v1[i],e.Vertex2))
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
                 if(InsideCC(Rightcc,RightccRadius,LeftOppositeVertex) || InsideCC(Leftcc,LeftccRadius,RightOppositeVertex))
                 {
                    List<FaceWE> newfaces = FlipEdge(e);
                    aux = true;
                    break;
                 }
             }
         }
         if(aux)
        {
            CheckEdge();
        }
         
     }
    // 
    public bool InsideCC(Vector3 c, double r, VertexWE p)
    {
        if (Vector3.Distance(c, p.Position) < r)
        {
            return true;
        }
        return false;
    }

    public double CalculateRadius(Vector3 cc, VertexWE A, VertexWE B, VertexWE C)
    {
        double maxradius = 0;

        if (Vector3.Distance(cc,A.Position) > maxradius)
            maxradius = Vector3.Distance(cc, A.Position);

        if (Vector3.Distance(cc, B.Position) > maxradius)
            maxradius = Vector3.Distance(cc, B.Position);

        if (Vector3.Distance(cc, C.Position) > maxradius)
            maxradius = Vector3.Distance(cc, C.Position);
        return maxradius;
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

    public List<FaceWE> FlipEdge(EdgeWE e)
    {
        List<FaceWE> f = new List<FaceWE>();

        FaceWE lf = e.LeftFace;
        FaceWE rf = e.RightFace;
        VertexWE v1 = e.Vertex1;
        VertexWE v2 = e.Vertex2;
        VertexWE rv = null;
        VertexWE lv = null;


        List<VertexWE> rfvertices = GetFaceVertices(rf);
        List<VertexWE> lfvertices = GetFaceVertices(lf);

        for(int i = 0; i < rfvertices.Count; i++)
        {
            if(!Equals(v1,rfvertices[i]) && !Equals(v2,rfvertices[i]))
            {
                rv = rfvertices[i];
            }
            if(!Equals(v1,lfvertices[i]) && !Equals(v2, lfvertices[i]))
            {
                lv = lfvertices[i];
            }
        }

        RemoveFace(rf);
        RemoveFace(lf);

        f.Add(AddFace(rv,lv,v1));
        f.Add(AddFace(rv,lv,v2));

        return f;
    }

    public bool IsFaceBorder(FaceWE f)
    {
        foreach (EdgeWE e in f.Edges)
        {
            if (e.LeftFace == null || e.RightFace == null)
            {
                return true;
            }
        }

        List<VertexWE> v = GetFaceVertices(f);

        foreach (VertexWE vertex in v)
        {
            foreach (EdgeWE e in vertex.Edges)
            {
                if (e.LeftFace == null || e.RightFace == null)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

