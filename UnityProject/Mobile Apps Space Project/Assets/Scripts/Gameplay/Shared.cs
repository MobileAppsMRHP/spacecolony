using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Shared {

   public struct Skills
    {
        int cooking;
        int navigation;
        int medical;
        int fighting;
    }

    public enum roles
    {
        unassigned,
        pilot,
        cook,
        farmer,
        explorer,
        lifesupport
    }

}
