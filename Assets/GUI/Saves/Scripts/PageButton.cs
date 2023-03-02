using UnityEngine;

public class PageButton : MonoBehaviour
{
    [SerializeField] private SavePageScroll side;
    public void Scroll()
    {
        SaveManager.instance.ScrollPage(side);
    }
}