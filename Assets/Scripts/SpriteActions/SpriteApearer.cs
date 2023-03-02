using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteApearer : MonoBehaviour
{
    public static SpriteApearer instance = null;
    private void Start()
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
    public IEnumerator SpriteAppear(string characterName, int Pose, int Emotion, Vector3 Position, float speed, bool skip)
    {
        int newSpriteNum = SpriteController.instance.GetNewActivity(characterName);

        SpriteController.instance.SaveSpriteData(newSpriteNum, characterName, Pose, Emotion, Position, 1f, false);

        GameObject Current = SpriteController.instance.GetSprite(newSpriteNum);
        GameObject Face1 = Current.transform.GetChild(0).gameObject;
        GameObject Face2 = Current.transform.GetChild(1).gameObject;

        Current.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        Face1.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        Face2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);

        SpriteController.instance.SetDefaultScale(Current, characterName);

        Current.transform.localPosition = Position;

        yield return StartCoroutine(SpriteController.instance.LoadSpriteByParts(Current, characterName, Pose, Emotion));

        yield return StartCoroutine(SpriteFade.instance.IAppearFullSprite(Current, speed, skip));

        // Возможно нужно доабвить скип фейда и мува, но вроде и так нормально
    }
}
