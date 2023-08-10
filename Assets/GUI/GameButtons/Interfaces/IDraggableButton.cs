using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// Абстрактный класс, описывающий кнопку, на которую можно нажать и потянуть
public abstract class IDraggableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    protected bool pointer_down = false;
    protected bool enter = false;

    public abstract void EnterAction();

    public abstract void ExitAction();

    public abstract IEnumerator IClick();

    public void OnClick() => StartCoroutine(IClick());

    public virtual bool PointerEnterCondition() { return true; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        enter = true;

        if (!StaticVariables.OverlayPanelActive && !pointer_down && PointerEnterCondition())
        {
            EnterAction();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        enter = false;

        if (!StaticVariables.OverlayPanelActive && !pointer_down)
        {
            ExitAction();
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
            ExitAction();
        }
    }

    public virtual void AppearIfEntered()
    {
        if (!StaticVariables.OverlayPanelActive && enter)
        {
            EnterAction();
        }
    }
}
