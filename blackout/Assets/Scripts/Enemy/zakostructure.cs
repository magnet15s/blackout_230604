using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zakostructure : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject breakEffect;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void thisDestory() {
        //MessageWindow.instance.function("敵\nオラの倉庫がぁ～！",1f);

        Destroy(this.gameObject);
        GameObject effect = Instantiate(breakEffect) as GameObject;
        //エフェクトが発生する場所を決定する(敵オブジェクトの場所)
        effect.transform.position = gameObject.transform.position;
    }
}
