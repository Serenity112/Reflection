using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

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
    public struct AudioArr
    {
        public string name;
        public AudioClip clip;
    }

    public AudioArr[] MusicData;
    public AudioArr[] AmbientData;
    public AudioArr[] SoundData;

    // int ��� �������������� �������, bool �� �������
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

    public IEnumerator FadeOutCurrent()
    {
        StartCoroutine(StartFade(musicSource, 0.4f, 0));
        StartCoroutine(StartFade(musicBuffSource, 0.4f, 0));
        StartCoroutine(StartFade(ambientSource, 0.4f, 0));
        yield return StartCoroutine(StartFade(soundSource, 0.4f, 0));

        ClearCurrent();
    }

    public void ClearCurrent()
    {
        musicSource.Stop();
        musicSource.clip = null;

        musicBuffSource.Stop();
        musicBuffSource.clip = null;

        ambientSource.Stop();
        ambientSource.clip = null;

        soundSource.Stop();
        soundSource.clip = null;
    }

    // ����� ��� �������� ������������, ������� ������ ��������� � ���������� ���������. ���� ��� �������, ������ �� ������
    // �� ����������. ���� ������ �������� ��� �������� �����-���������, ���� ��� ���������� �������
    private void InterruptCurrent()
    {
        if (_isFading != 0)
        {
            ClearCurrent();
        }

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


    // �������� ������ � 0� ��������� �� ��������� ��������
    public void MusicStart(string name, float duration, float targetVol = 1)
    {
        if (!Music.ContainsKey(name))
        {
            return;
        }

        InterruptCurrent();

        UserData.instance.CurrentMusic = name;
        UserData.instance.MusicSourceVolume = targetVol;

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

    public void MusicEnd(float duration) => StartCoroutine(IMusicEnd(duration));

    private IEnumerator IMusicEnd(float duration)
    {
        InterruptCurrent();

        UserData.instance.CurrentMusic = null;
        UserData.instance.MusicSourceVolume = 0;

        AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBuffSource;

        if (duration == 0 || DialogMod.skipping)
        {
            activeSource.volume = 0;
            activeSource.Stop();
        }
        else
        {
            yield return StartCoroutine(StartFade(activeSource, duration, 0));
            activeSource.Stop();
        }
    }

    // ������� ����� � ����
    public void MusicTransition(string name, float duration) => StartCoroutine(IMusicTransition(name, duration));

    private IEnumerator IMusicTransition(string name, float duration)
    {
        if (!Music.ContainsKey(name))
        {
            yield break;
        }

        InterruptCurrent();

        UserData.instance.CurrentMusic = name;

        AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBuffSource;
        AudioSource newSource = musicSource.isPlaying ? musicBuffSource : musicSource;

        float curr_vol = UserData.instance.MusicSourceVolume;

        newSource.volume = 0;
        newSource.clip = Music[name];
        newSource.Play();

        if (duration == 0 || DialogMod.skipping)
        {
            newSource.volume = curr_vol;
            activeSource.volume = 0;
            activeSource.Stop();
        }
        else
        {
            StartCoroutine(StartFade(newSource, duration, curr_vol));
            yield return StartCoroutine(StartFade(activeSource, duration, 0));
            activeSource.Stop();
        }
    }

    // ���� -> ������ -> ����
    public void MusicChange(string name, float duration) => StartCoroutine(IMusicChange(name, duration));

    private IEnumerator IMusicChange(string name, float duration)
    {
        if (!Music.ContainsKey(name))
        {
            yield break;
        }

        InterruptCurrent();

        UserData.instance.CurrentMusic = name;       

        AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBuffSource;

        float curr_vol = activeSource.volume;

        if (duration == 0 || DialogMod.skipping)
        {
            activeSource.volume = 0;
            activeSource.Stop();
            musicSource.clip = Music[name];
            musicSource.volume = curr_vol;
            musicSource.Play();
        }
        else
        {
            yield return StartCoroutine(StartFade(activeSource, duration, 0));
            musicSource.Stop();

            yield return new WaitForSeconds(1f); // �������� ����� �������

            musicSource.volume = 0;
            musicSource.clip = Music[name];
            musicSource.Play();

            yield return StartCoroutine(StartFade(musicSource, duration, curr_vol));
        }
    }

    public void MusicVolChange(float duration, float factor) // factor is 0 to 1
    {
        InterruptCurrent();

        UserData.instance.MusicSourceVolume = factor;

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
        InterruptCurrent();

        UserData.instance.MusicSourceVolume = 1;

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
        if (!Ambient.ContainsKey(name))
        {
            return;
        }

        InterruptCurrent();

        UserData.instance.CurrentAmbient = name;
        UserData.instance.AmbientSourceVolume = targetVol;

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

    public void AmbientEnd(float duration) => StartCoroutine(IAmbientEnd(duration));

    private IEnumerator IAmbientEnd(float duration)
    {
        if (!Ambient.ContainsKey(name))
        {
            yield break;
        }

        InterruptCurrent();

        UserData.instance.CurrentAmbient = null;
        UserData.instance.AmbientSourceVolume = 0;

        if (duration == 0 || DialogMod.skipping)
        {
            ambientSource.volume = 0;
            ambientSource.Stop();
        }
        else
        {
            yield return StartCoroutine(StartFade(ambientSource, duration, 0));
            ambientSource.Stop();
        }
    }

    public void AmbientChange(string name, float duration) => StartCoroutine(IAmbientChange(name, duration));

    private IEnumerator IAmbientChange(string name, float duration)
    {
        if (!Ambient.ContainsKey(name))
        {
            yield break;
        }

        InterruptCurrent();

        UserData.instance.CurrentAmbient = name;

        float curr_vol = ambientSource.volume;

        if (duration == 0 || DialogMod.skipping)
        {
            ambientSource.clip = Ambient[name];
            ambientSource.Play();
        }
        else
        {
            yield return StartCoroutine(StartFade(ambientSource, duration, 0));
            ambientSource.Stop();

            yield return new WaitForSeconds(1f); // �������� ����� �������

            ambientSource.clip = Ambient[name];
            ambientSource.Play();

            StartCoroutine(StartFade(ambientSource, duration, curr_vol));
        }
    }

    public void AmbientVolChange(float duration, float factor) // 0 to 1
    {
        InterruptCurrent();

        UserData.instance.AmbientSourceVolume = factor;

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
        InterruptCurrent();

        UserData.instance.AmbientSourceVolume = 1;

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
        if (!Sound.ContainsKey(name))
        {
            return;
        }

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
