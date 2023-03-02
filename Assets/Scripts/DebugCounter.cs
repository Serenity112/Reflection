using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugCounter : MonoBehaviour
{
    public TextMeshProUGUI TextPro;
    public Button referenceToTheButton;

    public void UpdateTxt(string ntxt)
    {
        TextPro.text = ntxt;
    }

    public void Play()
    {
        GameObject scene = GameObject.Find("Scene");

        Player.CurrentCommandIndex = 0;
        //scene.GetComponent<Player>().CurrentBlock = "d1_Dream";
       // Player.SaveBlock = Player.CurrentBlock;
        GameObject sc = GameObject.Find("SpriteManager");

        for (int i = 0; i < 4; i++)
        {
                //sc.GetComponent<SpriteController>().Activity[i] = false;
               // sc.GetComponent<SpriteController>().Names[i] = null;
               // sc.GetComponent<SpriteController>().Poses[i] = 0;
                GameObject sprite = scene.GetComponent<Player>().Sprites.transform.GetChild(i).gameObject;
                sprite.transform.localPosition = new Vector3(-2000, -2000, 0);         
        }

        referenceToTheButton.onClick.Invoke();
    }

}
