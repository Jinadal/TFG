using System.Collections;   
using System.Collections.Generic;
using UnityEngine;

public class Delaunay : MonoBehaviour
{
    public float x;
    public float y;
    public int points = 3;
    WingedEdge wingededge;
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
        //for (int i = 0; i < wingededge.Edges.Count; i++)
        //{
           wingededge.CheckEdge();
        //}
    }

    public void OnDrawGizmos()
    {
        for(int i = 0; i < wingededge.Faces.Count; i++)
        {
            if(i%2 == 0)
            {
                Gizmos.color = Color.blue;
            }
            else
            {
                Gizmos.color = Color.magenta;
            }
            List<VertexWE> v = wingededge.GetFaceVertices(wingededge.Faces[i]);
            Vector3 cc = wingededge.CalculateCircumscribedCircumference(v[0], v[1], v[2]);
            float r = (float)wingededge.CalculateRadius(cc, v[0], v[1], v[2]);
            Gizmos.DrawWireSphere(cc, r);
        }
        Gizmos.color = Color.green;
        for (int i = 0; i < wingededge.Edges.Count; i++)
        {
            Gizmos.DrawLine(wingededge.Edges[i].Vertex1.Position, wingededge.Edges[i].Vertex2.Position);
        }
        Gizmos.color = Color.red;
        for (int i = 0; i < wingededge.Vertices.Count; i++)
        {
            Gizmos.DrawSphere(wingededge.Vertices[i].Position, 1f);
        }
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(wingededge.Vertices[wingededge.Vertices.Count-1].Position, 1f);

    }
}