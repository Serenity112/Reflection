using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPanelsManager
{
    GameObject GetBlackPanel();

    public IEnumerator ILoadGame(int actualSaveNum);

    void OpenSaveMenu();

    void CloseSaveMenu();

    Camera GetGameCamera();
}
