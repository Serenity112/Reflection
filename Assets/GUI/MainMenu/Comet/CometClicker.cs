using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CometClicker : MonoBehaviour
{
    private AudioSource source;

    private int counter = 0;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        source = GetComponent<AudioSource>();
    }

    private void OnClick()
    {
        counter++;

        if (counter == 15)
        {
            StartCoroutine(IPlay());
        }
    }

    private IEnumerator IPlay()
    {
        source.Play();

        yield return new WaitForSeconds(20f);

        source.Stop();
    }
}
