﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;


namespace Fungus
{

    //public enum ClickMode
    //{
    //    /// <summary> Clicking disabled. </summary>
    //    Disabled,
    //    /// <summary> Click anywhere on screen to advance. </summary>
    //    ClickAnywhere,
    //    /// <summary> Click anywhere on Say Dialog to advance. </summary>
    //    ClickOnDialog,
    //    /// <summary> Click on continue button to advance. </summary>
    //    ClickOnButton
    //}


    public class DialogMod : MonoBehaviour
    {
        [Tooltip("Click to advance story")]
        [SerializeField] protected ClickMode clickMode;

        [Tooltip("Delay between consecutive clicks. Useful to prevent accidentally clicking through story.")]
        [SerializeField] protected float nextClickDelay = 0f;

        [Tooltip("Allow holding Cancel to fast forward text")]
        [SerializeField] protected bool cancelEnabled = true;

        [Tooltip("Ignore input if a Menu dialog is currently active")]
        [SerializeField] protected bool ignoreMenuClicks = true;

        protected bool dialogClickedFlag;

        protected bool nextLineInputFlag;

        protected float ignoreClickTimer;

        protected StandaloneInputModule currentStandaloneInputModule;

        protected Writer writer;

        // Кнопка скипа игры
        public static bool autoSkip { get; set; }

        // Публичный параметр для понимания, идтёт ли сейчас пропуск
        public static bool skipping { get; set; }

        public SkipButton skipButton;

        public static bool denyNextDialog { get; set;}

        public static bool wasCurrentDialogRead { get; set; }

        public void DenySkip() { 
            denyNextDialog = true; 
        }

        protected virtual void Awake()
        {
            writer = GetComponent<Writer>();

            CheckEventSystem();
        }

        // There must be an Event System in the scene for Say and Menu input to work.
        // This method will automatically instantiate one if none exists.
        protected virtual void CheckEventSystem()
        {
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                // Auto spawn an Event System from the prefab
                GameObject prefab = Resources.Load<GameObject>("Prefabs/EventSystem");
                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab) as GameObject;
                    go.name = "EventSystem";
                }
            }
        }

        public void delDialogSaves()
        {
            foreach (var key in ES3.GetKeys("DialogSaves.es3"))
            {
                ES3.DeleteKey(key, "DialogSaves.es3");
            }
        }

        private void Start()
        {
            //denySkip = true;
        }

        public void Update()
        {                    
            if (EventSystem.current == null)
            {
                return;
            }

            if (currentStandaloneInputModule == null)
            {
                currentStandaloneInputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
            }

            if (writer != null && writer.IsWriting)
            {
                if ((Input.GetKey(KeyCode.Tab) || autoSkip) && (SettingsConfig.skipEverything || wasCurrentDialogRead) && !denyNextDialog)
                {
                    if(!skipping)
                    {
                        skipping = true;
                    }
                    
                    SetNextLineFlag();
                }
                else
                {
                    skipping = false;
                }
            }


            switch (clickMode)
            {
                case ClickMode.Disabled:
                    break;
                case ClickMode.ClickAnywhere:
                    if (Input.GetMouseButtonDown(0))
                    {
                        //UnityEngine.Debug.Log("CLICK3");
                        SetNextLineFlag();
                    }
                    break;
                case ClickMode.ClickOnDialog:
                    if (dialogClickedFlag && !denyNextDialog)
                    {
                        //UnityEngine.Debug.Log("CLICK2");
                        SetNextLineFlag();
                        dialogClickedFlag = false;
                    }
                    break;
            }




            if (ignoreClickTimer > 0f)
            {
                ignoreClickTimer = Mathf.Max(ignoreClickTimer - Time.deltaTime, 0f);
            }

            if (ignoreMenuClicks)
            {
                // Ignore input events if a Menu is being displayed
                if (MenuDialog.ActiveMenuDialog != null &&
                    MenuDialog.ActiveMenuDialog.IsActive() &&
                    MenuDialog.ActiveMenuDialog.DisplayedOptionsCount > 0)
                {
                    dialogClickedFlag = false;
                    nextLineInputFlag = false;
                }
            }

            // Tell any listeners to move to the next line
            if (nextLineInputFlag)
            {
                var inputListeners = gameObject.GetComponentsInChildren<IDialogInputListener>();
                for (int i = 0; i < inputListeners.Length; i++)
                {
                    var inputListener = inputListeners[i];
                    inputListener.OnNextLineEvent();
                }
                nextLineInputFlag = false;
            }
        }

        #region Public members

        /// <summary>
        /// Trigger next line input event from script.
        /// </summary>
        public virtual void SetNextLineFlag()
        {
            //if(!denySkip)
                nextLineInputFlag = true;
        }

        /// <summary>
        /// Set the dialog clicked flag (usually from an Event Trigger component in the dialog UI).
        /// </summary>
        public virtual void SetDialogClickedFlag()
        {
            if(!denyNextDialog)
            {
                // Ignore repeat clicks for a short time to prevent accidentally clicking through the character dialogue
                if (ignoreClickTimer > 0f)
                {
                    return;
                }
                ignoreClickTimer = nextClickDelay;

                // Only applies in Click On Dialog mode
                if (clickMode == ClickMode.ClickOnDialog)
                {
                   // UnityEngine.Debug.Log("CLICK1");
                    dialogClickedFlag = true;
                }
                else
                {
                   // UnityEngine.Debug.Log("BLOCKEd");
                }
            } 
            else
            {
                //UnityEngine.Debug.Log("Button denied");
            }
        }

        /// <summary>
        /// Sets the button clicked flag.
        /// </summary>
        public virtual void SetButtonClickedFlag()
        {
            // Only applies if clicking is not disabled
            if (clickMode != ClickMode.Disabled)
            {

                SetNextLineFlag();
            }
        }

        #endregion
    }
}