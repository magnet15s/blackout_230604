using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneCurtain : MonoBehaviour
{
    private Image image;
    private Color c;
    [SerializeField] private float openTime;
    private float openCnt = 0;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        c = image.color;
        image.color = new Color(c.r, c.g, c.b, 1);
    }

    // Update is called once per frame
    void Update()
    {
        openCnt += Time.deltaTime;
        c.a = 1 - (openCnt / openTime);
        image.color = c;
    }
}
