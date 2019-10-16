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
            Vector2 point = new Vector2(Random.Range(0f,x),Random.Range(0f,y));
            if(i <= 2)
            {
                centroids.Add(point);
                if(i == 2)
                {
                    FirstTriangulation(centroids);
                }
            }
            else
            {
                validatePoint(point);
            }
        }

    }

    void FirstTriangulation(List<Vector2> c)
    {
        Triangle t = new Triangle(c[0],c[1],c[2]);
        Triangulations.Add(t);
    }

    void validatePoint(Vector2 p)
    {
        List<Triangle> invalidT = new List<Triangle>();
        foreach(Triangle t in Triangulations)
        {
            if(InsideCircumferences(t,p))
            {
                invalidT.Add(t);
            }
        }
        foreach(Triangle t in invalidT)
        {
            newTriangulation(t,p);
        }
    }

    void newTriangulation(Triangle t, Vector2 p)
    {
        Vector2 v1,v2,v3;
        float dpv1,dpv2,dpv3;
        Triangle triangle1 = null;
        Triangle triangle2 = null;
        v1 = t.getVertices()[0];
        v2 = t.getVertices()[1];
        v3 = t.getVertices()[2];

        Triangulations.Remove(t);

        dpv1 = Vector2.Distance(v1,p);
        dpv2 = Vector2.Distance(v2,p);
        dpv3 = Vector2.Distance(v3,p);

        if(dpv1 > dpv2 && dpv1 > dpv3)
        {
            Debug.Log("entro en caso 1");
            triangle1 = new Triangle(v1,v2,p);
            triangle2 = new Triangle(v1,v3,p);
        }
        if(dpv2 > dpv1 && dpv2 > dpv3)
        {
            Debug.Log("entro en caso 2");

            triangle1 = new Triangle(v2,v1,p);
            triangle2 = new Triangle(v2,v3,p);
        }
        if(dpv3 > dpv2 && dpv3 > dpv1)
        {
            Debug.Log("entro en caso 3");

            triangle1 = new Triangle(v3,v2,p);
            triangle2 = new Triangle(v3,v1,p);
        }
        Triangulations.Add(triangle1);
        Triangulations.Add(triangle2);
    }

    bool InsideCircumferences(Triangle t, Vector2 p)
    {
       if(Vector2.Distance(t.getCenter(),p) < t.getRadius())
       {
           return true;
       } 
       return false;
    }

    //Check if a point is inside de triangle by comparing it with its vectors
    bool checkInsideTriangle(Triangle t, Vector2 p)
    {
        List<Vector2> vrts = t.getVertices();

        float s1,s2,s3,s4,w1,w2;

        s1 = vrts[2].y - vrts[0].y;
        s2 = vrts[2].x - vrts[0].x;
        s3 = vrts[1].y - vrts[0].y;
        s4 = p.y - vrts[0].y;

        w1 = (vrts[0].x * s1 + s4 * s2 - p.x * s1)/(s3 * s2 - (vrts[1].x - vrts[0].x) * s1);
        w2 = (s4 - w1 * s3)/s1;
        return w1 >= 0 && w2 >= 0 && (w1+w2) <= 1;
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

        Vector2 calculateCircumscribedCircumference()
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

        public Vector2 getCenter()
        {
            return centerCircumference;
        }

        public float getRadius()
        {
            return radius;
        }

        public List<Vector2> getVertices()
        {
            return vertices;
        }
    }

    void OnDrawGizmos()
    {
        for(int i = 0; i < Triangulations.Count; i++)
        {
            Gizmos.color = Color.black;

            Debug.Log("Triangulo" + i + " tiene como puntos: " + "(" + Triangulations[i].vertices[0][0] + "|" + Triangulations[i].vertices[0][1] +")("+ Triangulations[i].vertices[1][0] + "|" + Triangulations[i].vertices[1][1]+")("+ Triangulations[i].vertices[2][0] + "|" + Triangulations[i].vertices[2][1]+")");

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

