using Fungus;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public struct AudioArr
    {
        public string name;
        public AudioClip clip;
    }

    private enum AudioType
    {
        Music,
        Ambient,
    }

    public AudioArr[] MusicData;
    public AudioArr[] AmbientData;
    public AudioArr[] SoundData;

    // Два иенумератора нужны т.к. не всегда идёт одна анимация, а объединить их в один вариант пока не найден
    private IEnumerator _IMusic1;
    private IEnumerator _IMusic2;
    private IEnumerator _IAmbient;

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
        float time = 0.4f;
        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            LinearFade(musicSource, time, 0),
            LinearFade(musicBuffSource, time, 0),
            LinearFade(ambientSource, time, 0),
            LinearFade(soundSource, time, 0),
        });
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

    // Включает музыку с 0й громкости до громкости настроек
    public void MusicStart(string name, float duration, float volume = 1)
    {
        if (!Music.ContainsKey(name))
        {
            return;
        }

        UserData.instance.CurrentMusic = name;
        UserData.instance.MusicSourceVolume = volume;

        musicSource.volume = 0;
        musicSource.clip = Music[name];
        musicSource.Play();

        if (_IMusic1 != null)
        {
            StopCoroutine(_IMusic1);
        }
        if (_IMusic2 != null)
        {
            StopCoroutine(_IMusic2);
        }

        if (duration == 0 || DialogMod.skipping)
        {
            musicSource.volume = volume;
        }
        else
        {
            _IMusic1 = LinearFade(musicSource, duration, volume);
            StartCoroutine(_IMusic1);
        }
    }

    public void MusicEnd(float duration) => StartCoroutine(IMusicEnd(duration));

    private IEnumerator IMusicEnd(float duration)
    {
        UserData.instance.CurrentMusic = null;
        UserData.instance.MusicSourceVolume = 0;

        if (_IMusic1 != null)
        {
            StopCoroutine(_IMusic1);
        }
        if (_IMusic2 != null)
        {
            StopCoroutine(_IMusic2);
        }

        if (duration == 0 || DialogMod.skipping)
        {
            musicSource.volume = 0;
            musicSource.Stop();

            musicBuffSource.volume = 0;
            musicBuffSource.Stop();
        }
        else
        {
            _IMusic1 = LinearFade(musicSource, duration, 0);
            _IMusic2 = LinearFade(musicBuffSource, duration, 0);

            StartCoroutine(_IMusic1);
            yield return StartCoroutine(_IMusic2);

            musicSource.Stop();
            musicBuffSource.Stop();
        }
    }

    // Переход трека в трек
    public void MusicTransition(string name, float duration) => StartCoroutine(IMusicTransition(name, duration));

    private IEnumerator IMusicTransition(string name, float duration)
    {
        if (!Music.ContainsKey(name))
        {
            yield break;
        }

        UserData.instance.CurrentMusic = name;

        AudioSource oldSource = musicSource.isPlaying ? musicSource : musicBuffSource;
        AudioSource newSource = musicSource.isPlaying ? musicBuffSource : musicSource;

        float curr_vol = UserData.instance.MusicSourceVolume;

        newSource.volume = 0;
        newSource.clip = Music[name];
        newSource.Play();

        if (_IMusic1 != null)
        {
            StopCoroutine(_IMusic1);
        }
        if (_IMusic2 != null)
        {
            StopCoroutine(_IMusic2);
        }

        if (duration == 0 || DialogMod.skipping)
        {
            newSource.volume = curr_vol;
            oldSource.volume = 0;
            oldSource.Stop();
        }
        else
        {
            _IMusic1 = LinearFade(newSource, duration, curr_vol);
            _IMusic2 = LinearFade(oldSource, duration, 0);

            StartCoroutine(_IMusic1);
            yield return StartCoroutine(_IMusic2);

            oldSource.Stop();
        }
    }

    // Трек -> тишина -> трек
    public void MusicChange(string name, float duration) => StartCoroutine(IMusicChange(name, duration));

    private IEnumerator IMusicChange(string name, float duration)
    {
        if (!Music.ContainsKey(name))
        {
            yield break;
        }

        UserData.instance.CurrentMusic = name;

        AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBuffSource;

        float curr_vol = activeSource.volume;

        if (_IMusic1 != null)
        {
            StopCoroutine(_IMusic1);
        }
        if (_IMusic2 != null)
        {
            StopCoroutine(_IMusic2);
        }

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
            _IMusic1 = LinearFade(activeSource, duration, 0);
            yield return StartCoroutine(_IMusic1);
            musicSource.Stop();

            yield return new WaitForSeconds(0f); // Задержка между треками

            musicSource.volume = 0;
            musicSource.clip = Music[name];
            musicSource.Play();

            _IMusic1 = LinearFade(musicSource, duration, curr_vol);
            yield return StartCoroutine(_IMusic1);
        }
    }

    public void MusicVolChange(float duration, float volume) // volume от 0 до 1
    {
        UserData.instance.MusicSourceVolume = volume;

        AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBuffSource;

        if (_IMusic1 != null)
        {
            StopCoroutine(_IMusic1);
        }
        if (_IMusic2 != null)
        {
            StopCoroutine(_IMusic2);
        }

        if (duration == 0 || DialogMod.skipping)
        {
            activeSource.volume = volume;
        }
        else
        {
            _IMusic1 = LinearFade(activeSource, duration, volume);
            StartCoroutine(_IMusic1);
        }
    }

    public void MusicVolDefault(float duration)
    {
        UserData.instance.MusicSourceVolume = 1;

        AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBuffSource;

        if (_IMusic1 != null)
        {
            StopCoroutine(_IMusic1);
        }
        if (_IMusic2 != null)
        {
            StopCoroutine(_IMusic2);
        }

        if (duration == 0 || DialogMod.skipping)
        {
            activeSource.volume = 1;
        }
        else
        {
            _IMusic1 = LinearFade(activeSource, duration, 1);
            StartCoroutine(_IMusic1);
        }
    }

    // Ambient
    public void AmbientStart(string name, float duration, float volume = 1)
    {
        if (!Ambient.ContainsKey(name))
        {
            return;
        }

        UserData.instance.CurrentAmbient = name;
        UserData.instance.AmbientSourceVolume = volume;

        ambientSource.volume = 0;
        ambientSource.clip = Ambient[name];
        ambientSource.Play();

        if (_IAmbient != null)
        {
            StopCoroutine(_IAmbient);
        }

        if (duration == 0 || DialogMod.skipping)
        {
            ambientSource.volume = volume;
        }
        else
        {
            _IAmbient = LinearFade(ambientSource, duration, volume);
            StartCoroutine(_IAmbient);
        }
    }

    public void AmbientEnd(float duration) => StartCoroutine(IAmbientEnd(duration));

    private IEnumerator IAmbientEnd(float duration)
    {
        if (!Ambient.ContainsKey(name))
        {
            yield break;
        }

        UserData.instance.CurrentAmbient = null;
        UserData.instance.AmbientSourceVolume = 0;

        if (_IAmbient != null)
        {
            StopCoroutine(_IAmbient);
        }

        if (duration == 0 || DialogMod.skipping)
        {
            ambientSource.volume = 0;
            ambientSource.Stop();
        }
        else
        {
            _IAmbient = LinearFade(ambientSource, duration, 0);
            yield return StartCoroutine(_IAmbient);
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

        UserData.instance.CurrentAmbient = name;

        float curr_vol = ambientSource.volume;

        if (_IAmbient != null)
        {
            StopCoroutine(_IAmbient);
        }

        if (duration == 0 || DialogMod.skipping)
        {
            ambientSource.clip = Ambient[name];
            ambientSource.Play();
        }
        else
        {
            _IAmbient = LinearFade(ambientSource, duration, 0);
            yield return StartCoroutine(_IAmbient);
            ambientSource.Stop();

            yield return new WaitForSeconds(0f); // Задержка между треками

            ambientSource.clip = Ambient[name];
            ambientSource.Play();

            _IAmbient = LinearFade(ambientSource, duration, curr_vol);
            StartCoroutine(_IAmbient);
        }
    }

    public void AmbientVolChange(float duration, float volume) // 0 to 1
    {
        UserData.instance.AmbientSourceVolume = volume;

        if (_IAmbient != null)
        {
            StopCoroutine(_IAmbient);
        }

        if (duration == 0 || DialogMod.skipping)
        {
            ambientSource.volume = volume;
        }
        else
        {
            _IAmbient = LinearFade(ambientSource, duration, volume);
            StartCoroutine(_IAmbient);
        }
    }

    public void AmbientVolDefault(float duration)
    {
        UserData.instance.AmbientSourceVolume = 1;

        if (_IAmbient != null)
        {
            StopCoroutine(_IAmbient);
        }

        if (duration == 0 || DialogMod.skipping)
        {
            ambientSource.volume = 1;
        }
        else
        {
            _IAmbient = LinearFade(ambientSource, duration, 1);
            StartCoroutine(_IAmbient);
        }
    }

    // Sound
    public void SoundStart(string name)
    {
        if (!Sound.ContainsKey(name))
        {
            return;
        }

        soundSource.volume = 0;
        soundSource.clip = Sound[name];
        soundSource.Play();
    }

    public IEnumerator LinearFade(AudioSource source, float duration, float targetVolume_linear)
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
