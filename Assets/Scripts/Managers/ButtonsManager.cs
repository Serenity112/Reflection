using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    public static ButtonsManager instance = null;

    public List<GameObject> underlinedPauseButtons = new List<GameObject>();

    public void unlinePauseButtons()
    {
        foreach(GameObject button in underlinedPauseButtons)
        {
            PauseOptionButton underlineButton = button.GetComponent<PauseOptionButton>();
            underlineButton.StopAllCoroutines();
            GameObject spacing = underlineButton.spacing;
            spacing.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }
    void Start()
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
