using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data: MonoBehaviour
{
    public static Data data;
    
    //Loading freom main menu settigs
    



    void Awake()
    {
        if (data != null)
            GameObject.Destroy(data);
        else
            data = this;

        DontDestroyOnLoad(this);
    }
}
