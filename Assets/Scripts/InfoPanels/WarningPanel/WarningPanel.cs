using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class WarningPanel : MonoBehaviour
{
    public static WarningPanel instance = null;

    [HideInInspector] public GameObject ActivePanels;

    private float FadingSpeed = 5f;

    public static string SavingErrorMessage =
       "� ������� ���������� �������� ������!\n" +
       "��������� �������� ���� ����� ���� �������� �/��� �������� �����������.\n" +
       "���� �� �� ����������������� � ������� ����, ������ ��� �������� �� ������ �������������.\n" +
       "�������� ��������� �� ������������ ����������!";

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
    }

    public void CreateWarningPanel(string title) => StartCoroutine(ICreateWarningPanel(title));

    private IEnumerator ICreateWarningPanel(string title)
    {
        if (StaticVariables.OverlayPanelActive)
        {
            yield break;
        }

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
