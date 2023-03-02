using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AudioChange",
                 "AudioChange")]
    public class AudioChange : Command
    {
        public int ost;
        public float durationOut;
        public float durationWait;
        public float durationIn;

        GameObject scene;
        GameObject AudioManager;
        AudioMixer audioMixer;

        void Start()
        {
            scene = this.transform.parent.gameObject;
            AudioManager = scene.transform.Find("AudioManager").gameObject;
            audioMixer = AudioManager.GetComponent<AudioManager>().audioMixer;
        }

        public override void OnEnter()
        {
            Player.IncreaseIndex(1);
            
            AudioManager.GetComponent<AudioManager>().MusicStartNew(ost, durationOut, durationWait, durationIn);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }

    }
}