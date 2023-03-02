using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Fungus
{   
    public class LoadLog : MonoBehaviour
    {   
        public GameObject scene;
        public GameObject ChatLog;
        public Flowchart targetFlowchart;
        public Scrollbar scrollbar;

        public List<Command> commandList;

        public void Load(int index)
        {
            scrollbar.value = 0;

            UnityEngine.Debug.Log("Начало загрузки");

            List<string> LogBlocks = Player.LogBlocks;
            UnityEngine.Debug.Log("LogBlocks.Count: " + LogBlocks.Count);

            for (int i = 0; i < LogBlocks.Count; i++)
            {
                Block currentBlock = targetFlowchart.FindBlock(LogBlocks[i]);
                UnityEngine.Debug.Log("Найден блок:" + LogBlocks[i]);

                commandList = currentBlock.CommandList;

                for (int j = 0; j < index; j++)
                {
                    UnityEngine.Debug.Log("Обработка команды индекса: " + j);
                    var command = commandList[j];

                    //if (command == null)
                    //{
                    //    continue;
                    //}

                    if (command.GetName() == "FungusSayDialog")
                    {
                        //ChatLog.GetComponent<LogManager>().CreateMessage(command.GetText());
                        
                    }
                } 
            }
        }
    } 
}