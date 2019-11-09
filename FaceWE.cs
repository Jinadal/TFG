using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceWE : ScriptableObject
{
    public List<EdgeWE> Edges;
    public FaceWE()
    {
        Edges = new List<EdgeWE>();
    }
}
