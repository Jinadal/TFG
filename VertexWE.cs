using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VertexWE : ScriptableObject, IComparable 
{
    public int id;
    public Vector3 Position;
    public List<EdgeWE> Edges = new List<EdgeWE>();

    public VertexWE() { }

    public void Init(float x, float y, float z)
    {
        Position = new Vector3(x,y,z);
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        VertexWE otherVertex = obj as VertexWE;
        if (otherVertex != null)
            return this.Position.z.CompareTo(otherVertex.Position.z);
        else
            throw new ArgumentException("Object is not a Temperature");
    }
}
