using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPages : MonoBehaviour
{
    public GameObject[] allPages;

    public void Awake()
    {
        allPages = new GameObject[SaveManager.maxPages];
        for (int i = 0; i < SaveManager.maxPages; i++)
        {
            allPages[i] = transform.GetChild(i).gameObject;
        }
    }

    public void StartingLoad()
    {
        allPages[0].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);


        for (int i = 0; i < SaveManager.maxPages; i++)
        {
            allPages[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = (i + 1).ToString();
        }

        allPages[0].transform.GetChild(0).gameObject.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
    }


    public void loadPageOnClick(int page)
    {
        if (page != SaveManager.instance.currentPage && !StaticVariables.UIsystemDown)
        {
            StaticVariables.UIsystemDown = true;

            SaveManager.instance.LoadPage(page);
        }
    }

    public void Disactivate(int page)
    {
        allPages[page].GetComponent<BottomPageButton>().SquareHide();
    }

    public void Activate(int page)
    {
        allPages[page].GetComponent<BottomPageButton>().SquareAppear();
    }
    public void AppearNumbers(int page)
    {
        if (page == 0)
        {
            allPages[0].GetComponent<BottomPageButton>().NumberToWhite();
            allPages[1].GetComponent<BottomPageButton>().NumberToGray();

        }
        else if (page == (SaveManager.maxPages - 1))
        {
            allPages[SaveManager.maxPages - 1].GetComponent<BottomPageButton>().NumberToWhite();
            allPages[SaveManager.maxPages - 2].GetComponent<BottomPageButton>().NumberToGray();
        }
        else
        {
            allPages[page - 1].GetComponent<BottomPageButton>().NumberToGray();
            allPages[page].GetComponent<BottomPageButton>().NumberToWhite();
            allPages[page + 1].GetComponent<BottomPageButton>().NumberToGray();
        }
    }

    public void DisappearNumbers(int page)
    {
        if (page == 0)
        {
            allPages[0].GetComponent<BottomPageButton>().NumberToBlank();
            allPages[1].GetComponent<BottomPageButton>().NumberToBlank();

        }
        else if (page == (SaveManager.maxPages - 1))
        {
            allPages[SaveManager.maxPages - 1].GetComponent<BottomPageButton>().NumberToBlank();
            allPages[SaveManager.maxPages - 2].GetComponent<BottomPageButton>().NumberToBlank();
        }
        else
        {
            allPages[page - 1].GetComponent<BottomPageButton>().NumberToBlank();
            allPages[page].GetComponent<BottomPageButton>().NumberToBlank();
            allPages[page + 1].GetComponent<BottomPageButton>().NumberToBlank();
        }
    }

}
