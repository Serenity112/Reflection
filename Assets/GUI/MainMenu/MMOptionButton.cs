using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MMOptionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private float speed = 5f;

    [HideInInspector]
    public GameObject spacing;

    [SerializeField] private AudioSource ScrollSource;
    [SerializeField] private AudioSource ClickSource;

    private IEnumerator _appear = null;
    private IEnumerator _disappear = null;

    private bool pointer_down = false;
    private bool enter = false;

    private void Awake()
    {
        spacing = transform.GetChild(0).gameObject;
        //GetComponent<Button>().onClick.AddListener(() => ClickSource.Play());
    }

    void Start()
    {
        MMButtonsManager.instance.MainMenuOptionButtons.Add(gameObject);
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
            Disappear();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        enter = true;

        if (!StaticVariables.OverlayPanelActive && !pointer_down)
        {
            //ScrollSource.Play();
            Appear();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        enter = false;

        if (!StaticVariables.OverlayPanelActive && !pointer_down)
        {
            Disappear();
        }
    }

    private void Appear()
    {
        if (_disappear != null)
        {
            StopCoroutine(_disappear);
        }

        _appear = FadeManager.FadeObject(spacing, true, speed);
        StartCoroutine(_appear);
    }

    private void Disappear()
    {
        if (_appear != null)
        {
            StopCoroutine(_appear);
        }

        _disappear = FadeManager.FadeObject(spacing, false, speed);
        StartCoroutine(_disappear);
    }
}
