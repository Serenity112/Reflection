using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public AudioSource musicSource;
    public AudioSource musicBuffSource;
    public AudioSource ambientSource;
    public AudioSource soundSource;

    private Dictionary<string, AudioClip> Music;
    private Dictionary<string, AudioClip> Ambient;
    private Dictionary<string, AudioClip> Sound;

    [Serializable]
    public struct MusicArr
    {
        public string name;
        public AudioClip clip;
    }
    public MusicArr[] MusicData;

    [Serializable]
    public struct AmbientArr
    {
        public string name;
        public AudioClip clip;
    }
    public AmbientArr[] AmbientData;

    [Serializable]
    public struct SoundArr
    {
        public string name;
        public AudioClip clip;
    }
    public SoundArr[] SoundData;

    void Awake()
    {
        AudioListener.volume = 1f;

        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }

        Music = new Dictionary<string, AudioClip>();
        Ambient = new Dictionary<string, AudioClip>();
        Sound = new Dictionary<string, AudioClip>();

        foreach (var data in MusicData)
        {
            Music.Add(data.name, data.clip);
        }

        foreach (var data in AmbientData)
        {
            Ambient.Add(data.name, data.clip);
        }

        foreach (var data in SoundData)
        {
            Sound.Add(data.name, data.clip);
        }
    }

    // Music

    // Включает музыку с 0й громкости до громкости настроек
    public void MusicStart(string name, float duration, float targetVol = 1)
    {
        UserData.instance.CurrentMusic = name;
        UserData.instance.MusicSourceVolume = targetVol;

        musicSource.volume = 0;
        musicSource.clip = Music[name];
        musicSource.Play();

        if (duration == 0)
        {
            musicSource.volume = targetVol;
        }
        else
        {
            StartCoroutine(StartFade(musicSource, duration, targetVol));
        }
    }

    public void MusicEnd(float duration)
    {
        StartCoroutine(IMusicEnd(duration));
    }

    private IEnumerator IMusicEnd(float duration)
    {
        UserData.instance.CurrentMusic = null;
        UserData.instance.MusicSourceVolume = 0;

        if (musicSource.isPlaying)
        {
            if (duration == 0)
            {
                musicSource.volume = 0;
                musicSource.Stop();
                yield break;
            }

            yield return StartCoroutine(StartFade(musicSource, duration, 0));
            musicSource.Stop();
        }
        else if (musicBuffSource.isPlaying)
        {
            if (duration == 0)
            {
                musicBuffSource.volume = 0;
                musicBuffSource.Stop();
                yield break;
            }

            yield return StartCoroutine(StartFade(musicBuffSource, duration, 0));
            musicBuffSource.Stop();
        }
    }

    // Переход трека в трек
    public void MusicTransition(string name, float duration)
    {
        StartCoroutine(IMusicTransition(name, duration));
    }

    private IEnumerator IMusicTransition(string name, float duration)
    {
        UserData.instance.CurrentMusic = name;

        if (musicSource.isPlaying)
        {
            float curr_vol = musicSource.volume;
            UserData.instance.MusicSourceVolume = curr_vol;

            musicBuffSource.volume = 0;
            musicBuffSource.clip = Music[name];
            musicBuffSource.Play();
            StartCoroutine(StartFade(musicBuffSource, duration, curr_vol));

            yield return StartCoroutine(StartFade(musicSource, duration, 0));
            musicSource.Stop();
        }
        else if (musicBuffSource.isPlaying)
        {
            float curr_vol = musicBuffSource.volume;
            UserData.instance.MusicSourceVolume = curr_vol;

            musicSource.volume = 0;
            musicSource.clip = Music[name];
            musicSource.Play();
            StartCoroutine(StartFade(musicSource, duration, curr_vol));

            yield return StartCoroutine(StartFade(musicBuffSource, duration, 0));
            musicBuffSource.Stop();
        }
    }

    // Трек -> тишина -> трек
    public void MusicChange(string name, float duration)
    {
        StartCoroutine(IMusicChange(name, duration));
    }

    private IEnumerator IMusicChange(string name, float duration)
    {
        UserData.instance.CurrentMusic = name;
        float curr_vol = 1; // Заглушка

        if (musicSource.isPlaying)
        {
            curr_vol = musicSource.volume;
            yield return StartCoroutine(StartFade(musicSource, duration, 0));
            musicSource.Stop();
        }
        else
        {
            curr_vol = musicSource.volume;
            yield return StartCoroutine(StartFade(musicBuffSource, duration, 0));
            musicBuffSource.Stop();
        }

        yield return new WaitForSeconds(1f); // Задержка между треками

        musicSource.volume = 0;
        musicSource.clip = Music[name];
        musicSource.Play();

        StartCoroutine(StartFade(musicSource, duration, curr_vol));
    }

    public void MusicVolChange(float duration, float factor) // 0 to 1
    {
        UserData.instance.MusicSourceVolume = factor;

        if (musicSource.isPlaying)
        {
            StartCoroutine(StartFade(musicSource, duration, factor));
        }
        else
        {
            StartCoroutine(StartFade(musicBuffSource, duration, factor));
        }
    }

    public void MusicVolDefault(float duration)
    {
        UserData.instance.MusicSourceVolume = 1;

        if (musicSource.isPlaying)
        {
            StartCoroutine(StartFade(musicSource, duration, 1));
        }
        else
        {
            StartCoroutine(StartFade(musicBuffSource, duration, 1));
        }
    }

    // Ambient
    public void AmbientStart(string name, float duration, float targetVol = 1)
    {
        UserData.instance.CurrentAmbient = name;
        UserData.instance.AmbientSourceVolume = targetVol;

        ambientSource.volume = 0;
        ambientSource.clip = Ambient[name];
        ambientSource.Play();

        if (duration == 0)
        {
            ambientSource.volume = targetVol;
        }
        else
        {
            StartCoroutine(StartFade(ambientSource, duration, targetVol));
        }
    }

    public void AmbientEnd(float duration)
    {
        StartCoroutine(IAmbientEnd(duration));
    }

    private IEnumerator IAmbientEnd(float duration)
    {
        UserData.instance.CurrentAmbient = null;
        UserData.instance.AmbientSourceVolume = 0;

        if (duration == 0)
        {
            ambientSource.Stop();
            yield break;
        }

        yield return StartCoroutine(StartFade(ambientSource, duration, 0));
        ambientSource.Stop();
    }

    public void AmbientChange(string name, float duration)
    {
        StartCoroutine(IAmbientChange(name, duration));
    }

    private IEnumerator IAmbientChange(string name, float duration)
    {
        UserData.instance.CurrentAmbient = name;

        float curr_vol = ambientSource.volume;
        yield return StartCoroutine(StartFade(ambientSource, duration, 0));
        ambientSource.Stop();

        yield return new WaitForSeconds(1f); // Задержка между треками

        ambientSource.clip = Ambient[name];
        ambientSource.Play();

        StartCoroutine(StartFade(ambientSource, duration, curr_vol));
    }

    public void AmbientVolChange(float duration, float factor) // 0 to 1
    {
        UserData.instance.AmbientSourceVolume = factor;
        StartCoroutine(StartFade(ambientSource, duration, factor));
    }

    public void AmbientVolDefault(float duration)
    {
        UserData.instance.AmbientSourceVolume = 1;
        StartCoroutine(StartFade(ambientSource, duration, 1));
    }

    // Sound
    public void SoundStart(string name)
    {
        soundSource.clip = Sound[name];
        soundSource.Play();
    }

    public IEnumerator StartFade(AudioSource source, float duration, float targetVolume_linear)
    {
        if (duration == 0)
        {
            source.volume = targetVolume_linear;
            yield break;
        }
        else
        {
            float currentTime = 0;
            float currentVolume_linear = source.volume;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                source.volume = Mathf.Lerp(currentVolume_linear, targetVolume_linear, currentTime / duration);
                yield return null;
            }
            yield break;
        }
    }
}
