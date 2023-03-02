using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AudioEnd",
                 "AudioEnd")]
    public class AudioEnd : Command
    {
        public float duration;
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
            
            AudioManager.GetComponent<AudioManager>().MusicEnd(duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }

    }
}