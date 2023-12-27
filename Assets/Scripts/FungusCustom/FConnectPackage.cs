using System;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FConnectPackage",
                 "FConnectPackage")]
    public class FConnectPackage : Command
    {
        [SerializeField]
        private string PackageName;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            var parsed = Enum.TryParse<global::Character>(PackageName, true, out global::Character character);

            Debug.Log(parsed + " " + character.ToString());

            StartCoroutine(PackageConntector.instance.IConnectPackageGroupPreloaded(parsed ? character : global::Character.None));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(110, 110, 200, 255);
        }
    }
}