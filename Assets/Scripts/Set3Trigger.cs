using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Set3Trigger : MonoBehaviour
{
    public Animator animator;
	public void Sett3Trigger()
	{
		animator.SetTrigger("Next");
	}
}
