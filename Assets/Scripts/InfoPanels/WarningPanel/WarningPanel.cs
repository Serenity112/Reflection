using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class WarningPanel : MonoBehaviour
{
    public static WarningPanel instance = null;

    [SerializeField]
    private float FadingSpeed;

    private GameObject ActivePanels;

    public static string SavingErrorMessage =
       "В системе сохранений возникла ошибка!\n" +
       "Некоторые элементы игры могут быть сброшены и/или работать нестабильно.\n" +
       "Если вы не взаимодействовали с файлами игры, просим вас сообщить об ошибке разработчикам.\n" +
       "Приносим извинения за доставленные неудобства!";

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

    private void OnEnable()
    {
        ActivePanels = PanelsConfig.CurrentManager.GetActivePanelsParent();
    }

    public void CreateWarningPanel(string title) => StartCoroutine(ICreateWarningPanel(title));

    private IEnumerator ICreateWarningPanel(string title)
    {
        StaticVariables.OverlayPanelActive = true;

        handler = Addressables.InstantiateAsync("WarningPanel", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return handler;

        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject result = handler.Result;
            result.name = "WarningPanel";
            result.transform.GetChild(1).GetComponent<Text>().text = title;
        }
        else
        {
            Debug.Log("Error loading");
        }
    }

    public void CloseWarningPanel()
    {
        StaticVariables.OverlayPanelActive = false;

        Addressables.ReleaseInstance(handler);

        Resources.UnloadUnusedAssets();
    }
}
