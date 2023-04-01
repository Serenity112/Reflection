using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPanel : MonoBehaviour
{
    public static ConfirmationPanel instance = null;

    [SerializeField]
    private GameObject Panel;

    [SerializeField]
    private GameObject Title;

    [SerializeField]
    private float speed;

    private IEnumerator IYes;
    private IEnumerator INo;

    void Start()
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

    public static IEnumerator CreatePanel(string title, IEnumerator YesAction, IEnumerator NoAction)
    {
        instance.IYes = YesAction;
        instance.INo = NoAction;
        instance.Title.GetComponent<Text>().text = title;

        instance.Panel.SetActive(true);

        yield return instance.StartCoroutine(FadeManager.FadeObject(instance.Panel, true, instance.speed));
    }

    public static IEnumerator ClosePanel()
    {
        StaticVariables.ConfirmationPanelActive = false;
        yield return instance.StartCoroutine(FadeManager.FadeObject(instance.Panel, false, instance.speed));
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
