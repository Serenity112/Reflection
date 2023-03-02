using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AudioTransition",
                 "AudioTransition")]
    public class AudioTransition : Command
    {
        public int ost;
        public float duration;
        GameObject AudioManager;
        AudioMixer audioMixer;

        void Start()
        {
            AudioManager = GameObject.Find("AudioManager").gameObject;
            audioMixer = AudioManager.GetComponent<AudioManager>().audioMixer;
        }

        public override void OnEnter()
        {
            Player.IncreaseIndex(1);
            
            AudioManager.GetComponent<AudioManager>().MusicTransition(ost, duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }

    }
}