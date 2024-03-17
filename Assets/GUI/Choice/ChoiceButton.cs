using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : IExpandableButton
{
    private string _blockName;
    private int _optionNumber;

    private IEnumerator _shrinkOnExit;
    private IEnumerator _expandOnEnter;

    public override void Awake()
    {
        base.Awake();

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void SetOptions(string message, string blockName, int number)
    {
        _blockName = blockName;
        _optionNumber = number;
        gameObject.transform.GetChild(0).GetComponent<Text>().text = message;
    }

    public override void EnterAction()
    {
        if (_shrinkOnExit != null)
            StopCoroutine(_shrinkOnExit);
        _expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, expandTime);
        StartCoroutine(_expandOnEnter);
    }

    public override void ExitAction()
    {
        if (_expandOnEnter != null)
            StopCoroutine(_expandOnEnter);
        _shrinkOnExit = ExpandManager.ExpandObject(gameObject, origScale, expandTime);
        StartCoroutine(_shrinkOnExit);
    }

    public override IEnumerator IClick()
    {
        yield return StartCoroutine(ChoiceManager.instance.LoadChoise(_blockName, _optionNumber));
    }

    public override void ResetButtonState()
    {
        base.ResetButtonState();
    }
}
