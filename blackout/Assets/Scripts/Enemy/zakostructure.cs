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
        //MessageWindow.instance.function("�G\n�I���̑q�ɂ����`�I",1f);

        Destroy(this.gameObject);
        GameObject effect = Instantiate(breakEffect) as GameObject;
        //�G�t�F�N�g����������ꏊ�����肷��(�G�I�u�W�F�N�g�̏ꏊ)
        effect.transform.position = gameObject.transform.position;
    }
}
