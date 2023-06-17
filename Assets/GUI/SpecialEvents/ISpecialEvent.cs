using System.Collections;

public interface ISpecialEvent
{
    int GetState();

    IEnumerator ILoadEventByState(int state);

    IEnumerator IReleaseEvent();
}
