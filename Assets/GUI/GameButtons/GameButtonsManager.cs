using UnityEngine;

public class GameButtonsManager : MonoBehaviour
{
    public static GameButtonsManager instance = null;

    private void Awake()
    {
        instance = this;

        GameButtons = this.gameObject;
    }

    public GameObject GameButtons;

    public GameObject PauseButton;

    public GameObject SkipButton;

    public GameObject HideButton;

    public GameObject LogButton;

    public GameObject ContinueGame;
}
