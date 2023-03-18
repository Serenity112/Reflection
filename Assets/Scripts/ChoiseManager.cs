using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Fungus
{
    public class ChoiseManager : MonoBehaviour
    {
        public Button LoadButton;
        public Button ContinueDialoggButton;
        public GameObject scene;
        public GameObject StoryText;
        public GameObject OptionPrefab;

        GameObject Manager;

        string currentChoise;
        Dictionary<string, int> optionsHistory;

        void Start()
        {
            currentChoise = null;
            Manager = this.transform.gameObject;
            optionsHistory = new Dictionary<string, int>();
        }

        public void GenerateChoise(string[] Messages, string[] BlocksRedirection, string ChoiseName)
        {
            currentChoise = ChoiseName;
            float Down = -30;
            float counter = 0;

            for (int i = 0; i < Messages.Length; i++)
            {
                GameObject option = Instantiate(OptionPrefab, OptionPrefab.transform.position, Quaternion.identity);

                option.name = "option" + counter;
                counter++;

                option.transform.SetParent(Manager.GetComponent<RectTransform>());

                RectTransform rect = option.GetComponent<RectTransform>();

                option.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                rect.anchorMin = new Vector2(0.5f, 1f);
                rect.anchorMax = new Vector2(0.5f, 1f);
                rect.pivot = new Vector2(0.5f, 0.5f);

                rect.anchoredPosition3D = new Vector3(0, Down, 0);
                Down -= 50;

                option.transform.GetChild(0).GetComponent<Text>().text = Messages[i];
                option.GetComponent<OptionScript>().BlockName = BlocksRedirection[i];
                option.GetComponent<OptionScript>().OptionNumber = i;

                StoryText.GetComponent<CanvasGroup>().alpha = 0;
            }
        }

        public void SplitBlocks(string choiseName, string[] BlocksRedirection)
        {
            int option = optionsHistory[choiseName];

            string block = BlocksRedirection[option];

            LoadBlock(block, 0);
        }

        public void LoadChoise(string BlockName, int numberChosen)
        {
            optionsHistory[currentChoise] = numberChosen;

            UserData.instance.CurrentBlock = BlockName;
            UserData.instance.CurrentCommandIndex = 0;
            StoryText.GetComponent<CanvasGroup>().alpha = 1;

            //Player.AllowSkip();

            DeleteOptions();

            LoadBlock(BlockName, 0);
        }

        void LoadBlock(string CurrentBlock, int CurrentIndex)
        {
            /*Block targetBlock = Player.targetFlowchart.FindBlock(CurrentBlock);

            if (targetBlock != null)
            {
                targetBlock.Stop();

                Player.targetFlowchart.ExecuteBlock(targetBlock, CurrentIndex, null);
            }*/
        }

        public void DeleteOptions()
        {
            foreach (Transform child in Manager.transform)
            {
                GameObject.Destroy(child.gameObject, 0f);
            }
        }
    }

}