using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AudioStart",
                 "AudioStart")]
    public class AudioStart : Command
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
            
            AudioManager.GetComponent<AudioManager>().MusicStart(ost, duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }

    }
}