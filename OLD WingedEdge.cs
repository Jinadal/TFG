//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//public class OLDWingedEdge : MonoBehaviour
//{
//    public List<EdgeWE> Edges = new List<EdgeWE>();
//    public List<VertexWE> Vertices = new List<VertexWE>();
//    public List<FaceWE> Faces = new List<FaceWE>();
//
//    public OLDWingedEdge() { }
//
//    public void CheckEdge(List<EdgeWE> edges)
//    {
//        foreach (EdgeWE e in edges)
//        {
//            if (e.LeftFace != null && e.RightFace != null)
//            {
//                List<VertexWE> v1 = getFaceVertices(e.RightFace);
//                List<VertexWE> v2 = getFaceVertices(e.LeftFace);
//
//                VertexWE rightOppositeVertex = null;
//                for (int i = 0; i < v1.Count; i++)
//                {
//                    if ((v1[i] != e.Vertex1) && v1[i] != e.Vertex2)
//                    {
//                        rightOppositeVertex = v1[i];
//                    }
//                }
//
//                VertexWE leftOppositeVertex = null;
//                for (int i = 0; i < v2.Count; i++)
//                {
//                    if ((v2[i] != e.Vertex1) && v2[i] != e.Vertex2)
//                    {
//                        leftOppositeVertex = v2[i];
//                    }
//                }
//                Vector3 ccl = CalculateCircumscribedCircumference(e.Vertex1, e.Vertex2, leftOppositeVertex);
//                Vector3 ccr = CalculateCircumscribedCircumference(e.Vertex1, e.Vertex2, rightOppositeVertex);
//
//                float radiusl = CalculateRadius(ccl, e.Vertex1);
//                float radiusr = CalculateRadius(ccr, e.Vertex2);
//                if (InsideCC(ccl, radiusl, rightOppositeVertex) || InsideCC(ccr, radiusr, leftOppositeVertex))
//                {
//                    List<FaceWE> newfaces = FlipEdge(e);
//                    List<EdgeWE> newedges = new List<EdgeWE>();
//                    for (int m = 0; m < newfaces.Count; m++)
//                    {
//                        for (int n = 0; n < newfaces[m].Edges.Count; n++)
//                        {
//                            if (!newedges.Contains(newfaces[m].Edges[n]))
//                            {
//                                newedges.Add(newfaces[m].Edges[n]);
//                            }
//                        }
//                    }
//                    CheckEdge(newedges);
//                }
//            }
//        }
//    }
//
//    public List<FaceWE> FlipEdge(EdgeWE e)
//    {
//        List<FaceWE> f = new List<FaceWE>();
//        VertexWE v1 = e.Vertex1;
//        VertexWE v2 = e.Vertex2;
//        VertexWE v3 = null;
//        VertexWE v4 = null;
//
//        FaceWE lf = e.LeftFace;
//        FaceWE rf = e.RightFace;
//
//        for (int i = 0; i < lf.Edges.Count; i++)
//        {
//            if (lf.Edges[i].Vertex1 != v1 && lf.Edges[i].Vertex1 != v2)
//            {
//                v3 = lf.Edges[i].Vertex1;
//            }
//            if (lf.Edges[i].Vertex2 != v1 && lf.Edges[i].Vertex2 != v2)
//            {
//                v3 = lf.Edges[i].Vertex2;
//            }
//        }
//        for (int i = 0; i < rf.Edges.Count; i++)
//        {
//            if (rf.Edges[i].Vertex1 != v1 && rf.Edges[i].Vertex1 != v2)
//            {
//                v4 = rf.Edges[i].Vertex1;
//            }
//            if (rf.Edges[i].Vertex2 != v1 && rf.Edges[i].Vertex2 != v2)
//            {
//                v4 = rf.Edges[i].Vertex2;
//            }
//        }
//        RemoveFace(e.LeftFace);
//        RemoveFace(e.RightFace);
//
//        f.Add(AddFace(v1, v3, v4));
//        f.Add(AddFace(v2, v3, v4));
//
//        return f;
//    }
//
//    public bool InsideCC(Vector3 c, float r, VertexWE p)
//    {
//        if (Vector3.Distance(c, p.Position) < r)
//        {
//            return true;
//        }
//        return true;
//    }
//    public float CalculateRadius(Vector3 c, VertexWE p)
//    {
//        float r = Vector3.Distance(c, p.Position);
//        return r;
//    }
//
//    public Vector3 CalculateCircumscribedCircumference(VertexWE a, VertexWE b, VertexWE c)
//    {
//        float rABa, rABb, rABc, rACa, rACb, rACc, mABx, mABy, mACx, mACy, prAB, prAC;
//
//        Vector3 v1 = a.Position;
//        Vector3 v2 = b.Position;
//        Vector3 v3 = c.Position;
//
//        rABa = v2.z - v1.z;
//        rABb = v1.x - v2.x;
//        rABc = rABa * v1.x + rABb * v1.z;
//
//        rACa = v3.z - v1.z;
//        rACb = v1.x - v3.x;
//        rACc = rABa * v1.x + rABb * v1.z;
//
//        mABx = (v1.x + v2.x) / 2;
//        mABy = (v1.z + v2.z) / 2;
//
//        mACx = (v1.x + v3.x) / 2;
//        mACy = (v1.z + v3.z) / 2;
//
//        //c     b     x     a     y
//        prAB = -rABb * mABx + rABa * mABy;
//        prAC = -rACb * mACx + rACa * mACy;
//
//        float determinant = (-rABb) * rACa - (-rACb) * rABa;
//
//        float x = (rACa * prAB - rABa * prAC) / determinant;
//        float y = ((-rABb) * prAC - (-rACb) * prAB) / determinant;
//
//        return new Vector3(x, 0, y);
//    }
//
//
//}