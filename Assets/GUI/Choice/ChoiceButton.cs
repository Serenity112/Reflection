using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    private string _blockName;

    private void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(LoadBlock);
    }

    public void SetOptions(string message, string blockName)
    {
        _blockName = blockName;
        gameObject.transform.GetChild(0).GetComponent<Text>().text = message;
    }

    public void LoadBlock()
    {
        Debug.Log("Loading block");
        StartCoroutine(ChoiceManager.instance.GetComponent<ChoiceManager>().LoadChoise(_blockName));
    }
}
