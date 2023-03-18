using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public AudioSource musicSource;
    public AudioSource musicBuffSource;
    public AudioSource ambientSource;
    public AudioSource soundSource;

    public int currentMusic;
    public int currentSound;
    public int currentAmbient;

    public AudioClip[] Music;
    public AudioClip[] Sound;
    public AudioClip[] Ambient;

    void Start()
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
    }

    public void ApplySourceVolume(Settings setting, float data)
    {
        switch (setting)
        {
            case Settings.sourceMusicVolume:
                musicSource.volume = data;
                break;
            case Settings.sourceMusicBufferVolume:
                musicBuffSource.volume = data;
                break;
            case Settings.sourceSoundVolume:
                soundSource.volume = data;
                break;
            case Settings.sourceAmbientVolume:
                ambientSource.volume = data;
                break;
        }
    }

    // Music

    // Включает музыку с 0й громкости до громкости настроек
    public void MusicStart(int num, float duration, float targetVol = 1)
    {
        currentMusic = num;
        musicSource.volume = 0;
        musicSource.clip = Music[num];
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
        if (musicSource.isPlaying)
        {
            if (duration == 0)
            {
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
                musicBuffSource.Stop();
                yield break;
            }

            yield return StartCoroutine(StartFade(musicBuffSource, duration, 0));
            musicBuffSource.Stop();
        }
    }

    // Переход трека в трек
    public void MusicTransition(int num, float duration)
    {
        StartCoroutine(IMusicTransition(num, duration));
    }

    private IEnumerator IMusicTransition(int num, float duration)
    {
        currentMusic = num;

        if (musicSource.isPlaying)
        {
            float curr_vol = musicSource.volume;

            musicBuffSource.volume = 0;
            musicBuffSource.clip = Music[num];
            musicBuffSource.Play();
            StartCoroutine(StartFade(musicBuffSource, duration, curr_vol));

            yield return StartCoroutine(StartFade(musicSource, duration, 0));
            musicSource.Stop();
        }
        else if (musicBuffSource.isPlaying)
        {
            float curr_vol = musicBuffSource.volume;

            musicSource.volume = 0;
            musicSource.clip = Music[num];
            musicSource.Play();
            StartCoroutine(StartFade(musicSource, duration, curr_vol));

            yield return StartCoroutine(StartFade(musicBuffSource, duration, 0));
            musicBuffSource.Stop();
        }
    }

    // Трек -> тишина -> трек
    public void MusicChange(int num, float duration)
    {
        StartCoroutine(IMusicChange(num, duration));
    }

    private IEnumerator IMusicChange(int num, float duration)
    {
        currentMusic = num;
        float curr_vol = 1;

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
        musicSource.clip = Music[num];
        musicSource.Play();

        StartCoroutine(StartFade(musicSource, duration, curr_vol));
    }

    public void MusicVolChange(float duration, float factor) // 0 to 1
    {
        if (musicSource.isPlaying)
        {
            StartCoroutine(StartFade(musicSource, duration, factor));
            musicSource.Stop();
        }
        else
        {
            StartCoroutine(StartFade(musicBuffSource, duration, factor));
            musicBuffSource.Stop();
        }
    }

    public void MusicVolDefault(float duration)
    {
        if (musicSource.isPlaying)
        {
            StartCoroutine(StartFade(musicSource, duration, 1));
            musicSource.Stop();
        }
        else
        {
            StartCoroutine(StartFade(musicBuffSource, duration, 1));
            musicBuffSource.Stop();
        }
    }

    // Ambient
    public void AmbientStart(int num, float duration, float targetVol = 1)
    {
        currentAmbient = num;
        ambientSource.volume = 0;
        ambientSource.clip = Ambient[num];
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
        if (duration == 0)
        {
            ambientSource.Stop();
            yield break;
        }

        yield return StartCoroutine(StartFade(ambientSource, duration, 0));
        ambientSource.Stop();
    }

    public void AmbientChange(int num, float duration)
    {
        StartCoroutine(IAmbientChange(num, duration));
    }
    
    private IEnumerator IAmbientChange(int num, float duration)
    {
        currentAmbient = num;

        float curr_vol = ambientSource.volume;
        yield return StartCoroutine(StartFade(ambientSource, duration, 0));
        ambientSource.Stop();

        yield return new WaitForSeconds(1f); // Задержка между треками

        ambientSource.clip = Ambient[num];
        ambientSource.Play();

        StartCoroutine(StartFade(ambientSource, duration, curr_vol));
    }

    // Sound
    public void SoundStart(int sound)
    {
        soundSource.clip = Sound[sound];
        soundSource.Play();
    }

    public IEnumerator StartFade(AudioSource source, float duration, float targetVolume_linear)
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
