using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainBG : MonoBehaviour
{
    public string sortingLayer;
	void Awake()
	{
		this.GetComponent<Renderer>().sortingLayerName = sortingLayer;
	}

}
