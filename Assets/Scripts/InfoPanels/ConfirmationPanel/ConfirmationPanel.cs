using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ConfirmationPanel : MonoBehaviour
{
    public static ConfirmationPanel instance = null;

    [SerializeField]
    private float FadingSpeed;

    private IEnumerator IYes;
    private IEnumerator INo;

    private bool _panelOpen = false;

    private GameObject ActivePanels;

    private GameObject Panel;

    private AsyncOperationHandle<GameObject> handler;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ActivePanels = PanelsConfig.CurrentManager.GetActivePanelsParent();
    }

    public IEnumerator CreateConfirmationPanel(string title, IEnumerator YesAction, IEnumerator NoAction)
    {
        StaticVariables.OverlayPanelActive = true;

        if (!_panelOpen)
        {
            _panelOpen = true;

            handler = Addressables.InstantiateAsync("ConfirmationPanel", ActivePanels.GetComponent<RectTransform>(), false, true);
            yield return handler;

            if (handler.Status == AsyncOperationStatus.Succeeded)
            {
                Panel = handler.Result;
                Panel.GetComponent<CanvasGroup>().alpha = 0f;
                Panel.name = "ConfirmationPanel";

                IYes = YesAction;
                INo = NoAction;

                Panel.GetComponent<Confirmation>().SetTitle(title);

                yield return instance.StartCoroutine(FadeManager.FadeOnly(Panel, true, FadingSpeed));
            }
            else
            {
                Debug.Log("Error loading");
            }
        }
    }

    public IEnumerator ClosePanel()
    {
        yield return StartCoroutine(FadeManager.FadeOnly(Panel, false, FadingSpeed));

        _panelOpen = false;

        StaticVariables.OverlayPanelActive = false;

        Addressables.ReleaseInstance(handler);

        Resources.UnloadUnusedAssets();
    }

    public void ChooseYes()
    {
        StartCoroutine(IYes);
    }

    public void ChooseNo()
    {
        StartCoroutine(INo);
    }
}
