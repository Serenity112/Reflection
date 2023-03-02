using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientStart",
                 "AmbientStart")]
    public class AmbientStart : Command
    {
        public int ambient;
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

            AudioManager.GetComponent<AudioManager>().AmbientStart(ambient, duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(111, 53, 112, 255);
        }

    }
}