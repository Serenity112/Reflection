using UnityEngine;

// Кнопка, которая увеличивается при наведении на неё
public abstract class IExpandableButton : IDraggableButton
{
    protected float expandTime = 0.05f;

    protected GameObject buttonParent;

    protected Animator animator;

    protected Vector3 origScale;
    protected Vector3 expandedScale;

    protected Vector3 parentOrigScale;
    protected Vector3 parentShrinkScale;

    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        buttonParent = transform.parent.gameObject;
        origScale = gameObject.GetComponent<RectTransform>().localScale;
        expandedScale = origScale * 1.1f;
        parentOrigScale = buttonParent.GetComponent<RectTransform>().localScale;
        parentShrinkScale = parentOrigScale * 0.85f;
    }

    public override void ResetButtonState()
    {
        gameObject.GetComponent<RectTransform>().localScale = origScale;
        buttonParent.GetComponent<RectTransform>().localScale = parentOrigScale;
        ResetFlags();
    }
}
