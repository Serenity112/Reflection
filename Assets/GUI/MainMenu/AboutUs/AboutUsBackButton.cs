using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AboutUsBackButton : MonoBehaviour
{
    private Animator animator;

    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnEnter;

    private GameObject buttonParent;

    private Vector3 origScale;
    private Vector3 expandedScale;

    void Start()
    {
        animator = GetComponent<Animator>();

        buttonParent = transform.parent.gameObject;

        origScale = gameObject.GetComponent<RectTransform>().localScale;
        expandedScale = origScale * 1.1f;
    }

    void OnMouseEnter()
    {
        if (shrinkOnEnter != null)
            StopCoroutine(shrinkOnEnter);
        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, 0.05f);
        StartCoroutine(expandOnEnter);
    }

    void OnMouseExit()
    {
        if (expandOnEnter != null)
            StopCoroutine(expandOnEnter);
        shrinkOnEnter = ExpandManager.ExpandObject(gameObject, origScale, 0.05f);
        StartCoroutine(shrinkOnEnter);
    }

    public void Click()
    {
        StartCoroutine(IClick());
    }

    public IEnumerator IClick()
    {
        GetComponent<Button>().interactable = false;

        animator.Play("Back");
        MMPanelsManager.instance.CloseInfoMenu();

        Vector3 currParentScale = buttonParent.GetComponent<RectTransform>().localScale;        
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, 0.85f, 0.05f));
       
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, currParentScale, 0.05f));

        GetComponent<Button>().interactable = true;
    }
}
