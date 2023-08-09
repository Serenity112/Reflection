using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// Абстрактный класс, описывающий кнопку, на которую можно нажать и потянуть
public abstract class IDraggableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    protected bool pointer_down = false;
    protected bool enter = false;

    public abstract void EnterActioin();

    public abstract void ExitActioin();

    public abstract IEnumerator IClick();

    public void OnClick() => StartCoroutine(IClick());

    public virtual bool PointerEnterCondition() { return true; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        enter = true;

        if (!StaticVariables.OverlayPanelActive && !pointer_down && PointerEnterCondition())
        {
            EnterActioin();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        enter = false;

        if (!StaticVariables.OverlayPanelActive && !pointer_down)
        {
            ExitActioin();
        }
    }

    public virtual void PrePointerDown() { }

    public void OnPointerDown(PointerEventData eventData)
    {
        PrePointerDown();

        pointer_down = true;
    }

    public virtual void PrePointerUp() { }

    public void OnPointerUp(PointerEventData eventData)
    {
        PrePointerUp();

        pointer_down = false;

        if (!enter)
        {
            ExitActioin();
        }
    }

    public virtual void AppearIfEntered()
    {
        if (!StaticVariables.OverlayPanelActive && enter)
        {
            EnterActioin();
        }
    }
}
