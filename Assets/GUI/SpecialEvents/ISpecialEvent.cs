using System.Collections;

public interface ISpecialEvent
{
    string GetData();

    IEnumerator ILoadEventByData(string data);

    IEnumerator IReleaseEvent();
}
