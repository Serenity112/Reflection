using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCrossButton : MonoBehaviour
{
    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnExit;

    private IEnumerator CrossFadeIn;
    private IEnumerator CrossFadeOut;

    private GameObject buttonParent;

    private Vector3 origScale;
    private Vector3 expandedScale;

    void Start()
    {
        buttonParent = transform.parent.gameObject;

        origScale = gameObject.GetComponent<RectTransform>().localScale;
        expandedScale = origScale * 1.1f;

        GetComponent<Button>().onClick.AddListener(OnClickAnimation);
    }

    void OnMouseEnter()
    {
        if (shrinkOnExit != null)
            StopCoroutine(shrinkOnExit);
        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, 0.05f);
        StartCoroutine(expandOnEnter);

        AppearCross();
    }

    void OnMouseExit()
    {
        if (expandOnEnter != null)
            StopCoroutine(expandOnEnter);
        shrinkOnExit = ExpandManager.ExpandObject(gameObject, origScale, 0.05f);
        StartCoroutine(shrinkOnExit);

        DisappearCross();
    }

    public void AppearCross()
    {
        if (!StaticVariables.ConfirmationPanelActive)
        {
            if (CrossFadeOut != null)
                StopCoroutine(CrossFadeOut);

            CrossFadeIn = FadeManager.FadeOnly(gameObject, true, SaveManager.instance.optionsGradientSpeed);
            StartCoroutine(CrossFadeIn);
        }
    }

    public void DisappearCross()
    {
        if (!StaticVariables.ConfirmationPanelActive)
        {
            if (CrossFadeIn != null)
                StopCoroutine(CrossFadeIn);

            CrossFadeOut = FadeManager.FadeOnly(gameObject, false, SaveManager.instance.optionsGradientSpeed);
            StartCoroutine(CrossFadeOut);
        }
    }

    private void OnClickAnimation()
    {
        StartCoroutine(IOnClickAnimation());
    }

    private IEnumerator IOnClickAnimation()
    {
        GetComponent<Button>().interactable = false;
        Vector3 currParentScale = buttonParent.GetComponent<RectTransform>().localScale;
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, 0.85f, 0.05f));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, currParentScale, 0.05f));
        GetComponent<Button>().interactable = true;
    }
}
