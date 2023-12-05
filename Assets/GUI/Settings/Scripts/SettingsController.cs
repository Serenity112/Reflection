using UnityEngine;

public class SettingsController : MonoBehaviour
{
    public SettingsBackButton settingsBackButton;

    public void InitialReset()
    {
        SettingsConfig.InitialUpdateAllVisuals();

        settingsBackButton.ResetButtonState();
    }
}
