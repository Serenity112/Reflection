using System.Collections;
using System.Collections.Generic;

public static class SpriteScalesBase
{
    private static Dictionary<Character, float> _defaultScales;

    private static Dictionary<(Character, int), float> _specificScales;

    static SpriteScalesBase()
    {
        _defaultScales = new Dictionary<Character, float>()
        {
            { Character.Pasha, 0.4f },
            { Character.Tanya, 0.4f },
            { Character.Katya, 0.4f },
            { Character.Evelina, 0.4f },
            { Character.Nastya, 0.4f },

            { Character.Tumanov, 0.4f },
            { Character.Raketnikov, 0.4f },
        };

        _specificScales = new();
    }

    public static float GetCharacterScale(Character character)
    {
        if (_defaultScales.ContainsKey(character))
        {
            return _defaultScales[character];
        }
        else
        {
            return 1f;
        }
    }
}
