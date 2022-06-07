using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> clips = new List<AudioClip>();

    public void PlayAudio(string name, Vector3 pos, float volume)
    {
        foreach (var item in clips)
        {
            if(item.name == name)
            {
                AudioSource.PlayClipAtPoint(item, pos, volume);
            }
        }
        
    }



}
