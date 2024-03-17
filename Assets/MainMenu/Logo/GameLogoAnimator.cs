using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogoAnimator : MonoBehaviour
{
    [SerializeField] private GameObject UnityBG;
    [SerializeField] private GameObject LogoNew;
    [SerializeField] private GameObject LogoOld;
    [SerializeField] private GameObject WRLogo;

    private float logoSpeed = 1.5f;
    private IEnumerator iupdate;

    private bool logo_first_load = false;

    private void OnEnable()
    {
        // Добавить проверки
        /*if (ES3.FileExists(SaveSystemUtils.SaveFilesData) && ES3.KeyExists("logo_first_load", SaveSystemUtils.SaveFilesData))
        {

        }*/

        StartUpdate();
    }

    private void OnDisable()
    {
        EndUpdate();
    }

    public IEnumerator IAnimate()
    {
        List<IEnumerator> list = new List<IEnumerator>()
        {
            IUpdate(),
            Animate()
        };

        yield return StartCoroutine(CoroutineUtils.WaitForAny(list, this));
        StopAllCoroutines();
    }

    private IEnumerator Animate()
    {
        yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>() {
            FadeManager.FadeObject(WRLogo, true, logoSpeed),
            FadeManager.FadeObject(UnityBG, false, logoSpeed)
        }, this));

        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(FadeManager.FadeObject(WRLogo, false, logoSpeed));
        yield return StartCoroutine(FadeManager.FadeObject(LogoNew, true, logoSpeed));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeManager.FadeObject(LogoOld, true, logoSpeed));
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator IUpdate()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Space))
            {
                yield break;
            }

            yield return null;
        }
    }

    public void StartUpdate()
    {
        if (iupdate != null)
        {
            StopCoroutine(iupdate);
        }

        iupdate = IUpdate();
        StartCoroutine(iupdate);
    }

    public void EndUpdate()
    {
        if (iupdate != null)
        {
            StopCoroutine(iupdate);
        }
    }
}
