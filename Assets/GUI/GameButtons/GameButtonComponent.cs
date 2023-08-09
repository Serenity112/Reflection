using UnityEngine;

public class GameButtonComponent : MonoBehaviour
{
    [SerializeField] private IButtonGroup GameButton;

    public void AppearIfEntered()
    {
        GameButton.AppearIfEntered();
    }
}
