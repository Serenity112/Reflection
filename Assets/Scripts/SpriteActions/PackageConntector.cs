using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PackageConntector : MonoBehaviour
{
    // ѕотр€сающий класс, нужен дл€ предзагрузки пакета с ассетами. ≈сли его не примен€ть, будет зажержка пару мсп перед загрузкой первого ассета из пакета

    public static PackageConntector instance = null;

    private static Dictionary<string, AsyncOperationHandle<Sprite>> handlers;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }

        handlers = new Dictionary<string, AsyncOperationHandle<Sprite>>();
    }
    public void ConnectPackage(string PackageName)
    {
        StartCoroutine(IConnectPackage(PackageName));
    }

    public void DisconnectPackage(string PackageName)
    {
        string address = PackageName += "_connector";
        Addressables.Release(handlers[address]);
        handlers.Remove(address);
    }
    public void DisconnectAllPackages()
    {
        foreach(KeyValuePair<string, AsyncOperationHandle<Sprite>> handler in handlers)
        {
            Addressables.Release(handler.Value);          
        }

        handlers.Clear();
    }
    private IEnumerator IConnectPackage(string PackageName)
    {
        string address = PackageName += "_connector";
        AsyncOperationHandle<Sprite> handler = Addressables.LoadAssetAsync<Sprite>(address);
        yield return handler;

        if(handler.Status == AsyncOperationStatus.Succeeded)
        {
            handlers.Add(address, handler);
        }
        else
        {
            Debug.Log("Error loading package connector");
        }      
    }
}