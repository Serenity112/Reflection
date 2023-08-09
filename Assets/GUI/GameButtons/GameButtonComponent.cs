using UnityEngine;

public class GameButtonComponent : MonoBehaviour
{
    [SerializeField] private IDraggableButton GameButton;

    public void AppearIfEntered()
    {
        GameButton.AppearIfEntered();
    }
}
