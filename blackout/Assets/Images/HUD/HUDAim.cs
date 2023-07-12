using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDAim : MonoBehaviour
{
    [SerializeField] private Transform trackObj;
    [SerializeField] private RectTransform Canvas;
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 w2sPos = Camera.main.WorldToScreenPoint(trackObj.position);
        rectTransform.position = new Vector2(
            w2sPos.x,
            w2sPos.y);
    }
}
