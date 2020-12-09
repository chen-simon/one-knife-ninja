using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager manager;

    void Awake()
    {
        //Singleton
        if (!manager) { manager = this; }
        else { Destroy(gameObject); }
        DontDestroyOnLoad(gameObject);
    }
}
