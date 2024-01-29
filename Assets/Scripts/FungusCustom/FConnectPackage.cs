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
            var parsed = Enum.TryParse<global::Character>(PackageName, true, out global::Character character);

            // Нет yield return, т.к. должно происходить в фоне
            StartCoroutine(PackageConntector.instance.IConnectPackageGroupPreloaded(parsed ? character : global::Character.None));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(110, 110, 200, 255);
        }
    }
}