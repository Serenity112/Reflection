using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    AsyncOperation async;

    void Start()
    {
        
        //StartCoroutine(LoadAsyncScene());     
    }

    public void OpenSettings()
    {
       
    }

    public void CloseSettings()
    {
      
    }




    public void Play()
    {
        async = SceneManager.LoadSceneAsync("Game");
       // async.allowSceneActivation = true;
        
    }

    IEnumerator LoadAsyncScene()
    {
        
        async.allowSceneActivation = false;
        
        while (!async.isDone)
        {
            yield return null;
        }
    }
}
