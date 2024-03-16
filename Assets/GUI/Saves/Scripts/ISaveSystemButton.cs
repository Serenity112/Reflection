using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ISaveSystemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected float expandTime = 0.05f;

    protected GameObject buttonParent;

    protected Animator animator;

    protected Vector3 origScale;
    protected Vector3 expandedScale;

    protected Vector3 parentOrigScale;
    protected Vector3 parentShrinkScale;

    public virtual void Awake()
    {
        animator = GetComponent<Animator>();
        buttonParent = transform.parent.gameObject;
        origScale = gameObject.GetComponent<RectTransform>().localScale;
        expandedScale = origScale * 1.1f;
        parentOrigScale = buttonParent.GetComponent<RectTransform>().localScale;
        parentShrinkScale = parentOrigScale * 0.85f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnterAction();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ExitAction();
    }

    public abstract void EnterAction();

    public abstract void ExitAction();

    public void BlockButton()
    {
        GetComponent<Button>().interactable = false;
    }

    public void UnlockButton()
    {
        GetComponent<Button>().interactable = true;
    }

    public virtual void ResetButtonState()
    {
        Awake();
        gameObject.GetComponent<RectTransform>().localScale = origScale;
        buttonParent.GetComponent<RectTransform>().localScale = parentOrigScale;
    }
}
