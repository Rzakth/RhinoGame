using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject AudioPrefab;
    //audio called once
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void Play3D(AudioClip clip, Vector3 position)
    {
        //if there's no clip, return
        if (clip == null)
            return;

        var audioGameObject = Instantiate(AudioPrefab, position, Quaternion.identity);
        var source = audioGameObject.GetComponent<AudioSource>();

        source.clip = clip;
        source.Play();

        //destroy sound once clip is done
        Destroy(audioGameObject, clip.length);
    }
}
