using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOstNames : MonoBehaviour
{
    public static Dictionary<string, string> OstNames;

    private void Awake()
    {
        OstNames = new()
        {
            { "AdventureTendency", "King Of Eternity - Adventure tendency" },
            { "FictionalPlanets", "King Of Eternity - Fictional planets"},
            { "MiddayWorld", "King Of Eternity - Midday world"},
            { "PioneersSpirit", "King Of Eternity - Pioneers spirit"},
            { "ForgottenFuture", "King Of Eternity - Forgotten future"},
            { "NothingOddAboutIt", "No Time For Electric Bananas - Nothing odd about it"},
            { "FarewellToUnknown", "No Time For Electric Bananas - Farewell to unknown"},
        };
    }

    public static string GetDisplayableName(string key)
    {
        if (OstNames.ContainsKey(key))
        {
            return OstNames[key];
        }
        else
        {
            return "%0ÿ»· 4%";
        }
    }
}
