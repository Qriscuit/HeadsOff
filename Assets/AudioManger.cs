using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManger : MonoBehaviour
{
    public AudioSource MenuMusic;
    public AudioSource ButtonHover;
    public AudioSource ButtonClick1;
    public AudioSource ButtonClick2;
    public AudioSource GameMusic;
    public static AudioManger Inst;

    private void Awake()
    {
        Inst = this;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Audio");

        if (objs.Length > 1)
        {
            Destroy(objs[0].gameObject);
        }
        GameMusic.Stop();
        MenuMusic.Play();
        DontDestroyOnLoad(this.gameObject);
    }
}

