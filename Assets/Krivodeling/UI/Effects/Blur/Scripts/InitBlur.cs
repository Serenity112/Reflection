using Krivodeling.UI.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitBlur : MonoBehaviour
{
    void Start()
    {
        GetComponent<UIBlur>().BeginBlur(2f);
    }
}
