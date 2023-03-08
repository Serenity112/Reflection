using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMButtonsManager : MonoBehaviour
{
    public static MMButtonsManager instance = null;

    public List<GameObject> underlinedButtons = new List<GameObject>();

    public void unlineButtons()
    {
        foreach (GameObject button in underlinedButtons)
        {
            MMOptionButton underlineButton = button.GetComponent<MMOptionButton>();
            underlineButton.StopAllCoroutines();
            GameObject spacing = underlineButton.spacing;
            spacing.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
    }
}
