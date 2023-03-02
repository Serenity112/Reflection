using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Load : MonoBehaviour
{
    public AsyncOperationHandle<Sprite> handle1;
    public AsyncOperationHandle<Sprite> handle2;
    public AsyncOperationHandle<Sprite> handle3;
    public AsyncOperationHandle<Sprite> handle4;


    public GameObject sprite1;
    public GameObject sprite2;
    public GameObject sprite3;
    public GameObject sprite4;


    public void Appear()
    {
        StartCoroutine(SpriteAppear());
    }


    private IEnumerator SpriteAppear()
    {
        handle1 = Addressables.LoadAssetAsync<Sprite>("Pasha_1_0");
        handle2 = Addressables.LoadAssetAsync<Sprite>("Pasha_1_1");
        handle3 = Addressables.LoadAssetAsync<Sprite>("Pasha_1_2");
        handle4 = Addressables.LoadAssetAsync<Sprite>("Pasha_1_3");

        yield return handle1;
        yield return handle2;
        yield return handle3;
        yield return handle4;

        sprite1.GetComponent<SpriteRenderer>().sprite = handle1.Result;
        sprite2.GetComponent<SpriteRenderer>().sprite = handle2.Result;
        sprite3.GetComponent<SpriteRenderer>().sprite = handle3.Result;
        sprite4.GetComponent<SpriteRenderer>().sprite = handle4.Result;


    }
}
