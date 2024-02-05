using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInstantiater : MonoBehaviour
{
    public Transform parent;
    public GameObject prefab;

    public bool useThisTransformForInstantiateBase = true;

    public bool overridePosition = false;
    public bool overrideAngle = false;
    public bool overrideScale = false;

    public Vector3 prefabPosition = Vector3.zero;
    public Vector3 prefabEulerAngle = Vector3.zero;
    public Vector3 prefabScale = Vector3.one;

    // Start is called before the first frame update
    public void OnInstantiate()
    {
        Transform pr = Instantiate(prefab, useThisTransformForInstantiateBase ? transform : parent).transform;

        if(overridePosition)pr.localPosition = prefabPosition;
        if(overrideAngle)pr.localEulerAngles = prefabEulerAngle;
        if(overrideScale)pr.localScale = prefabScale;

        if (useThisTransformForInstantiateBase) pr.SetParent(parent);
    }
}
