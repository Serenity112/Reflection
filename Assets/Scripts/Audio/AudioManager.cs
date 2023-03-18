using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using System.Numerics;

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

    // int для накопительного эффекта, bool не подойдёт
    public int _isFading = 0;

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
    public IEnumerator UnloadCurrent()
    {
        StartCoroutine(StartFade(musicSource, 0.4f, 0));
        StartCoroutine(StartFade(musicBuffSource, 0.4f, 0));
        StartCoroutine(StartFade(ambientSource, 0.4f, 0));
        yield return StartCoroutine(StartFade(soundSource, 0.4f, 0));

        musicSource.clip = null;
        musicBuffSource.clip = null;
        ambientSource.clip = null;
        soundSource.clip = null;
        yield return null;
    }

    // Метод для быстрого закликивания, который ставит источники в актуальное положение. Если нет заклика, ничего не делает
    // НЕ ДОРАБОТАНО
    private void LoadCurrent()
    {
        return;

        if (_isFading != 0)
        {
            AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBuffSource;

            string currMusic = UserData.instance.CurrentMusic;
            if (currMusic != null)
            {
                musicSource.Stop();
                musicBuffSource.Stop();

                activeSource.volume = UserData.instance.MusicSourceVolume;
                musicSource.clip = Music[currMusic];
                activeSource.Play();
            }

            string currAmbient = UserData.instance.CurrentAmbient;
            if (currAmbient != null)
            {
                ambientSource.Stop();

                ambientSource.volume = UserData.instance.AmbientSourceVolume;
                ambientSource.clip = Ambient[currAmbient];

                ambientSource.Play();
            }
        }
    }


    // Включает музыку с 0й громкости до громкости настроек
    public void MusicStart(string name, float duration, float targetVol = 1)
    {
        UserData.instance.CurrentMusic = name;
        UserData.instance.MusicSourceVolume = targetVol;

        LoadCurrent();

        musicSource.volume = 0;
        musicSource.clip = Music[name];
        musicSource.Play();

        if (duration == 0 || DialogMod.skipping)
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

        LoadCurrent();

        AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBuffSource;

        if (duration == 0 || DialogMod.skipping)
        {
            activeSource.volume = 0;
            activeSource.Stop();
            yield break;
        }

        yield return StartCoroutine(StartFade(activeSource, duration, 0));
        activeSource.Stop();
    }

    // Переход трека в трек
    public void MusicTransition(string name, float duration)
    {
        StartCoroutine(IMusicTransition(name, duration));
    }

    private IEnumerator IMusicTransition(string name, float duration)
    {
        UserData.instance.CurrentMusic = name;

        AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBuffSource;
        AudioSource newSource = musicSource.isPlaying ? musicBuffSource : musicSource;

        float curr_vol = UserData.instance.MusicSourceVolume;

        LoadCurrent();

        newSource.volume = 0;
        newSource.clip = Music[name];
        newSource.Play();

        if (duration == 0 || DialogMod.skipping)
        {
            newSource.volume = curr_vol;
            activeSource.volume = 0;
            activeSource.Stop();
            yield break;
        }

        StartCoroutine(StartFade(newSource, duration, curr_vol));
        yield return StartCoroutine(StartFade(activeSource, duration, 0));
        activeSource.Stop();
    }

    // Трек -> тишина -> трек
    public void MusicChange(string name, float duration)
    {
        StartCoroutine(IMusicChange(name, duration));
    }

    private IEnumerator IMusicChange(string name, float duration)
    {      
        UserData.instance.CurrentMusic = name;

        LoadCurrent();

        AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBuffSource;

        float curr_vol = activeSource.volume;

        if (duration == 0 || DialogMod.skipping)
        {
            activeSource.volume = 0;
            activeSource.Stop();
            musicSource.clip = Music[name];
            musicSource.volume = curr_vol;
            musicSource.Play();
            yield break;
        }

        yield return StartCoroutine(StartFade(activeSource, duration, 0));
        musicSource.Stop();

        yield return new WaitForSeconds(1f); // Задержка между треками

        musicSource.volume = 0;
        musicSource.clip = Music[name];
        musicSource.Play();

        yield return StartCoroutine(StartFade(musicSource, duration, curr_vol));
    }

    public void MusicVolChange(float duration, float factor) // 0 to 1
    {
        UserData.instance.MusicSourceVolume = factor;

        LoadCurrent();

        AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBuffSource;

        if (duration == 0 || DialogMod.skipping)
        {
            activeSource.volume = factor;
        }
        else
        {
            StartCoroutine(StartFade(activeSource, duration, factor));
        }
    }

    public void MusicVolDefault(float duration)
    {
        UserData.instance.MusicSourceVolume = 1;

        LoadCurrent();

        AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBuffSource;

        if (duration == 0 || DialogMod.skipping)
        {
            activeSource.volume = 1;
        }
        else
        {
            StartCoroutine(StartFade(activeSource, duration, 1));
        }
    }

    // Ambient
    public void AmbientStart(string name, float duration, float targetVol = 1)
    {
        UserData.instance.CurrentAmbient = name;
        UserData.instance.AmbientSourceVolume = targetVol;

        LoadCurrent();

        ambientSource.volume = 0;
        ambientSource.clip = Ambient[name];
        ambientSource.Play();

        if (duration == 0 || DialogMod.skipping)
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

        LoadCurrent();

        if (duration == 0 || DialogMod.skipping)
        {
            ambientSource.volume = 0;
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

        LoadCurrent();

        float curr_vol = ambientSource.volume;

        if (duration == 0 || DialogMod.skipping)
        {
            ambientSource.clip = Ambient[name];
            ambientSource.Play();
            yield break;
        }

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

        LoadCurrent();

        if (duration == 0 || DialogMod.skipping)
        {
            ambientSource.volume = factor;
        }
        else
        {
            StartCoroutine(StartFade(ambientSource, duration, factor));
        }
    }

    public void AmbientVolDefault(float duration)
    {       
        UserData.instance.AmbientSourceVolume = 1;

        LoadCurrent();

        if (duration == 0 || DialogMod.skipping)
        {
            ambientSource.volume = 1;
        }
        else
        {
            StartCoroutine(StartFade(ambientSource, duration, 1));
        }
    }

    // Sound
    public void SoundStart(string name, float duration)
    {
        soundSource.volume = 0;
        soundSource.clip = Sound[name];
        soundSource.Play();

        if (duration == 0 || DialogMod.skipping)
        {
            soundSource.volume = 1;
        }
        else
        {
            StartCoroutine(StartFade(ambientSource, duration, 1));
        }
    }

    public IEnumerator StartFade(AudioSource source, float duration, float targetVolume_linear)
    {
        _isFading++;

        float currentTime = 0;
        float currentVolume_linear = source.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(currentVolume_linear, targetVolume_linear, currentTime / duration);
            yield return null;
        }

        _isFading--;
        yield break;
    }
}
