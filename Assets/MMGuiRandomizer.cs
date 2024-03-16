using System;
using UnityEngine;

public class MMGuiRandomizer : MonoBehaviour
{
    [SerializeField] private GameObject GuiV1;
    [SerializeField] private GameObject GuiV2;

    private void Awake()
    {
        System.Random random = new System.Random();

        int guiVersion = random.Next(100);

        if (guiVersion == 0)
        {
            ChooseV1();
        }
        else
        {
            ChooseV2();
        }
    }

    private void ChooseV1()
    {
        GuiV1.SetActive(true);
        GuiV2.SetActive(false);
    }

    private void ChooseV2()
    {
        GuiV1.SetActive(false);
        GuiV2.SetActive(true);
    }
}
