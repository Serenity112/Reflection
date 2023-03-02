using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    [CommandInfo("Ref",
                 "SoundStart",
                 "SoundStart")]
    public class SoundStart : Command
    {
        public int sound;
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
            
            AudioManager.GetComponent<AudioManager>().SoundStart(sound);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(96, 63, 97, 255);
        }

    }
}