using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] Camera mapCam;
    [SerializeField] Material mapMat;
    [SerializeField] Transform player;
    void Start()
    {
        setTex();
    }

    // Update is called once per frame
    void Update()
    {
        setTex();
        transform.position = player.transform.position + new Vector3(0, 200, 0);
    }

    void setTex()
    {
        mapMat.SetTexture("_MainTex", mapCam.targetTexture, 0);
    }
}
