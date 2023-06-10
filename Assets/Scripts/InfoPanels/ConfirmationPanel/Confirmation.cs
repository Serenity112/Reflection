using UnityEngine;
using UnityEngine.UI;

public class Confirmation : MonoBehaviour
{
    [SerializeField]
    private GameObject Title;

    public void SetTitle(string title)
    {
        Title.GetComponent<Text>().text = title;
    }

    public void ChooseYes()
    {
        ConfirmationPanel.instance.ChooseYes();
    }

    public void ChooseNo()
    {
        ConfirmationPanel.instance.ChooseNo();
    }
}
