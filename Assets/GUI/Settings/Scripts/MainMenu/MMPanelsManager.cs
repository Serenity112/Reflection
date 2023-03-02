using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class MMPanelsManager : MonoBehaviour
{
    public static MMPanelsManager instance = null;

    public void LoadGame()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
