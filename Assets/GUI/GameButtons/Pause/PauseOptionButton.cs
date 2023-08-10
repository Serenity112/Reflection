using UnityEngine;

public class PauseOptionButton : MonoBehaviour
{
    [SerializeField]
    private float speed;

    public GameObject spacing;    
    void Start()
    {
        spacing = transform.GetChild(0).gameObject;

        PauseButtonsManager.instance.underlinedPauseButtons.Add(gameObject);
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
