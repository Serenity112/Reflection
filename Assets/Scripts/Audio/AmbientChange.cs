using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientChange",
                 "AmbientChange")]
    public class AmbientChange : Command
    {
        public int ambient;
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

            AudioManager.GetComponent<AudioManager>().AmbientChange(ambient, durationOut, durationWait, durationIn);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(111, 53, 112, 255);
        }

    }
}