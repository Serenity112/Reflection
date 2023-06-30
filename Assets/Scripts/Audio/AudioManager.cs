using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public AudioSource musicSource;
    public AudioSource musicBufferSource;

    public AudioSource ambientSource;
    public AudioSource ambientBufferSource;

    public AudioSource soundSource;

    private Dictionary<string, AudioClip> Music;
    private Dictionary<string, AudioClip> Ambient;
    private Dictionary<string, AudioClip> Sound;

    // Нужны для определения дорожек
    private string _ambient1name;
    private string _ambient2name;

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

    private IEnumerator _IAmbient1;
    private IEnumerator _IAmbient2;

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
    // Метод для отгрузки при загрузкит сейв файла
    public IEnumerator FadeOutCurrent()
    {
        float time = 0.4f;
        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            LinearFade(musicSource, time, 0),
            LinearFade(musicBufferSource, time, 0),
            LinearFade(ambientSource, time, 0),
            LinearFade(ambientBufferSource, time, 0),
            LinearFade(soundSource, time, 0),
        });
        ClearCurrent();
    }

    public void ClearCurrent()
    {
        musicSource.Stop();
        musicSource.clip = null;

        musicBufferSource.Stop();
        musicBufferSource.clip = null;

        ambientSource.Stop();
        ambientSource.clip = null;

        ambientBufferSource.Stop();
        ambientBufferSource.clip = null;

        soundSource.Stop();
        soundSource.clip = null;
    }

    // Включает музыку с 0й громкости до громкости настроек
    public void MusicStart(string name, float duration, float volume)
    {
        if (!Music.ContainsKey(name))
        {
            return;
        }

        UserData.instance.CurrentMusic = (name, volume);

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
        UserData.instance.CurrentMusic = (null, 0);

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

            musicBufferSource.volume = 0;
            musicBufferSource.Stop();
        }
        else
        {
            _IMusic1 = LinearFade(musicSource, duration, 0);
            _IMusic2 = LinearFade(musicBufferSource, duration, 0);

            yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
            {
                _IMusic1,
                _IMusic2,
            });

            musicSource.Stop();
            musicBufferSource.Stop();
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

        float curr_vol = UserData.instance.CurrentMusic.Volume;

        UserData.instance.CurrentMusic = (name, curr_vol);

        AudioSource oldSource = musicSource.isPlaying ? musicSource : musicBufferSource;
        AudioSource newSource = musicSource.isPlaying ? musicBufferSource : musicSource;

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

            yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
            {
                _IMusic1,
                _IMusic2,
            });

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

        AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBufferSource;

        float curr_vol = activeSource.volume;

        UserData.instance.CurrentMusic = (name, curr_vol);

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
        UserData.instance.CurrentMusic = (UserData.instance.CurrentMusic.Name, volume);

        AudioSource activeSource = musicSource.isPlaying ? musicSource : musicBufferSource;

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

    public void MusicVolDefault(float duration) => MusicVolChange(duration, 1);


    // Ambient
    public void AmbientStart(string name, float duration, float volume = 1, bool newLine = false)
    {
        if (!Ambient.ContainsKey(name))
        {
            return;
        }

        AudioSource sourceToPlay;
        if (newLine)
        {
            _ambient2name = name;
            UserData.instance.CurrentAmbient2 = (name, volume);
            sourceToPlay = ambientSource.isPlaying ? ambientBufferSource : ambientSource;
        }
        else
        {
            _ambient1name = name;
            UserData.instance.CurrentAmbient1 = (name, volume);
            sourceToPlay = ambientSource.isPlaying ? ambientSource : ambientBufferSource;
        }

        sourceToPlay.volume = 0;
        sourceToPlay.clip = Ambient[name];
        sourceToPlay.Play();

        if (_IAmbient1 != null)
        {
            StopCoroutine(_IAmbient1);
        }
        if (_IAmbient2 != null)
        {
            StopCoroutine(_IAmbient2);
        }

        if (duration == 0 || DialogMod.skipping)
        {
            sourceToPlay.volume = volume;
        }
        else
        {
            IEnumerator play = LinearFade(sourceToPlay, duration, volume);
            if (newLine)
            {
                _IAmbient2 = play;
                StartCoroutine(_IAmbient2);
            }
            else
            {
                _IAmbient1 = play;
                StartCoroutine(_IAmbient1);
            }
        }
    }

    public void AmbientEnd(string name, float duration) => StartCoroutine(IAmbientEnd(name, duration));

    private IEnumerator IAmbientEnd(string name, float duration)
    {
        if (!Ambient.ContainsKey(name))
        {
            yield break;
        }

        AudioSource sourceToEnd;
        if (name == _ambient1name)
        {
            sourceToEnd = ambientSource;
            UserData.instance.CurrentAmbient1 = (null, 0);
        }
        else
        {
            sourceToEnd = ambientBufferSource;
            UserData.instance.CurrentAmbient2 = (null, 0);
        }

        if (_IAmbient1 != null)
        {
            StopCoroutine(_IAmbient1);
        }
        if (_IAmbient2 != null)
        {
            StopCoroutine(_IAmbient2);
        }

        if (duration == 0 || DialogMod.skipping)
        {
            sourceToEnd.volume = 0;
            sourceToEnd.Stop();
        }
        else
        {
            _IAmbient1 = LinearFade(sourceToEnd, duration, 0);
            yield return StartCoroutine(_IAmbient1);
            sourceToEnd.Stop();
        }
    }

    public void AmbientVolChange(string name, float duration, float volume) // 0 to 1
    {
        //UserData.instance.AmbientSourceVolume = volume;

        if (_IAmbient1 != null)
        {
            StopCoroutine(_IAmbient1);
        }

        if (duration == 0 || DialogMod.skipping)
        {
            ambientSource.volume = volume;
        }
        else
        {
            _IAmbient1 = LinearFade(ambientSource, duration, volume);
            StartCoroutine(_IAmbient1);
        }
    }

    public void AmbientVolDefault(string name, float duration) => AmbientVolChange(name, duration, 1);

    public void AmbientTransition(string name, float duration)
    {

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

    public IEnumerator LinearFade(AudioSource source, float fadeTime, float targetVolume_linear)
    {
        float currentTime = 0;
        float currentVolume_linear = source.volume;
        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(currentVolume_linear, targetVolume_linear, currentTime / fadeTime);
            yield return null;
        }

        yield break;
    }
}
