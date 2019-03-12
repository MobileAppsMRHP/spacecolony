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

   /*public struct Skills
    {
        int cooking;
        int navigation;
        int medical;
        int fighting;
    }*/

    public enum Roles
    {
        unassigned = 0,
        pilot,
        cook,
        farmer,
        explorer,
        lifesupport
    }

    public enum RoomTypes
    {
        empty = 0,
        bridge,
        energy,
        food
    }

    public enum ResourceTypes
    {
        scraps,
        money,
        energy,
        preciousMetal,
        premiumCurrency
    }

}
