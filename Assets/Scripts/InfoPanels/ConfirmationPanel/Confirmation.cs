using UnityEngine;
using UnityEngine.UI;

public class Confirmation : MonoBehaviour
{
    [SerializeField]
    private GameObject Title;

    [SerializeField]
    private ConfirmationPanelButton YesButton;

    [SerializeField]
    private ConfirmationPanelButton NoButton;

    public void ResetButtons()
    {
        YesButton.ResetButtonState();
        NoButton.ResetButtonState();
    }

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
