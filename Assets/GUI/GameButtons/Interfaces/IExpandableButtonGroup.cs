using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class IExpandableButtonGroup : IButtonGroup
{
    protected float speed = 5f;
    protected float expandTime = 0.05f;

    protected GameObject buttonParent;

    protected Animator animator;

    protected Vector3 origScale;
    protected Vector3 expandedScale;

    protected Vector3 parentOrigScale;
    protected Vector3 parentShrinkScale;

    public IExpandableButtonGroup() : base()
    {
        OnAwakeActions(new List<Action>
         {
             delegate { animator = GetComponent<Animator>(); },
             delegate { buttonParent = transform.parent.gameObject; },
             delegate { origScale = gameObject.GetComponent<RectTransform>().localScale; },
             delegate { expandedScale = origScale * 1.1f; },
             delegate { parentOrigScale = buttonParent.GetComponent<RectTransform>().localScale; },
             delegate { parentShrinkScale = parentOrigScale * 0.85f; }
         });
    }
}
