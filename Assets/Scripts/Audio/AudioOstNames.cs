using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioManager;

public class AudioOstNames
{
    public static Dictionary<AudioLine, Dictionary<string, string>> OstNames;

    private static Dictionary<string, string> MusicNames;

    private static Dictionary<string, string> AmbientNames;

    private static Dictionary<string, string> SoundNames;

    static AudioOstNames()
    {
        MusicNames = new()
        {
            { "AdventureTendency", "King Of Eternity - Adventure tendency" },
            { "FictionalPlanets", "King Of Eternity - Fictional planets"},
            { "MiddayWorld", "King Of Eternity - Midday world"},
            { "PioneersSpirit", "King Of Eternity - Pioneers spirit"},
            { "ForgottenFuture", "King Of Eternity - Forgotten future"},
            { "NothingOddAboutIt", "No Time For Electric Bananas - Nothing odd about it"},
            { "FarewellToUnknown", "No Time For Electric Bananas - Farewell to unknown"},
        };

        AmbientNames = new();
        SoundNames = new();

        OstNames = new()
        {
            { AudioLine.Music, MusicNames},
            { AudioLine.Ambient, AmbientNames},
            { AudioLine.Sound, SoundNames}
        };
    }

    public static string GetDisplayableName(string key)
    {
        if (MusicNames.ContainsKey(key))
        {
            return MusicNames[key];
        }
        else
        {
            return "[Имя трека]";
        }
    }
}
