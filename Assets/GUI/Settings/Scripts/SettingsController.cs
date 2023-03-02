using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour
{   
    private void Start()
    {
        SettingsConfig.UpdateAllVisuals();
    }

    public void CloseSettings()
    {
        SettingsConfig.unSubscribeAllOptions();

        SettingsConfig.currentManager.CloseSettings();
    }
}
