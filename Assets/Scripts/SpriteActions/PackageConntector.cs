using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PackageConntector : MonoBehaviour
{
    // Потрясающий класс, нужен для предзагрузки пакета с ассетами. Если его не применять, будет зажержка пару мс перед загрузкой первого ассета из пакета

    public static PackageConntector instance = null;

    private static Dictionary<string, AsyncOperationHandle<Sprite>> handlers;

    private static Dictionary<string, int> packageSizes;

    private void Awake()
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
        packageSizes = new Dictionary<string, int>()
        {
            { "Pasha", 3 },
            { "Katya", 3 },
            { "Tumanov", 2 },
        };
    }

    // Для прелоада 1
    public IEnumerator IConnectPackageGroup(string packageName, int mainNum)
    {
        if (!packageSizes.ContainsKey(packageName))
        {
            yield break;
        }

        List<IEnumerator> list = new List<IEnumerator>();
        for (int i = 1; i <= packageSizes[packageName]; i++)
        {
            if (i != mainNum)
            {
                list.Add(IConnectSinglePackage($"{packageName}{i}_connector"));
            }
        }
        StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(list));
        yield return StartCoroutine(IConnectSinglePackage($"{packageName}{mainNum}_connector"));
    }

    // Для прелоада 2
    public IEnumerator IConnectPackageGroup(string packageName)
    {
        Debug.Log("Start connect " + packageName);

        if (!packageSizes.ContainsKey(packageName))
        {
            yield break;
        }

        List<IEnumerator> list = new List<IEnumerator>();

        for (int i = 1; i <= packageSizes[packageName]; i++)
        {
            list.Add(IConnectSinglePackage($"{packageName}{i}_connector"));
        }

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(list));

        Debug.Log("End connect " + packageName);
    }

    private IEnumerator IConnectSinglePackage(string address)
    {
        AsyncOperationHandle<Sprite> handler = Addressables.LoadAssetAsync<Sprite>(address);
        yield return handler;
        if (handler.Status == AsyncOperationStatus.Succeeded && !handlers.ContainsKey(address))
        {
            handlers.Add(address, handler);
        }
    }

    public IEnumerator IConnectPackage(string packageName, int poseNum)
    {
        yield return StartCoroutine(IConnectSinglePackage($"{packageName}{poseNum}_connector"));
    }

    public void DisconnectPackageGroup(string packageName)
    {
        for (int i = 1; i <= packageSizes[packageName]; i++)
        {
            string address = $"{packageName}{i}_connector";
            if (handlers.ContainsKey(address))
            {
                Addressables.Release(handlers[address]);
                handlers.Remove(address);
            }
        }
    }

    public IEnumerator IDisconnectAllPackages()
    {
        foreach (KeyValuePair<string, AsyncOperationHandle<Sprite>> handler in handlers)
        {
            Addressables.Release(handler.Value);
            yield return null;
        }

        handlers.Clear();
    }
}
