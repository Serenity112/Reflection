using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class OptionScript : MonoBehaviour
{
    public string BlockName;
    public int OptionNumber;
    GameObject ChoiseManager;

    void Start()
    {
        ChoiseManager = GameObject.Find("ChoiseManager");
    }

    public void LoadBlock()
    {     
        ChoiseManager.GetComponent<ChoiseManager>().LoadChoise(BlockName, OptionNumber);
    }

}
