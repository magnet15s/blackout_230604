using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialEFlag : MissionEventFlag
{
    // Start is called before the first frame update
    private void Update() {
        if(isActive)OnFlagUp();
    }
}
