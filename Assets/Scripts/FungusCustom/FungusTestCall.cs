﻿using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;

namespace Fungus
{
    /// <summary>
    /// Supported modes for calling a block.
   
    /// <summary>
    /// Execute another block in the same Flowchart as the command, or in a different Flowchart.
    /// </summary>
    [CommandInfo("Flow",
                 "FungusCall",
                 "Execute another block in the same Flowchart as the command, or in a different Flowchart.")]
    [AddComponentMenu("")]
    public class FungusTestCall : Command
    {
        [Tooltip("Flowchart which contains the block to execute. If none is specified then the current Flowchart is used.")]
        [SerializeField] protected Flowchart targetFlowchart;

        [FormerlySerializedAs("targetSequence")]
        [Tooltip("Block to start executing")]
        [SerializeField] static Block targetBlock;

        [Tooltip("Label to start execution at. Takes priority over startIndex.")]
        [SerializeField] protected StringData startLabel = new StringData();

        [Tooltip("Command index to start executing")]
        [FormerlySerializedAs("commandIndex")]
        [SerializeField] protected int startIndex;


        [Tooltip("Select if the calling block should stop or continue executing commands, or wait until the called block finishes.")]
        [SerializeField] protected CallMode callMode;

        public void TestCall()
        {
           // GameObject.Find("ButtonManager1").GetComponent<Save>().LoadThis();

            targetBlock = targetFlowchart.FindBlock(Player.CurrentBlock);
            startIndex = Player.CurrentCommandIndex;


            var flowchart = GetFlowchart();

            if (targetBlock != null)
            {
                // Check if calling your own parent block
                if (ParentBlock != null && ParentBlock.Equals(targetBlock))
                {
                    // Just ignore the callmode in this case, and jump to first command in list
                    Continue(0);
                    return;
                }

                if (targetBlock.IsExecuting())
                {
                    Debug.LogWarning(targetBlock.BlockName + " cannot be called/executed, it is already running.");
                    Continue();
                    return;
                }

                // Callback action for Wait Until Finished mode
                Action onComplete = null;
                if (callMode == CallMode.WaitUntilFinished)
                {
                    onComplete = delegate {
                        Continue();
                    };
                }

                // Find the command index to start execution at
                int index = startIndex;
                if (startLabel.Value != "")
                {
                    int labelIndex = targetBlock.GetLabelIndex(startLabel.Value);
                    if (labelIndex != -1)
                    {
                        index = labelIndex;
                    }
                }

                if (targetFlowchart == null ||
                    targetFlowchart.Equals(GetFlowchart()))
                {
                    if (callMode == CallMode.StopThenCall)
                    {
                        StopParentBlock();
                    }
                    StartCoroutine(targetBlock.Execute(index, onComplete));
                }
                else
                {
                    if (callMode == CallMode.StopThenCall)
                    {
                        StopParentBlock();
                    }
                    // Execute block in another Flowchart
                    targetFlowchart.ExecuteBlock(targetBlock, index, onComplete);
                }
            }

            if (callMode == CallMode.Stop)
            {
                StopParentBlock();
            }
            else if (callMode == CallMode.Continue)
            {
                Continue();
            }
        }


    }
}