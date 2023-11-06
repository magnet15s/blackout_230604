using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mission_select : MonoBehaviour
{
    public static int mission_number=0;
    // Start is called before the first frame update
    public void OnButtonClick01() {
        mission_number = 1;
        Initiate.Fade("menu_04", Color.black, 3f);
    }
    public void OnButtonClick02() {
        mission_number = 2;
        Initiate.Fade("menu_04", Color.black, 3f);
    }
    public void OnButtonClick03() {
        mission_number = 3;
        Initiate.Fade("menu_04", Color.black, 3f);
    }
    public void OnButtonClick04() {
        mission_number = 4;
        Initiate.Fade("menu_04", Color.black, 3f);
    }
    public void OnButtonClick05() {
        mission_number = 5;
        Initiate.Fade("menu_04", Color.black, 3f);
    }
    public void OnButtonClickRe() {
        Initiate.Fade("menu_02", Color.black, 3f);
    }

    public void OnButtonClickInmenu() {
        Initiate.Fade("menu_03", Color.black, 3f);
    }
}
