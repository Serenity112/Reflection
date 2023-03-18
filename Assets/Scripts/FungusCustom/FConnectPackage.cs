using System.Collections;
using System.Collections.Generic;
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

            PackageConntector.instance.ConnectPackage(PackageName);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(110, 110, 200, 255);
        }
    }
}