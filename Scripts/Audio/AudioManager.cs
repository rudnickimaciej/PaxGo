using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public VolumeSettings VolumeSettings;
    public Sound[] sounds;

    float EffectsVolume;
    float MusicVolume;

    void Awake()
    {

        #region Singleton
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
        #endregion


        foreach (Sound sound in sounds)
        {
            GameObject go = new GameObject("Sound: " + sound.name);
            sound.source = go.AddComponent<AudioSource>();
            go.transform.SetParent(transform);

        }
    }

    public void Play(string name, bool loop = false, float pitch = 1f)
    {
        Sound soundToPlay = sounds.Where(s => s.name == name).SingleOrDefault();
        int random = Random.Range(0, soundToPlay.clips.Count);
        soundToPlay.source.clip = soundToPlay.clips[random];
        soundToPlay.source.pitch = pitch;
        soundToPlay.source.loop = loop;
        soundToPlay.source.Play();
    }

    public void PlayIfNotAlreadyPlayed(string name, bool loop = false, float pitch = 1f)
    {
        if (!(sounds.Where(s => s.name == name).SingleOrDefault().source.isPlaying))
        {
            Play(name, loop, pitch);
        }
    }

    public void SetAudioVolume(float effectVolume, float backgroundVolume)
    {

        foreach (Sound s in sounds)
        {
            s.source.volume = effectVolume;
        }
        sounds.Where(s => s.name == "background").SingleOrDefault().source.volume = backgroundVolume;

        VolumeSettings.SetSliders(new VolumeData(effectVolume, backgroundVolume));

        EffectsVolume = effectVolume;
        MusicVolume = backgroundVolume;
    }
    public void SetAudioVolume(VolumeData volData)
    {
        SetAudioVolume(volData.EffectsVolume, volData.MusicVolume);
    }

    public VolumeData GetVolumeData()
    {
        return new VolumeData(EffectsVolume, MusicVolume);
    }
}
