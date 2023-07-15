using System.Collections.Generic;
using UnityEngine;

public class MMButtonsManager : MonoBehaviour
{
    public static MMButtonsManager instance = null;

    public List<GameObject> MainMenuOptionButtons;

    public void DisableColliders()
    {
        foreach (GameObject button in MainMenuOptionButtons)
        {
            BoxCollider collider = button.GetComponent<BoxCollider>();
            collider.enabled = false;
        }
    }

    public void EnableColliders()
    {
        foreach (GameObject button in MainMenuOptionButtons)
        {
            BoxCollider collider = button.GetComponent<BoxCollider>();
            collider.enabled = true;
        }
    }

    public void UnlineButtons()
    {
        foreach (GameObject button in MainMenuOptionButtons)
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

        MainMenuOptionButtons = new List<GameObject>();
    }
}
