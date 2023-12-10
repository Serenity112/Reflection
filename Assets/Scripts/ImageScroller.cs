using UnityEngine;
using UnityEngine.UI;

public class ImageScroller : MonoBehaviour
{
    public RawImage image;
    public float x, y;

    private void Update()
    {
        if (SettingsConfig.chosenOptions[SettingsList.GuiAnimation].data == 1)
        {
            image.uvRect = new Rect(image.uvRect.position + new Vector2(x, y) * Time.deltaTime, image.uvRect.size);
        }
    }
}
