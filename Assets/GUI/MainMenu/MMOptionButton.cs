using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MMOptionButton : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    private float speed = 5f;

    [HideInInspector]
    public GameObject spacing;

    [SerializeField] private AudioSource ScrollSource;
    [SerializeField] private AudioSource ClickSource;

    private IEnumerator _appear = null;
    private IEnumerator _disappear = null;

    private bool dragging = false;
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

    private void OnMouseEnter()
    {
        enter = true;

        if (!StaticVariables.OverlayPanelActive && !dragging)
        {
            //ScrollSource.Play();
            Appear();
        }
    }

    private void OnMouseExit()
    {
        enter = false;

        if (!StaticVariables.OverlayPanelActive && !dragging)
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

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;

        if (!enter)
        {
            Disappear();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;  
    }
}
