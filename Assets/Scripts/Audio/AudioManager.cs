using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public Slider sliderMain;
    public Slider sliderMusic;
    public Slider sliderAmbient;
    public Slider sliderSound;
   

    private float mainVol = 0f;
    private float musicVol = 0f;
    private float ambientVol = 0f;
    private float soundVol = 0f;

    public AudioMixer audioMixer;

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
        AudioListener.volume = 0.33f;

        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
    }

    public void LoadSliders()
    {
        //if (ES3.KeyExists("musicVol"))
        //{
        //  musicVol = ES3.Load<float>("musicVol");
        //    sliderMusic.value = musicVol;
        //}

        //if (ES3.KeyExists("SoundVolume"))
        //{
        //    SoundVolume = ES3.Load<float>("SoundVolume");
        //    sliderSound.value = SoundVolume;
        //}
    }

    public void SetLevelMain()
    {
        mainVol = sliderMain.value;
        audioMixer.SetFloat("MainVol", sliderMain.value);

        ES3.Save<float>("MainVol", sliderMain.value);
        if (sliderMain.value == -40)
        {
            audioMixer.SetFloat("MainVol", -80);
        }
    }
    public void SetLevelMusic()
    {
        musicVol = sliderMusic.value;
        audioMixer.SetFloat("MusicVol", sliderMusic.value);

        ES3.Save<float>("MusicVol", sliderMusic.value);

        if (sliderMusic.value == -40)
        {
            audioMixer.SetFloat("AudioVol", -180);
        }
    }
   
    public void SetLevelAmbient()
    {
        audioMixer.SetFloat("AmbientVol", sliderAmbient.value);
        ambientVol = sliderAmbient.value;

        ES3.Save<float>("AmbientVol", sliderAmbient.value);

        if (sliderAmbient.value == -40)
        {
            audioMixer.SetFloat("AmbientVol", -100);
        }
    }

    public void SetLevelSound()
    {
        audioMixer.SetFloat("SoundVol", sliderSound.value);
        soundVol = sliderSound.value;

        ES3.Save<float>("SoundVol", sliderSound.value);

        if (sliderSound.value == -40)
        {
            audioMixer.SetFloat("SoundVol", -100);
        }
    }

    //AUIDIO FUNCTONS

    //public void DecreaseVolume(float LowPercent, float duration, bool DownVol)
    //{


    //    if(DownVol)
    //    {
    //        StartCoroutine(StartFade("MasterVol", duration, NewVol));
    //    } else
    //    {
    //        StartCoroutine(StartFade("MasterVol", duration, 0f));
    //    }

    //}

    //AMBIENT
    public void AmbientStart(int num, float duration)
    {
        currentAmbient = num;
        audioMixer.SetFloat("AmbientVol", -80);
        ambientSource.clip = Ambient[num];
        ambientSource.Play();

        if (duration == 0)
        {
            audioMixer.SetFloat("AmbientVol", ambientVol);
        }
        else
        {
            StartCoroutine(StartFade("AmbientVol", duration, ambientVol));
        }
    }
    public void AmbientEnd(float duration)
    {
        StartCoroutine(IAmbientEnd(duration));
    }
    public void AmbientChange(int ambient, float durationOut, float durationWait, float durationIn)
    {
        StartCoroutine(IAmbientChange(ambient, durationOut, durationWait, durationIn));
    }
    public IEnumerator IAmbientEnd(float duration)
    {
        if (ambientSource.isPlaying)
        {
            if (duration == 0)
            {
                ambientSource.Stop();
                yield break;
            }

            yield return StartCoroutine(StartFade("AmbientVol", duration, -80f));
            ambientSource.Stop();
        }
    }
    public IEnumerator IAmbientChange(int ambient, float durationOut, float durationWait, float durationIn)
    {
        yield return StartCoroutine(StartFade("AmbientVol", durationOut, -80f));
        ambientSource.Stop();

        yield return new WaitForSeconds(durationWait);

        audioMixer.SetFloat("AmbientVol", -80);
        ambientSource.clip = Ambient[ambient];
        ambientSource.Play();

        StartCoroutine(StartFade("AmbientVol", durationIn, musicVol));
    }
    //SOUND
    public void SoundStart(int sound)
    {
        soundSource.clip = Sound[sound];
        soundSource.Play();
    }

    // Music
    public void MusicStart(int num, float duration)
    {
        currentMusic = num;
        audioMixer.SetFloat("MusicVol", -80);
        musicSource.clip = Music[num];
        musicSource.Play();

        if (duration == 0)
        {
            audioMixer.SetFloat("MusicVol", musicVol);
        }
        else
        {
            StartCoroutine(StartFade("MusicVol", duration, musicVol));
        }
    }
    public void MusicEnd(float duration)
    {
        StartCoroutine(IMusicEnd(duration));
    }
    public void MusicTransition(int num, float duration)
    {
        StartCoroutine(IMusicTransition(num, duration));
    }
    public void MusicStartNew(int num, float durationOut, float durationWait, float durationIn)
    {
        StartCoroutine(IMusicChange(num, durationOut, durationWait, durationIn));
    }
    public IEnumerator IMusicEnd(float duration)
    {
        if (musicSource.isPlaying)
        {
            if (duration == 0)
            {
                musicSource.Stop();
                yield break;
            }

            yield return StartCoroutine(StartFade("AudioVol", duration, -80f));
            musicSource.Stop();
        }
        else if (musicBuffSource.isPlaying)
        {
            if (duration == 0)
            {
                musicBuffSource.Stop();
                yield break;
            }

            yield return StartCoroutine(StartFade("AudioVol2", duration, -80f));
            musicBuffSource.Stop();
        }
    }
    public IEnumerator IMusicTransition(int num, float duration)
    {
        if (musicSource.isPlaying)
        {
            audioMixer.SetFloat("BufferMusicVol", -80);
            musicBuffSource.clip = Music[num];
            musicBuffSource.Play();

            yield return StartCoroutine(StartFade("BufferMusicVol", duration, musicVol));
            yield return StartCoroutine(StartFade("MusicVol", duration, -80f));
            musicSource.Stop();
        }
        else if (musicBuffSource.isPlaying)
        {
            audioMixer.SetFloat("MusicVol", -80);
            musicSource.clip = Music[num];
            musicSource.Play();

            yield return StartCoroutine(StartFade("MusicVol", duration, musicVol));
            yield return StartCoroutine(StartFade("BufferMusicVol", duration , -80f));
            musicBuffSource.Stop();
        }
    }
    public IEnumerator IMusicChange(int num, float durationOut, float durationWait, float durationIn)
    {
        if (musicSource.isPlaying)
        {
            yield return StartCoroutine(StartFade("MusicVol", durationOut, -80f));
            musicSource.Stop();
        }
        else
        {
            yield return StartCoroutine(StartFade("BufferMusicVol", durationOut, -80f));
            musicBuffSource.Stop();
        }

        yield return new WaitForSeconds(durationWait);

        audioMixer.SetFloat("MusicVol", -80);
        musicSource.clip = Music[num];
        musicSource.Play();

        StartCoroutine(StartFade("MusicVol", durationIn, musicVol));
    }

    //IENUMERATORS


    public IEnumerator StartFade(string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        audioMixer.GetFloat(exposedParam, out currentVol);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetVolume, currentTime / duration);
            audioMixer.SetFloat(exposedParam, newVol);
            yield return null;
        }
        yield break;
    }

}