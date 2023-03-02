using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public struct SentenceData
{
    public SentenceData(int saveNum)
    {
        sentenceNum = saveNum;
        phrase = string.Empty;
        speaker = string.Empty;
        extend = false;
    }

    public int sentenceNum;
    public string phrase;
    public string speaker;
    public bool extend;
}

public class DaysManager : MonoBehaviour
{
    public static DaysManager instance = null;

    public Dictionary<int, SentenceData> daySentences;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
    }
    public IEnumerator ILoadDay(int day)
    {
        daySentences = new Dictionary<int, SentenceData>();

        XmlDocument xDoc = new XmlDocument();

        switch (day)
        {
            case 1:
                xDoc.Load(Application.dataPath + "/StreamingAssets/e1d1.xml");
                break;
            case 2:
                xDoc.Load(Application.dataPath + "/StreamingAssets/day2.xml");
                break;
            case 3:
                xDoc.Load(Application.dataPath + "/StreamingAssets/day3.xml");
                break;
            default:
                xDoc.Load(Application.dataPath + "/StreamingAssets/e1d1.xml");
                break;
        }


        int i =0;

        foreach (XmlNode key in xDoc["Keys"].ChildNodes)
        {
            var sentenceData = new SentenceData(i);

            if (i > 100) break;


            string Phrase = key.Attributes["Phrase"].Value;
            sentenceData.phrase = Phrase;

            if(key["Speaker"] != null)
            {
                sentenceData.speaker = key["Speaker"].InnerText;
            }

            if (key["Extend"] != null)
            {
                sentenceData.extend = true;
            }

            //Debug.Log(i + " " + Phrase);

            daySentences[i] = sentenceData;

            i++;
            yield return null;
        }
    }

}
