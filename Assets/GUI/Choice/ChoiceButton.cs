using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    private string _blockName;

    private IEnumerator _shrinkOnExit;
    private IEnumerator _expandOnEnter;

    private Vector3 _origScale;
    private Vector3 _expandedScale;
    private float _expandTime = 0.05f;

    private void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(LoadBlock);

        _origScale = GetComponent<RectTransform>().localScale;
        _expandedScale = _origScale * 1.1f;
    }

    public void SetOptions(string message, string blockName)
    {
        _blockName = blockName;
        gameObject.transform.GetChild(0).GetComponent<Text>().text = message;
    }

    public void LoadBlock()
    {
        StartCoroutine(ChoiceManager.instance.GetComponent<ChoiceManager>().LoadChoise(_blockName));
    }

    private void OnMouseEnter()
    {
        if (_shrinkOnExit != null)
            StopCoroutine(_shrinkOnExit);
        _expandOnEnter = ExpandManager.ExpandObject(gameObject, _expandedScale, _expandTime);
        StartCoroutine(_expandOnEnter);
    }

    private void OnMouseExit()
    {
        if (_expandOnEnter != null)
            StopCoroutine(_expandOnEnter);
        _shrinkOnExit = ExpandManager.ExpandObject(gameObject, _origScale, _expandTime);
        StartCoroutine(_shrinkOnExit);
    }
}
