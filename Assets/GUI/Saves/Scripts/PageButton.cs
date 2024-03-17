using UnityEngine;

public class PageButton : MonoBehaviour
{
    [SerializeField] private SavePageScroll side;
    public void Scroll()
    {
        if (StaticVariables.IN_SAVE_MENU &&
                !StaticVariables.GAME_IS_LOADING &&
                !StaticVariables.OVERLAY_ACTIVE &&
                !SaveManagerStatic.ClickBlocker &&
                !SaveManagerStatic.UiBloker)
        {
            SaveManager.instance.ScrollPage(side);
        }
        else
        {
        }
    }
}