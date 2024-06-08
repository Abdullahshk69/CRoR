using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitComparer : IComparer<Unit>
{
    public int Compare(Unit x, Unit y)
    {
        return y.GetSpeed().CompareTo(x.GetSpeed());
    }
}
