using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutUsButtonManager : IButtonManager
{
    public static AboutUsButtonManager instance;

    public static bool IN_ABOUTUS_MENU { get; set; } = false;

    private void Awake()
    {
        instance = this;
    }

    public override void ResetManager()
    {
        ResetAllButtonsState();
    }
}
