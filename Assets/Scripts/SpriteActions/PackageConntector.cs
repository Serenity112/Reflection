using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PackageConntector : MonoBehaviour
{
    public static PackageConntector instance = null;

    private static Dictionary<string, AsyncOperationHandle<Sprite>> handlers;

    private static Dictionary<Character, int> packageSizes;

    private void Awake()
    {
        instance = this;

        handlers = new Dictionary<string, AsyncOperationHandle<Sprite>>();
        packageSizes = new Dictionary<Character, int>()
        {
            { Character.Pasha, 3 },
            { Character.Katya, 3 },
            { Character.Nastya, 5 },
            { Character.Tanya, 3 },

            { Character.Tumanov, 2 },
            { Character.Raketnikov, 2 },
        };
    }

    public IEnumerator IConnectPackageGroupPreloaded(Character characterGroup)
    {
        SpriteController.instance.GetAvaliableSprite(characterGroup);

        SpriteController.instance.SaveSpriteDataPreloaded(characterGroup, true);

        yield return StartCoroutine(IConnectPackageGroup(characterGroup));
    }

    public IEnumerator IConnectPackageGroup(Character characterGroupName)
    {
        if (!packageSizes.ContainsKey(characterGroupName))
        {
            yield break;
        }

        List<IEnumerator> list = new List<IEnumerator>();

        for (int i = 1; i <= packageSizes[characterGroupName]; i++)
        {
            list.Add(IConnectSinglePackage($"{characterGroupName}{i}_connector"));
        }

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(list));
    }

    private IEnumerator IConnectSinglePackage(string address)
    {
        if (handlers.ContainsKey(address))
        {
            yield break;
        }

        AsyncOperationHandle<Sprite> handler = Addressables.LoadAssetAsync<Sprite>(address);
        yield return handler;

        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            handlers.Add(address, handler);
        }
    }

    public void DisconnectPackageGroup(Character characterGroupName)
    {
        for (int i = 1; i <= packageSizes[characterGroupName]; i++)
        {
            string address = $"{characterGroupName}{i}_connector";
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
