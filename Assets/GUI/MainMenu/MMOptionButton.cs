using UnityEngine;
using UnityEngine.UI;

public class MMOptionButton : MonoBehaviour
{
    private float speed = 10f;

    [HideInInspector]
    public GameObject spacing;

    [SerializeField] private AudioSource ScrollSource;
    [SerializeField] private AudioSource ClickSource;

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
        if (!StaticVariables.OverlayPanelActive)
        {
            //ScrollSource.Play();
            StopAllCoroutines();
            StartCoroutine(FadeManager.FadeObject(spacing, true, speed));
        }
    }

    private void OnMouseExit()
    {
        if (!StaticVariables.OverlayPanelActive)
        {
            StopAllCoroutines();
            StartCoroutine(FadeManager.FadeObject(spacing, false, speed));
        }
    }
}
