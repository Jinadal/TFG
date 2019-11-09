using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexWE : ScriptableObject
{
    public int id;
    public Vector3 Position;
    public List<EdgeWE> Edges = new List<EdgeWE>();

    public VertexWE() { }

    public void Init(float x, float y, float z)
    {
        Position = new Vector3(x,y,z);
    }
}
