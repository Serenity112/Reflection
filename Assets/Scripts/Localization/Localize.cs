using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localize : MonoBehaviour
{
    public void LocalizeIt()
    {
        LocalizedText[] objects = GameObject.FindObjectsOfType<LocalizedText>();
        foreach (LocalizedText obj in objects)
        {
            obj.Localize(obj.GetComponent<LocalizedText>().publicKey);
        }
    }
}
