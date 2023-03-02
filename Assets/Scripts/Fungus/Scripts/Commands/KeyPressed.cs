using UnityEngine;
using System;
namespace Fungus
{
 [CommandInfo("iTween", 
                 "KeyPress", 
                 "m")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class KeyPress : Command
	{
	 public override void OnEnter()
	 {
		Console.ReadKey();
	}
	}

}