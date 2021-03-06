﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class InputInputConnection : Wire
{
    private CircuitInput RealInput1;
    private CircuitInput RealInput2;
    public CircuitInput Input1
    {
        get { return RealInput1; }
        set
        {
            if (value == null) { Debug.LogError("Tried to set input of II connection to null. Destroying wire"); Destroy(gameObject); return; }
            RealInput1 = value;
            Point1 = GetWireReference(value.transform);
            value.IIConnections.Add(this);
        }
    }
    public CircuitInput Input2
    {
        get { return RealInput2; }
        set
        {
            if (value == null) { Debug.LogError("Tried to set input of II connection to null. Destroying wire"); Destroy(gameObject); return; }
            RealInput2 = value;
            Point2 = GetWireReference(value.transform);
            value.IIConnections.Add(this);
        }
    }

    public override void SetPegsBasedOnPoints()
    {
        Input1 = Point1.parent.GetComponent<CircuitInput>();
        Input2 = Point2.parent.GetComponent<CircuitInput>();
    }
}
