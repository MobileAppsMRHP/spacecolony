using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Shared {

    //consider the user to have been logged off after this much delta time has passed
    public static readonly float ProcessElapsedTime_ConsiderLoggedOff = 20.0f; // 20 seconds
    public static readonly string PlayerPrefs_AuthTokenKey = "UserAuthToken";
    public static readonly int NewUser_StartingCrewCount = 2;

    public enum Roles
    {
        unassigned = 0,
        pilot,
        cook,
        farmer,
        explorer,
        lifesupport
    }

    public enum RoomTypes //don't reorder, might break database
    {
        empty = 0,
        bridge,
        energy,
        food,
        mineral,
        water
    }

    public enum ResourceTypes
    {
        minerals,
        food,
        water,
        money,
        energy,
        preciousMetal,
        premiumCurrency
    }

    public enum Planets
    {
        Loading = 0,
        MineralPlanet,
        FoodPlanet,
        WaterPlanet,
        MoneyPlanet, //probably don't use this; here for consistency
        EnergyPlanet,//probably don't use this; here for consistency
        PreciousMetalPlanet,
        PremiumCurrencyPlanet//probably don't use this; here for consistency

    }

}
