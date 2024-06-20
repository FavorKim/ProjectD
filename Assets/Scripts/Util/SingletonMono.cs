using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();

                if (instance == null)
                {
                    var obj = new GameObject(nameof(T));
                    instance = obj.AddComponent<T>();

                    DontDestroyOnLoad(instance);
                }
            }
            return instance;
        }
    }

    

}
