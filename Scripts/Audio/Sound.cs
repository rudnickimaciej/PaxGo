using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public List<AudioClip> clips;
    public AudioSource source;
}
