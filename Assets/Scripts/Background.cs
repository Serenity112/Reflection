using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Background : MonoBehaviour
{
	public float scrollSpeed = 0.1f;

	MeshRenderer mesh;

	public string sortingLayer;

    void Awake()
    {
		mesh = GetComponent<MeshRenderer>();
		this.GetComponent<Renderer>().sortingLayerName = sortingLayer;
	}

    void Start()
	{
		scrollSpeed = -scrollSpeed;	
	}

	private void Update()
	{
		float x = Time.time * scrollSpeed;

		Vector2 offset = new Vector2(x, 0);//

		mesh.sharedMaterial.SetTextureOffset("_MainTex", offset);
	}
}