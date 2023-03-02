using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameChanger : MonoBehaviour
{
    public static NameChanger instance = null;

    public Sprite[] Names;

    GameObject namePanel;

    public Dictionary<string, int> namesToSprite = new Dictionary<string, int>();

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }

        namePanel = this.gameObject;

        namesToSprite["kat"] = 0;
        namesToSprite["nas"] = 1;
        namesToSprite["eve"] = 2;
        namesToSprite["tan"] = 3;
        namesToSprite["pas"] = 4;
        namesToSprite["ser"] = 5;
        namesToSprite["tum"] = 6;
        namesToSprite["raket"] = 7;
        namesToSprite["dev"] = 8;
        namesToSprite["nob"] = 9;
    }

    public void SetName(string name)
    {
        if (!(namesToSprite.ContainsKey(name)))
        {
            DelName();
        } else
        {
            namePanel.GetComponent<Image>().sprite = Names[namesToSprite[name]];
            namePanel.SetActive(true);
        }   
    }

    public void DelName()
    {
        namePanel.SetActive(false);
    }
  
}
