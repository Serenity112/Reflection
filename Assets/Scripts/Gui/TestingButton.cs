using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestingButton : MonoBehaviour
{
    private void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            UnityEngine.Debug.Log("test_");
        }
    }
            

    private void OnMouseExit()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            UnityEngine.Debug.Log("test2_");
        }
    }
}
