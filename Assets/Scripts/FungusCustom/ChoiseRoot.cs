using UnityEngine;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Ref",
                 "ChoiseRoot",
                 "ChoiseRoot")]
    public class ChoiseRoot : Command
    {
        public string Name;
        public int Root;
        GameObject scene;

        void Start()
        {
            scene = this.transform.parent.gameObject;
        }

        public override void OnEnter()
        {
            Player.IncreaseIndex(0);

            //�������� ��������� � �����, ����� ���� �� ������

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(200, 130, 200, 255);
        }

    }
}