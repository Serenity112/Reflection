using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static MMButtonsManager;

public class MMOptionButton : IButtonGroup
{
    private float speed = 5f;

    [SerializeField] private MainMenuOption option;

    [HideInInspector]
    public GameObject spacing;

    [SerializeField] private AudioSource ScrollSource;
    [SerializeField] private AudioSource ClickSource;

    private IEnumerator _appear = null;
    private IEnumerator _disappear = null;

    public override void OnAwake()
    {
        spacing = transform.GetChild(0).gameObject;
        GetComponent<Button>().onClick.AddListener(OnClick);
        //GetComponent<Button>().onClick.AddListener(() => ClickSource.Play());
    }

    public override void EnterActioin()
    {
        if (_disappear != null)
        {
            StopCoroutine(_disappear);
        }

        _appear = FadeManager.FadeObject(spacing, true, speed);
        StartCoroutine(_appear);
    }

    public override void ExitActioin()
    {
        if (_appear != null)
        {
            StopCoroutine(_appear);
        }

        _disappear = FadeManager.FadeObject(spacing, false, speed);
        StartCoroutine(_disappear);
    }

    public override IEnumerator IClick()
    {
        MMButtonsManager.instance.DisableButtons();
        MMButtonsManager.instance.ExecuteOption(option);
        yield return null;
    }

    public override void RegisterManager()
    {
        SetManager(MMButtonsManager.instance);
    }
}
