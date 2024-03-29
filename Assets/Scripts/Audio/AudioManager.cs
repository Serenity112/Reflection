using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

    public struct SingleOstData
    {
        public SingleOstData(string name, float vol)
        {
            OstName = name;
            Volume = vol;
        }

        public string OstName;
        public float Volume;
    }

    public class AudioDataSaveFile
    {
        public AudioDataSaveFile()
        {
            saveData = new List<(AudioLine audioLine, SingleOstData ostdata)>();
        }

        public List<(AudioLine audioLine, SingleOstData ostdata)> saveData;
    }

    public struct AudioData
    {
        public AudioData(int count)
        {
            Count = count;

            transitionType = new TransitionType[count];
            OstData = new SingleOstData[count];
            IEnumerators = new IEnumerator[count];
            Handlers = new AsyncOperationHandle<AudioClip>?[count];
            TodoActions = new Action[count];

            for (int i = 0; i < count; i++)
            {
                OstData[i] = new SingleOstData(null, 0);
                IEnumerators[i] = null;
                Handlers[i] = null;
                transitionType[i] = TransitionType.None;
                TodoActions[i] = delegate { };
            }
        }

        public void SetOstData(SingleOstData[] newData)
        {
            OstData = newData;
        }

        public int Count;
        public TransitionType[] transitionType;
        public SingleOstData[] OstData;
        public IEnumerator[] IEnumerators;
        public AsyncOperationHandle<AudioClip>?[] Handlers;
        public Action[] TodoActions;
    }

    private Dictionary<AudioLine, AudioData> _data = new();

    private Dictionary<AudioLine, List<AudioSource>> _sources = new();

    private void Awake()
    {
        AudioListener.volume = 1f;
        instance = this;

        // ������������� ����������
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

        InitAudioData();
    }

    private void InitAudioData()
    {
        _data.Clear();
        _data.Add(AudioLine.Music, new(MusicSourceCount));
        _data.Add(AudioLine.Ambient, new(AmbientSourceCount));
        _data.Add(AudioLine.Sound, new(SoundSourceCount));
    }

    // ��� ���� �������
    public void UnloadAllData()
    {
        for (int i = 0; i < MusicSourceCount; i++)
        {
            StopLineCoroutine(_data[AudioLine.Music].IEnumerators[i]);
            CompleteAfterTask(AudioLine.Music, i);
        }

        for (int i = 0; i < AmbientSourceCount; i++)
        {
            StopLineCoroutine(_data[AudioLine.Ambient].IEnumerators[i]);
            CompleteAfterTask(AudioLine.Ambient, i);
        }

        for (int i = 0; i < SoundSourceCount; i++)
        {
            StopLineCoroutine(_data[AudioLine.Sound].IEnumerators[i]);
            CompleteAfterTask(AudioLine.Sound, i);
        }
    }

    public List<(AudioLine audioLine, SingleOstData ostdata)> GetActiveSources()
    {
        List<(AudioLine audioLine, SingleOstData ostdata)> list = new();

        for (int j = 0; j < _sources[AudioLine.Music].Count; j++)
        {
            SingleOstData n_ost = _data[AudioLine.Music].OstData[j];
            if (n_ost.OstName != null)
            {
                list.Add((AudioLine.Music, n_ost));
            }
        }

        for (int j = 0; j < _sources[AudioLine.Ambient].Count; j++)
        {
            SingleOstData n_ost = _data[AudioLine.Ambient].OstData[j];
            if (n_ost.OstName != null)
            {
                list.Add((AudioLine.Ambient, n_ost));
            }
        }

        return list;
    }

    public AudioDataSaveFile GetSaveData()
    {
        AudioDataSaveFile data = new();
        data.saveData = GetActiveSources();
        return data;
    }

    public IEnumerator FadeOutCurrent()
    {
        //StopAllCoroutines();

        List<IEnumerator> list = new List<IEnumerator>();

        var sources = GetActiveSources();
        foreach (var source in sources)
        {
            IEnumerator fadeout = AudioLineEnd(source.audioLine, source.ostdata.OstName, 0.25f, true);
            list.Add(fadeout);
        }

        yield return StartCoroutine(WaitForAll(list));

        UnloadAllData();
    }

    public IEnumerator FadeInCurrent(AudioDataSaveFile data)
    {
        InitAudioData();

        List<IEnumerator> list = new List<IEnumerator>();
        foreach (var savedata in data.saveData)
        {
            AudioLine line = savedata.audioLine;
            SingleOstData ostdata = savedata.ostdata;

            IEnumerator fadein = AudioLineStart(line, ostdata.OstName, 0.25f, ostdata.Volume);
            list.Add(fadein);
        }

        yield return StartCoroutine(WaitForAll(list));
    }

    // ����������

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
            if (ost_data.OstName != null)
            {
                if (ost_data.OstName == name)
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
        _data[line].TodoActions[index] = delegate { };
    }

    private void AddAfterTask(AudioLine line, int index, Action action)
    {
        _data[line].TodoActions[index] = action;
    }

    private void StopLineCoroutine(IEnumerator enumerator)
    {
        if (enumerator != null)
        {
            StopCoroutine(enumerator);
        }
    }

    // ����� ����
    public IEnumerator AudioLineStart(AudioLine line, string name, float time, float targetVolume, float start_delay = 0f)
    {
        if (!AudioOstNames.OstNames[line].ContainsKey(name))
        {
            yield break;
        }

        int freeSourceId = GetAvaliableAudioSourceId(line);
        if (freeSourceId == -1)
        {
            var fade_out_sources = GetSourcesByState(line, TransitionType.End);
            if (fade_out_sources.Count > 0)
            {
                freeSourceId = fade_out_sources[0];
                IEnumerator free_ienum = _data[line].IEnumerators[freeSourceId];
                StopLineCoroutine(free_ienum);
                CompleteAfterTask(line, freeSourceId);
            }
            else
            {
                Debug.Log("��� ��������� ����������");
                yield break;
            }
        }

        AudioSource targetSource = _sources[line][freeSourceId];

        _data[line].OstData[freeSourceId] = new SingleOstData(name, targetVolume);
        _data[line].transitionType[freeSourceId] = TransitionType.Start;

        targetSource.Stop();
        targetSource.volume = 0;

        var currentOperationHandle = Addressables.LoadAssetAsync<AudioClip>(name);
        yield return currentOperationHandle;
        var newAudioClip = currentOperationHandle.Result;
        _data[line].Handlers[freeSourceId] = currentOperationHandle;

        targetSource.clip = newAudioClip;
        targetSource.Play();

        IEnumerator fadein = LinearFadeTime(targetSource, time, targetVolume, start_delay);
        _data[line].IEnumerators[freeSourceId] = fadein;
        AddAfterTask(line, freeSourceId, delegate
        {
            _data[line].transitionType[freeSourceId] = TransitionType.Play;
        });

        StartCoroutine(ProcessAudioLineStart(fadein, line, freeSourceId));
    }

    private IEnumerator ProcessAudioLineStart(IEnumerator fadein, AudioLine line, int id)
    {
        yield return StartCoroutine(fadein);
        CompleteAfterTask(line, id);
    }

    // ��������� ����
    public IEnumerator AudioLineEnd(AudioLine line, string name, float time, bool full_wait = false)
    {
        if (!AudioOstNames.OstNames[line].ContainsKey(name))
        {
            yield break;
        }

        int sourceId = GetSourceIdByName(line, name);
        if (sourceId == -1)
        {
            yield break;
        }

        IEnumerator curr = _data[line].IEnumerators[sourceId];
        StopLineCoroutine(curr);
        CompleteAfterTask(line, sourceId);

        _data[line].transitionType[sourceId] = TransitionType.End;
        _data[line].OstData[sourceId] = new SingleOstData(null, 0);

        AudioSource source = _sources[line][sourceId];
        IEnumerator fadeOut = LinearFadeTime(source, time, 0);
        _data[line].IEnumerators[sourceId] = fadeOut;

        var old_handler = _data[line].Handlers[sourceId];
        AddAfterTask(line, sourceId, delegate
        {
            _data[line].transitionType[sourceId] = TransitionType.None;
            ReleaseHandler(old_handler);
            source.Stop();
            source.clip = null;
        });

        if (full_wait)
        {
            yield return StartCoroutine(ProcessAudioLineEnd(fadeOut, sourceId, line));
        }
        else
        {
            StartCoroutine(ProcessAudioLineEnd(fadeOut, sourceId, line));
        }
    }

    private IEnumerator ProcessAudioLineEnd(IEnumerator fadeout, int id, AudioLine line)
    {
        yield return StartCoroutine(fadeout);
        CompleteAfterTask(line, id);
    }

    // ��������� �����
    public IEnumerator AudioLineTransition(AudioLine line, string prev_name, string new_name, float time, float targetVolume)
    {
        if (!AudioOstNames.OstNames[line].ContainsKey(new_name) || !AudioOstNames.OstNames[line].ContainsKey(prev_name))
        {
            yield break;
        }

        yield return StartCoroutine(WaitForAll(new List<IEnumerator>()
        {
            AudioLineStart(line, new_name, time, targetVolume),
            AudioLineEnd(line, prev_name, time)
        }));
    }

    // ��������� ����� � ���������
    public IEnumerator AudioLineChange(AudioLine line, string prev_name, string new_name, float fade_time, float delay_time, float targetVolume)
    {
        if (!AudioOstNames.OstNames[line].ContainsKey(new_name) || !AudioOstNames.OstNames[line].ContainsKey(prev_name))
        {
            yield break;
        }

        yield return StartCoroutine(WaitForAll(new List<IEnumerator>()
        {
            AudioLineStart(line, new_name, fade_time, targetVolume, fade_time + delay_time),
            AudioLineEnd(line, prev_name, fade_time)
        }));
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

    // ��������� ��������� �����
    public IEnumerator AudioLineVolChange(AudioLine line, string name, float time, float volume)
    {
        if (!AudioOstNames.OstNames[line].ContainsKey(name))
        {
            yield break;
        }

        int sourceId = GetSourceIdByName(line, name);
        if (sourceId == -1)
        {
            yield break;
        }

        _data[line].transitionType[sourceId] = TransitionType.VolChange;
        _data[line].OstData[sourceId] = new SingleOstData(name, volume);

        StopLineCoroutine(_data[line].IEnumerators[sourceId]);
        CompleteAfterTask(line, sourceId);

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

    // ��������� �����
    public IEnumerator SoundStart(string name, float targetVolume)
    {
        if (!AudioOstNames.OstNames[AudioLine.Sound].ContainsKey(name))
        {
            yield break;
        }

        int sourceId = 0;
        AudioLine line = AudioLine.Sound;

        CompleteAfterTask(line, sourceId);

        var currentOperationHandle = Addressables.LoadAssetAsync<AudioClip>(name);
        yield return currentOperationHandle;
        var newAudioClip = currentOperationHandle.Result;
        _data[line].Handlers[sourceId] = currentOperationHandle;

        AudioSource source = _sources[AudioLine.Sound][sourceId];

        source.Stop();
        source.volume = targetVolume;
        source.clip = newAudioClip;
        source.Play();

        var old_handler = _data[line].Handlers[sourceId];
        AddAfterTask(line, sourceId, delegate
        {
            ReleaseHandler(old_handler);
            source.Stop();
            source.clip = null;
        });

        StartCoroutine(ProcessSound(source, sourceId));
    }

    private IEnumerator ProcessSound(AudioSource source, int id)
    {
        while (true)
        {
            if (!source.isPlaying)
            {
                break;
            }
            yield return null;
        }

        CompleteAfterTask(AudioLine.Sound, id);
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
    private const float DefaultStep = 1f;
    public IEnumerator LinearFadeTime(AudioSource source, float fadeTime, float targetVolume_linear, float start_delay = 0f)
    {
        if (start_delay != 0f)
        {
            yield return new WaitForSeconds(start_delay);
        }

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
