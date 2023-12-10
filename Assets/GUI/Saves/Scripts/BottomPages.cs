using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BottomPages : MonoBehaviour
{
    private BottomPageButton[] _buttons;

    private int _count;

    public void Awake()
    {
        _count = SaveManager.maxPages;
        _buttons = new BottomPageButton[_count];

        for (int i = 0; i < _count; i++)
        {
            _buttons[i] = transform.GetChild(i).gameObject.GetComponent<BottomPageButton>();

            _buttons[i].InitializeButton(i, this);
        }
    }

    public void StartingLoad()
    {
        for (int i = 0; i < _count; i++)
        {
            _buttons[i].SetText((i + 1).ToString());
        }

        _buttons[0].SquareToWhite();
        _buttons[0].NumberToWhite();

        _buttons[1].SquareToGray();
        _buttons[1].NumberToGray();
    }


    public void loadPageOnClick(int page)
    {
        if (page != SaveManager.instance.currentPage && !StaticVariables.UIsystemDown && !StaticVariables.GAME_LOADING)
        {
            StaticVariables.UIsystemDown = true;

            SaveManager.instance.LoadPage(page);
        }
    }

    public void HidePage(int page)
    {
        _buttons[page].SquareToGray();
    }

    public void ShowPage(int page)
    {
        _buttons[page].SquareToWhite();
    }

    public void AppearNumbers(int page)
    {
        if (page == 0)
        {
            _buttons[0].NumberToWhite();
            _buttons[1].NumberToGray();

        }
        else if (page == (SaveManager.maxPages - 1))
        {
            _buttons[SaveManager.maxPages - 1].NumberToWhite();
            _buttons[SaveManager.maxPages - 2].NumberToGray();
        }
        else
        {
            _buttons[page - 1].NumberToGray();
            _buttons[page].NumberToWhite();
            _buttons[page + 1].NumberToGray();
        }
    }

    public void DisappearNumbers(int page)
    {
        if (page == 0)
        {
            _buttons[0].NumberToBlank();
            _buttons[1].NumberToBlank();

        }
        else if (page == (SaveManager.maxPages - 1))
        {
            _buttons[SaveManager.maxPages - 1].NumberToBlank();
            _buttons[SaveManager.maxPages - 2].NumberToBlank();
        }
        else
        {
            _buttons[page - 1].NumberToBlank();
            _buttons[page].NumberToBlank();
            _buttons[page + 1].NumberToBlank();
        }
    }


    public void InitialReset()
    {
        for (int i = 0; i < SaveManager.maxPages; i++)
        {
            _buttons[i].ResetButtonState();
        }
    }
}
