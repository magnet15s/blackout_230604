using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mission_driver : MonoBehaviour
{
    private GameObject[] enemyBox;
    private int enemyDef;
    // Start is called before the first frame update
    void Start()
    {
        enemyDef = GameObject.FindGameObjectsWithTag("structure").Length;
    }

    // Update is called once per frame
    void Update()
    {
        enemyBox = GameObject.FindGameObjectsWithTag("structure");
        int enemycount = enemyDef - enemyBox.Length;
        print("åöë¢ï®îjâÛÅF" + enemycount+"/"+enemyDef);
    }
}
