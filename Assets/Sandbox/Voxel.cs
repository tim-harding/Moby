using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Voxel
{
    public bool Mode;
    public int Base;
    public int XEdge;
    public int YEdge;

    public void ResetIndices()
    {
        Base = -1;
        XEdge = -1;
        YEdge = -1;
    }

    public Voxel(bool mode)
    {
        Mode = mode;
        Base = -1;
        YEdge = -1;
        XEdge = -1;
    }
}