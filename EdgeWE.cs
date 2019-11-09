using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeWE : ScriptableObject
{
    public VertexWE Vertex1;
    public VertexWE Vertex2;

    public FaceWE LeftFace;
    public FaceWE RightFace;

    // Clockwise ordering
    //public EdgeWE PreviousLeft;
    //public EdgeWE PreviousRight;

    //public EdgeWE NextLeft;
    //public EdgeWE NextRight;

    public EdgeWE() { }

    public void Init(VertexWE v1, VertexWE v2)
    {
        Vertex1 = v1;
        Vertex2 = v2;
    }
    
}
