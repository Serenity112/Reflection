using UnityEngine;

public class MMOptionButton : MonoBehaviour
{
    [SerializeField]
    private float speed;

    public GameObject spacing;
    void Start()
    {
        spacing = transform.GetChild(0).gameObject;

        MMButtonsManager.instance.underlinedButtons.Add(gameObject);
    }

    private void OnMouseEnter()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeObject(spacing, true, speed));
    }

    private void OnMouseExit()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeObject(spacing, false, speed));
    }
}
