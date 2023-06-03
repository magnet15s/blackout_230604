using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class StatePrint : MonoBehaviour
{
    float timer = 0;

    public PlayerController pl;

    public TextMeshProUGUI tmp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 0.1)
        {
            timer = 0;
            tmp.text = pl.getActState();
        }
    }
}
