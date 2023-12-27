using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static AudioManager;

public class AudioManager : MonoBehaviour
{
    public enum AudioLine
    {
        Music,
        Ambient,
        Sound,
    }

    public enum TransitionType
    {
        None,
        Start,
        End,
        Play,
        VolChange,
    }

    public static AudioManager instance;

    public GameObject MusicSourcesObject;
    public GameObject AmbientSourcesObject;
    public GameObject SoundSourcesObject;

    private int MusicSourceCount = 0;
    private int AmbientSourceCount = 0;
    private int SoundSourceCount = 0;

    public float currentMusicVolume = 1;
    public float currentAmbientVolume = 1;
    public float currentSoundVolume = 1;

    public struct AudioData
    {
        public AudioData(int count)
        {
            Count = count;

            transitionType = new TransitionType[count];
            OstData = new (string ostName, float volume)?[count];
            IEnumerators = new IEnumerator[count];
            Handlers = new AsyncOperationHandle<AudioClip>?[count];
            TodoActions = new Action[count];

            for (int i = 0; i < count; i++)
            {
                OstData[i] = null;
                IEnumerators[i] = null;
                Handlers[i] = null;
                transitionType[i] = TransitionType.None;
                TodoActions[i] = delegate { };
            }
        }

        public int Count;
        public TransitionType[] transitionType;
        public (string ostName, float volume)?[] OstData;
        public IEnumerator[] IEnumerators;
        public AsyncOperationHandle<AudioClip>?[] Handlers;
        public Action[] TodoActions;
    }

    private Dictionary<AudioLine, AudioData> _data = new();

    private Dictionary<AudioLine, List<AudioSource>> _sources = new();

    void Awake()
    {
        AudioListener.volume = 1f;
        instance = this;

        // Инициализация источников
        _sources.Add(AudioLine.Music, new List<AudioSource>());
        foreach (var component in MusicSourcesObject.GetComponents<AudioSource>())
        {
            _sources[AudioLine.Music].Add(component);
            MusicSourceCount++;
        }

        _sources.Add(AudioLine.Ambient, new List<AudioSource>());
        foreach (var component in AmbientSourcesObject.GetComponents<AudioSource>())
        {
            _sources[AudioLine.Ambient].Add(component);
            AmbientSourceCount++;
        }

        _sources.Add(AudioLine.Sound, new List<AudioSource>());
        foreach (var component in SoundSourcesObject.GetComponents<AudioSource>())
        {
            _sources[AudioLine.Sound].Add(component);
            SoundSourceCount++;
        }

        _data.Add(AudioLine.Music, new(MusicSourceCount));
        _data.Add(AudioLine.Ambient, new(AmbientSourceCount));
    }

    #region onskip
    public void ResetManager()
    {

        foreach (var item in _sources)
        {
            //item.Value.volume = 0;
            // item.Value.Stop();
        }
    }

    // Music
    // Метод для отгрузки при загрузкит сейв файла
    public IEnumerator FadeOutCurrent()
    {
        yield break;
    }

    public void ClearCurrent()
    {
    }

    public void OnSkipStart()
    {

    }

    public void OnSkipEnd()
    {

    }

    #endregion onskip

    // Функционал

    private int GetAvaliableAudioSourceId(AudioLine line)
    {
        int not_found = -1;

        for (int i = 0; i < _sources[line].Count; i++)
        {
            if (!_sources[line][i].isPlaying &&
                !(_data[line].transitionType[i] == TransitionType.Start) &&
                !(_data[line].transitionType[i] == TransitionType.Play))
            {
                return i;
            }
        }

        return not_found;
    }

    private List<int> GetSourcesByState(AudioLine line, TransitionType transitionType)
    {
        var data = _data[line];
        List<int> found = new List<int>();

        for (int i = 0; i < _sources[line].Count; i++)
        {
            if (data.transitionType[i] == transitionType)
            {
                found.Add(i);
            }
        }

        return found;
    }

    private int GetSourceIdByName(AudioLine line, string name)
    {
        int not_found = -1;

        for (int i = 0; i < _data[line].Count; i++)
        {
            var ost_data = _data[line].OstData[i];
            if (ost_data != null)
            {
                (string ostName, float volume) curr = ((string, float))ost_data;
                if (curr.ostName == name)
                {
                    return i;
                }
            }
        }

        return not_found;
    }

    private void CompleteAfterTask(AudioLine line, int index)
    {
        _data[line].TodoActions[index].Invoke();
        ClearTask(line, index);
    }

    private void AddAfterTask(AudioLine line, int index, Action action)
    {
        _data[line].TodoActions[index] = action;
    }

    private void ClearTask(AudioLine line, int index)
    {
        _data[line].TodoActions[index] = delegate { };
    }

    public IEnumerator AudioLineStart(AudioLine line, string name, float time, float targetVolume)
    {
        int freeSourceId = GetAvaliableAudioSourceId(line);
        if (freeSourceId == -1)
        {
            //UnloadHandlers();
            var fade_out_sources = GetSourcesByState(line, TransitionType.End);
            if (fade_out_sources.Count > 0)
            {
                freeSourceId = fade_out_sources[0];
                IEnumerator free_ienum = _data[line].IEnumerators[freeSourceId];
                if (free_ienum != null)
                {
                    StopCoroutine(free_ienum);
                }
                CompleteAfterTask(line, freeSourceId);
            }
            else
            {
                Debug.Log("Нет свободных источников");
                yield break;
            }
        }

        AudioSource targetSource = _sources[line][freeSourceId];

        _data[line].OstData[freeSourceId] = (name, targetVolume);
        _data[line].transitionType[freeSourceId] = TransitionType.Start;

        targetSource.Stop();
        targetSource.volume = 0;

        var currentOperationHandle = Addressables.LoadAssetAsync<AudioClip>(name);
        yield return currentOperationHandle;
        var newAudioClip = currentOperationHandle.Result;
        _data[line].Handlers[freeSourceId] = currentOperationHandle;

        targetSource.clip = newAudioClip;
        targetSource.Play();

        IEnumerator fadein = LinearFadeTime(targetSource, time, targetVolume);
        _data[line].IEnumerators[freeSourceId] = fadein;
        AddAfterTask(line, freeSourceId, delegate { _data[line].transitionType[freeSourceId] = TransitionType.Play; });

        StartCoroutine(ProcessAudioLineStart(fadein, line, freeSourceId));
    }

    private IEnumerator ProcessAudioLineStart(IEnumerator fadein, AudioLine line, int id)
    {
        yield return StartCoroutine(fadein);
        CompleteAfterTask(line, id);
    }

    public IEnumerator AudioLineEnd(AudioLine line, string name, float time)
    {
        int sourceId = GetSourceIdByName(line, name);
        if (sourceId == -1 /*|| !Music.ContainsKey(name)*/)
        {
            yield break;
        }

        _data[line].transitionType[sourceId] = TransitionType.End;
        _data[line].OstData[sourceId] = null;

        CompleteAfterTask(line, sourceId);
        IEnumerator curr = _data[line].IEnumerators[sourceId];
        if (curr != null)
        {
            StopCoroutine(curr);
        }

        IEnumerator fadeOut = LinearFadeTime(_sources[line][sourceId], time, 0);
        _data[line].IEnumerators[sourceId] = fadeOut;

        //AddHandlerToReleaseList(line, _data[line].Handlers[sourceId]);

        AddAfterTask(line, sourceId, delegate
        {
            _data[line].transitionType[sourceId] = TransitionType.None;
            ReleaseHandler(_data[line].Handlers[sourceId]);
            var source = _sources[line][sourceId];
            source.clip = null;
            source.Stop();
        });

        StartCoroutine(ProcessAudioLineEnd(fadeOut, sourceId, line));
    }

    private IEnumerator ProcessAudioLineEnd(IEnumerator fadeout, int id, AudioLine line)
    {
        yield return StartCoroutine(fadeout);
        CompleteAfterTask(line, id);
    }

    // Переход трека в трек
    public IEnumerator AudioLineTransition(AudioLine line, string prev_name, string new_name, float time, float targetVolume)
    {
        int freeSourceId = GetAvaliableAudioSourceId(line);
        if (freeSourceId == -1)
        {
            var fade_out_sources = GetSourcesByState(line, TransitionType.End);
            if (fade_out_sources.Count > 0)
            {
                freeSourceId = fade_out_sources[0];
                IEnumerator free_ienum = _data[line].IEnumerators[freeSourceId];
                if (free_ienum != null)
                {
                    StopCoroutine(free_ienum);
                }
                CompleteAfterTask(line, freeSourceId);
            }
            else
            {
                yield break;
            }
        }

        int currentSourceId = GetSourceIdByName(line, prev_name);
        if (currentSourceId == -1)
        {
            yield break;
        }
        IEnumerator old_enum = _data[line].IEnumerators[currentSourceId];
        if (old_enum != null)
        {
            StopCoroutine(old_enum);
        }

        AudioSource targetSource = _sources[line][freeSourceId];
        AudioSource currentSource = _sources[line][currentSourceId];

        _data[line].OstData[currentSourceId] = null;
        _data[line].transitionType[currentSourceId] = TransitionType.End;

        _data[line].OstData[freeSourceId] = (new_name, targetVolume);
        _data[line].transitionType[freeSourceId] = TransitionType.Start;

        targetSource.Stop();
        targetSource.volume = 0;

        var currentOperationHandle = Addressables.LoadAssetAsync<AudioClip>(new_name);
        yield return currentOperationHandle;
        var newAudioClip = currentOperationHandle.Result;
        _data[line].Handlers[freeSourceId] = currentOperationHandle;

        targetSource.clip = newAudioClip;
        targetSource.Play();

        IEnumerator fadein = LinearFadeTime(targetSource, time, targetVolume);
        _data[line].IEnumerators[freeSourceId] = fadein;

        IEnumerator fadeout = LinearFadeTime(currentSource, time, 0);
        _data[line].IEnumerators[currentSourceId] = fadeout;

        AddAfterTask(line, freeSourceId, delegate
        {
            _data[line].transitionType[freeSourceId] = TransitionType.Play;
            _data[line].transitionType[currentSourceId] = TransitionType.None;
            ReleaseHandler(_data[line].Handlers[currentSourceId]);
            var source = _sources[line][currentSourceId];
            source.clip = null;
            source.Stop();
        });

        StartCoroutine(ProcessLineTransition(fadein, fadeout, line, freeSourceId));
    }

    private IEnumerator ProcessLineTransition(IEnumerator fadein, IEnumerator fadeout, AudioLine line, int new_id)
    {
        yield return StartCoroutine(WaitForAll(new List<IEnumerator>() { fadein, fadeout }));
        CompleteAfterTask(line, new_id);
    }

    // Трек -> тишина -> трек
    public IEnumerator AudioLineChange(AudioLine line, string old_name, string new_name, float fade_time, float delay_time, float targetVolume)
    {
        int currentSourceId = GetSourceIdByName(line, old_name);
        if (currentSourceId == -1)
        {
            yield break;
        }
        IEnumerator old_enum = _data[line].IEnumerators[currentSourceId];
        if (old_enum != null)
        {
            StopCoroutine(old_enum);
        }

        int freeSourceId = GetAvaliableAudioSourceId(line);
        if (freeSourceId == -1)
        {
            var fade_out_sources = GetSourcesByState(line, TransitionType.End);
            if (fade_out_sources.Count > 0)
            {
                freeSourceId = fade_out_sources[0];
                IEnumerator free_ienum = _data[line].IEnumerators[freeSourceId];
                if (free_ienum != null)
                {
                    StopCoroutine(free_ienum);
                }
                CompleteAfterTask(line, freeSourceId);
            }
            else
            {
                yield break;
            }
        }

        AudioSource targetSource = _sources[line][freeSourceId];
        AudioSource currentSource = _sources[line][currentSourceId];

        _data[line].OstData[currentSourceId] = null;
        _data[line].transitionType[currentSourceId] = TransitionType.End;

        _data[line].OstData[freeSourceId] = (new_name, targetVolume);
        _data[line].transitionType[freeSourceId] = TransitionType.Start;

        targetSource.Stop();
        targetSource.volume = 0;

        var currentOperationHandle = Addressables.LoadAssetAsync<AudioClip>(new_name);
        yield return currentOperationHandle;
        var newAudioClip = currentOperationHandle.Result;
        _data[line].Handlers[freeSourceId] = currentOperationHandle;

        targetSource.clip = newAudioClip;

        IEnumerator fadeout = LinearFadeTime(currentSource, fade_time, 0);
        _data[line].IEnumerators[currentSourceId] = fadeout;

        IEnumerator fadein = LinearFadeTime(targetSource, fade_time, targetVolume);
        _data[line].IEnumerators[freeSourceId] = fadein;

        AddAfterTask(line, freeSourceId, delegate
        {
            _data[line].transitionType[freeSourceId] = TransitionType.Play;
            _data[line].transitionType[currentSourceId] = TransitionType.None;
            ReleaseHandler(_data[line].Handlers[currentSourceId]);
            var source = _sources[line][currentSourceId];
            source.clip = null;
            source.Stop();
            var targetSource = _sources[line][freeSourceId];
            targetSource.Play();
        });

        StartCoroutine(ProcessLineChange(line, fadein, fadeout, currentSourceId, freeSourceId, delay_time, targetSource));
    }

    private IEnumerator ProcessLineChange(AudioLine line, IEnumerator fadein, IEnumerator fadeout, int old_id, int new_id, float delay_time, AudioSource targetSource)
    {
        yield return StartCoroutine(fadeout);

        yield return StartCoroutine(IDelay(delay_time));

        CompleteAfterTask(line, new_id);

        StartCoroutine(fadein);
    }

    private void ReleaseHandler(AsyncOperationHandle<AudioClip>? handler)
    {
        if (handler != null)
        {
            var h = (AsyncOperationHandle<AudioClip>)handler;
            if (h.IsValid())
            {
                Addressables.ReleaseInstance(h);
            }
        }
    }

    public IEnumerator AudioLineVolChange(AudioLine line, string name, float time, float volume)
    {
        int sourceId = GetSourceIdByName(line, name);
        if (sourceId == -1)
        {
            yield break;
        }

        _data[line].transitionType[sourceId] = TransitionType.VolChange;
        _data[line].OstData[sourceId] = (name, volume);

        CompleteAfterTask(line, sourceId);
        IEnumerator curr = _data[line].IEnumerators[sourceId];
        if (curr != null)
        {
            StopCoroutine(curr);
        }

        IEnumerator volFade = LinearFadeTime(_sources[line][sourceId], time, volume);
        _data[line].IEnumerators[sourceId] = volFade;

        AddAfterTask(line, sourceId, delegate
        {
            _data[line].transitionType[sourceId] = TransitionType.Play;
        });

        StartCoroutine(ProcessVolChange(line, volFade, sourceId));
    }

    private IEnumerator ProcessVolChange(AudioLine line, IEnumerator volFade, int id)
    {
        yield return StartCoroutine(volFade);
        CompleteAfterTask(line, id);
    }

    public IEnumerator SoundStart(string name)
    {
        yield break;
    }

    private IEnumerator IDelay(float time)
    {
        float currentTime = 0;
        while (currentTime < time)
        {
            float step;
            if (Typewriter.Instance.SkipIsActive)
            {
                step = SkipStep;
            }
            else
            {
                step = DefaultStep;
            }

            currentTime += Time.deltaTime * step;
            yield return null;
        }

        yield break;
    }

    private IEnumerator WaitForAll(List<IEnumerator> coroutines)
    {
        int tally = 0;

        foreach (IEnumerator c in coroutines)
        {
            StartCoroutine(RunCoroutine(c));
        }

        while (tally > 0)
        {
            yield return null;
        }

        IEnumerator RunCoroutine(IEnumerator c)
        {
            tally++;
            yield return StartCoroutine(c);
            tally--;
        }
    }

    private const float SkipStep = 10f;
    private const float DefaultStep = 1;
    public IEnumerator LinearFadeTime(AudioSource source, float fadeTime, float targetVolume_linear)
    {
        float currentTime = 0;
        float currentVolume_linear = source.volume;
        while (currentTime < fadeTime)
        {
            float step;
            if (Typewriter.Instance.SkipIsActive)
            {
                step = SkipStep;
            }
            else
            {
                step = DefaultStep;
            }

            currentTime += Time.deltaTime * step;
            source.volume = Mathf.Lerp(currentVolume_linear, targetVolume_linear, (currentTime / fadeTime));
            yield return null;
        }

        yield break;
    }
}
