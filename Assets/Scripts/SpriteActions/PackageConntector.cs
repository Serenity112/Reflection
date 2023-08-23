using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PackageConntector : MonoBehaviour
{
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

    public IEnumerator IConnectPackageGroup(string packageName, bool preload = false)
    {
        GameSpriteObject? sprite_obj = SpriteController.instance.GetAvaliableSprite(packageName);
        if (sprite_obj == null)
        {
            yield break;
        }
        GameSpriteObject sprite = (GameSpriteObject)sprite_obj;

        if (preload)
        {
            SpriteController.instance.SaveSpriteDataPreloaded(sprite.num, true);
        }

        if (!packageSizes.ContainsKey(packageName))
        {
            yield break;
        }

        List<IEnumerator> list = new List<IEnumerator>();

        for (int i = 1; i <= packageSizes[packageName]; i++)
        {
            list.Add(IConnectSinglePackage($"{packageName}{i}_connector"));
        }

        //Debug.Log("Start connect " + packageName);

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(list));

        //Debug.Log("Connected " + packageName);
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

        //Debug.Log("IDisconnectAllPackages ended");
    }
}
