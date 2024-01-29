using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PauseMusicDisplay : MonoBehaviour
{
    private Text _text;

    private void Awake()
    {
        _text = GetComponent<Text>();
    }

    public void UpdateDisplayMusic()
    {
        StringBuilder stringBuilder = new StringBuilder();

        var sourceData = AudioManager.instance.GetActiveSources();
        if (sourceData.Count > 0)
        {  
            stringBuilder.Append($"Сейчас играет:\n");
            foreach (var source in sourceData)
            {
                if (source.audioLine == AudioManager.AudioLine.Music)
                {
                    string music_code = source.ostdata.OstName;
                    string displayable_name = AudioOstNames.GetDisplayableName(music_code);
                    stringBuilder.Append($"{displayable_name}\n");
                }
            } 
        }

        _text.text = stringBuilder.ToString();
    }
}
