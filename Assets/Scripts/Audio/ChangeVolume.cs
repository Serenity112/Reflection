using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    [CommandInfo("Ref",
                 "ChangeVolume",
                 "ChangeVolume")]
    public class ChangeVolume : Command
    {
        public float LowPercent;

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

            //AudioManager.GetComponent<AudioManager>().SoundStart(sound);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(96, 63, 97, 255);
        }

    }
}