using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// Абстрактный класс, описывающий кнопку, на которую можно нажать и потянуть
public abstract class IDraggableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    protected bool pointer_down = false;
    protected bool enter = false;

    public virtual void Awake()
    {

    }

    public abstract void EnterAction();

    public abstract void ExitAction();

    public abstract IEnumerator IClick();

    public void OnClick() => StartCoroutine(IClick());

    protected void ResetFlags()
    {
        pointer_down = false;
        enter = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        enter = true;

        if (!StaticVariables.OverlayPanelActive && !pointer_down)
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

    public void OnPointerDown(PointerEventData eventData)
    {
        pointer_down = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointer_down = false;

        if (!enter)
        {
            ExitAction();
        }
    }

    public abstract void ResetButtonState();
}
