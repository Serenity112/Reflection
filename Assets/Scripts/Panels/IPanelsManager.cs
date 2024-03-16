using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPanelsManager
{
    GameObject GetBlackPanel();

    void OpenSaveMenu();

    void CloseSaveMenu();

    Camera GetGameCamera();
}
