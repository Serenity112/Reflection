using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbuotUsSupportButton : IDraggableButton
{
    public enum SupportLink
    {
        VK,
        Boosty,
        Patreon
    }

    public static Dictionary<SupportLink, string> Links = new Dictionary<SupportLink, string>()
    {
        { SupportLink.VK, "https://vk.com/white_rainbow_digital" },
        { SupportLink.Boosty, "https://vk.com/white_rainbow_digital" },
        { SupportLink.Patreon, "https://vk.com/white_rainbow_digital" },
    };

    [SerializeField] private SupportLink Link;

    private static Color Clicked = new Color(0.62f, 0.38f, 0.37f, 1f);
    private static Color Hide = new Color(0.62f, 0.38f, 0.37f, 0f);
    private IEnumerator shadeIn;
    private IEnumerator shadeOut;
    private float speed = 5f;

    public override void Awake()
    {
        base.Awake();

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Start()
    {
        AboutUsButtonManager.instance.SubscribeButton(this);
    }

    public override void EnterAction()
    {
        if (shadeOut != null)
        {
            StopCoroutine(shadeOut);
        }

        shadeIn = FadeManager.FadeTextToColor(GetComponent<Text>(), Clicked, speed);
        StartCoroutine(shadeIn);
    }

    public override void ExitAction()
    {
        if (shadeIn != null)
        {
            StopCoroutine(shadeIn);
        }

        shadeOut = FadeManager.FadeTextToColor(GetComponent<Text>(), Hide, speed);
        StartCoroutine(shadeOut);
    }

    public override IEnumerator IClick()
    {
        Application.OpenURL(Links[Link]);
        yield return null;
    }

    public override void ResetButtonState()
    {
        GetComponent<Text>().color = Hide;
    }
}
