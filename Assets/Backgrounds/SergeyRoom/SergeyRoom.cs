using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SergeyRoom : MonoBehaviour, ISpecialEvent
{
    public enum TrainNum
    {
        Far,
        Close,
    }

    [SerializeField]
    private GameObject Clock;

    private string currentData;

    private Animator animator;

    public List<SpriteAtlas> Atlases = new List<SpriteAtlas>();


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        //currentData = ((int)StationScrollState.Start).ToString();
        SpecialEventManager.instance.SetEvent(this, SpecialEvent.SergeyRoom);
    }

    public void SetTime(string time)
    {
        time = time.Replace(":", "");
        for (int i = 0; i < 4; i++)
        {
            Clock.transform.GetChild(i).GetComponent<Image>().sprite = Atlases[i].GetSprite(time[i].ToString());
        }
    }

    // 0 - Close, 1 - Far
    public IEnumerator LaunchTrain(TrainNum train, float delay)
    {
        yield return new WaitForSeconds(delay);

        switch (train)
        {
            case TrainNum.Close:
                animator.Play("TrainCloseAnim");
                break;
            case TrainNum.Far:
                animator.Play("TrainFarAnim");
                break;
        }
    }

    public string GetData()
    {
        return currentData;
    }

    public IEnumerator ILoadEventByData(string data)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator IReleaseEvent()
    {
        yield return null;
    }
}
