using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Shared {

    /*public Dictionary<string, int> SkillLevels = new Dictionary<string, int>
    {
        {"cooking", 0 },
        {"navigation", 0},
        {"medical", 0 },
        {"fighting", 0 }

    };*/

   public struct Skills
    {
        int cooking;
        int navigation;
        int medical;
        int fighting;
    }

    public enum Roles
    {
        unassigned,
        pilot,
        cook,
        farmer,
        explorer,
        lifesupport
    }

}
