using System.Collections;   
using System.Collections.Generic;
using UnityEngine;

public class Delaunay : MonoBehaviour
{
    public int x;
    public int y;
    public int points = 3;

    List<Vector2> centroids;
    List<Triangle> Triangulations;
    void Start()
    {
        GeneratePoints();   
    }

    void Update()
    {
         if (Input.GetMouseButtonDown(0))
            GeneratePoints();

    }

    void GeneratePoints()
    {
        centroids = new List<Vector2>();
        Triangulations = new List<Triangle>();
        for(int i = 0; i < points; i++)
        {
            centroids.Add(new Vector2(Random.Range(0f,x),Random.Range(0f,y)));
            if(i == 2)
            {
                FirstTriangulation(centroids);
            }
        }

    }

    void FirstTriangulation(List<Vector2> c)
    {
        Triangle t = new Triangle(c[0],c[1],c[2]);
        Triangulations.Add(t);
    }


    class Triangle
    {
        public List<Vector2> vertices;
        public Vector2 centerCircumference;
        public float radius;

        public Triangle(){}
        public Triangle(Vector2 p1,Vector2 p2,Vector2 p3)
        {
            vertices = new List<Vector2>();
            vertices.Add(p1);
            vertices.Add(p2);
            vertices.Add(p3);
            centerCircumference = calculateCircumscribedCircumference();
            radius = Vector2.Distance(centerCircumference,p1);
        }

        public Vector2 calculateCircumscribedCircumference()
        {
            float rABa, rABb, rABc, rACa, rACb, rACc, mABx, mABy, mACx, mACy, prAB, prAC;

            rABa = vertices[1][1] - vertices[0][1];
            rABb = vertices[0][0] - vertices[1][0];
            rABc = rABa*vertices[0][0] + rABb*vertices[0][1];

            rACa = vertices[2][1] - vertices[0][1];
            rACb = vertices[0][0] - vertices[2][0];
            rACc = rABa*vertices[0][0] + rABb*vertices[0][1];

            mABx = (vertices[0][0] + vertices[1][0]) / 2;
            mABy = (vertices[0][1] + vertices[1][1]) / 2;

            mACx = (vertices[0][0] + vertices[2][0]) / 2;
            mACy = (vertices[0][1] + vertices[2][1]) / 2;
           
            //c     b     x     a     y
            prAB = -rABb*mABx + rABa*mABy;
            prAC = -rACb*mACx + rACa*mACy;

            float determinant = (-rABb) * rACa - (-rACb) * rABa; 

            float x = (rACa * prAB - rABa * prAC) / determinant; 
            float y = ((-rABb) * prAC - (-rACb) * prAB) / determinant; 

            return new Vector2(x, y); 
        }

        public List<Vector2> getVertices()
        {
            return vertices;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        for(int i = 0; i < Triangulations.Count; i++)
        {
            Gizmos.DrawCube(new Vector3(Triangulations[i].vertices[0][0],0,Triangulations[i].vertices[0][1]), new Vector3(1,1,1));
            Gizmos.DrawCube(new Vector3(Triangulations[i].vertices[1][0],0,Triangulations[i].vertices[1][1]), new Vector3(1,1,1));
            Gizmos.DrawCube(new Vector3(Triangulations[i].vertices[2][0],0,Triangulations[i].vertices[2][1]), new Vector3(1,1,1));

            Gizmos.DrawLine(new Vector3(Triangulations[i].vertices[0][0],0,Triangulations[i].vertices[0][1]),new Vector3(Triangulations[i].vertices[1][0],0,Triangulations[i].vertices[1][1]));
            Gizmos.DrawLine(new Vector3(Triangulations[i].vertices[0][0],0,Triangulations[i].vertices[0][1]),new Vector3(Triangulations[i].vertices[2][0],0,Triangulations[i].vertices[2][1]));
            Gizmos.DrawLine(new Vector3(Triangulations[i].vertices[1][0],0,Triangulations[i].vertices[1][1]),new Vector3(Triangulations[i].vertices[2][0],0,Triangulations[i].vertices[2][1]));
    
    
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(new Vector3(Triangulations[i].centerCircumference[0],0,Triangulations[i].centerCircumference[1]), Triangulations[i].radius);
        }
    }
}

