using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLoadButton : ISaveSystemButton
{
    [SerializeField] private SaveFileFields saveFileFields;
    [SerializeField] private GameObject Cassette;

    private DeleteCrossButton DeleteCross;
    private MainMenuLoad mainMenuLoad;

    private IEnumerator CassetteFadeIn;
    private IEnumerator CassetteFadeOut;

    public override void Awake()
    {
        base.Awake();

        mainMenuLoad = saveFileFields._MainMenuLoad;
        DeleteCross = mainMenuLoad.DeleteCross;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(delegate
        {
            StartCoroutine(IClick());
        });
    }

    private IEnumerator IClick()
    {
        if (!SaveManagerStatic.ClickBlocker)
        {
            SaveManagerStatic.ClickBlocker = true;
            SaveManagerStatic.UiBloker = true;

            yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
            yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));

            mainMenuLoad.SaveLoadDeleteAction(SaveOption.Load);
            StartRotation();
        }

        yield return null;
    }

    public override void EnterAction()
    {
        if (!SaveManagerStatic.UiBloker)
        {
            DeleteCross.AppearCross();
            StartCoroutine(saveFileFields.CloseOverPanel());
            StartCoroutine(AppearCassette());
        }
    }

    public override void ExitAction()
    {
        if (!SaveManagerStatic.UiBloker)
        {
            DeleteCross.DisappearCross();
            StartCoroutine(saveFileFields.OpenOverPanel());
            StartCoroutine(DisappearCassette());
        }
    }

    public IEnumerator AppearCassette()
    {
        if (CassetteFadeOut != null)
            StopCoroutine(CassetteFadeOut);
        CassetteFadeIn = FadeManager.FadeObject(Cassette, true, SaveManager.instance.speed);
        yield return StartCoroutine(CassetteFadeIn);
    }

    public IEnumerator DisappearCassette()
    {
        if (CassetteFadeIn != null)
            StopCoroutine(CassetteFadeIn);
        CassetteFadeOut = FadeManager.FadeObject(Cassette, false, SaveManager.instance.speed);
        yield return StartCoroutine(CassetteFadeOut);
    }

    public void InstantHideCassette()
    {
        Cassette.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void StartRotation()
    {
        animator.Play("MM_Anim_Rotation");
    }

    public void EndRotation()
    {
        animator.Play("MM_Anim_Idle");
    }

    public override void ResetButtonState()
    {
        base.ResetButtonState();

        InstantHideCassette();
        EndRotation();
        UnlockButton();
    }
}
