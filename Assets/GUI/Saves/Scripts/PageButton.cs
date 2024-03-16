using UnityEngine;

public class PageButton : MonoBehaviour
{
    [SerializeField] private SavePageScroll side;
    public void Scroll()
    {
        if (StaticVariables.IN_SAVE_MENU &&
                !StaticVariables.GAME_IS_LOADING &&
                !StaticVariables.OVERLAY_ACTIVE &&
                !SaveManagerStatic.ClickBlocker)
        {          
            SaveManager.instance.ScrollPage(side);
        } else
        {
            Debug.Log(StaticVariables.IN_SAVE_MENU);
            Debug.Log(StaticVariables.GAME_IS_LOADING);
            Debug.Log(StaticVariables.OVERLAY_ACTIVE);
            Debug.Log(SaveManagerStatic.ClickBlocker);
        }
    }
}