using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ChristinaCreatesGames.Typography.Typewriter
{
    public class Typewriter : MonoBehaviour
    {
        public static Typewriter Instance;

        public static bool denyNextDialog;

        [SerializeField] private GameObject StoryText;

        private Text _text;

        public bool autoSkip = false;
        public bool wasCurrentDialogRead = false;
        public bool skipping = false;

        [SerializeField] private SkipButton skipButton;

        private Coroutine _say;

        private string currentText;

        private bool clickFlag = false;

        private float defaultDelay;

        //private bool dialogSkipFlag = false;

        public float TextSpeed { get; private set; }

        private void Awake()
        {
            Instance = this;

            SetTextSpeed(80);

            _text = StoryText.GetComponent<Text>();
        }

        private void Update()
        {
            if ((Input.GetKey(KeyCode.Tab) || autoSkip) && (SettingsConfig.skipEverything || wasCurrentDialogRead))
            {
                if (!skipping)
                {
                    skipButton.EnableSkip();
                    skipping = true;
                }

                if (!denyNextDialog)
                {
                    SetClickFlag();
                }
            }
            else
            {
                if (skipping)
                {
                    skipButton.DisableSkip();
                    skipping = false;
                }
            }
        }

        public void SetTextSpeed(float value)
        {
            defaultDelay = 1 / value;
        }

        public void SetClickFlag()
        {
            clickFlag = true;
        }

        public void StopTypewriter()
        {
            if (_say != null)
            {
                StopCoroutine(_say);
            }
        }

        public IEnumerator Say(string storyText, bool extend)
        {
            if (extend)
            {
                _text.text += " ";
                currentText = currentText + " " + storyText;
            }
            else
            {
                _text.text = "";
                currentText = storyText;
            }

            _say = StartCoroutine(ISay(storyText));
            yield return _say;
        }

        private IEnumerator ISay(string storyText)
        {
            float timeElapsed = 0f;

            foreach (char c in currentText)
            {
                _text.text += c;

                timeElapsed += Time.deltaTime;
                while (timeElapsed < defaultDelay)
                {
                    if (clickFlag)
                    {
                        break;
                    }

                    timeElapsed += Time.deltaTime;
                    yield return null;
                }

                timeElapsed -= defaultDelay;

                if (clickFlag)
                {
                    clickFlag = false;
                    _text.text = currentText;
                    break;
                }
            }

            while (!clickFlag)
            {
                yield return null;
            }

            clickFlag = false;
        }
    }
}
