using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
    [SerializeField] private GameObject[] objects;

    // Start is called before the first frame update
    public void Activation() {
        foreach(GameObject o in objects) {
            if (o != null) o.SetActive(true);
        }
    }
}
