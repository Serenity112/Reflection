using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPanelsManager
{
    GameObject GetBlackPanel();

    GameObject GetActivePanelsParent();

    void OpenSaveMenu();

    void CloseSaveMenu();

    public void ReleaseSaveMenu();

    Camera GetGameCamera();
}
