using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneCurtain : MonoBehaviour
{
    private Image image;
    public Color c;
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
        if(openCnt <= openTime) {
            openCnt += Time.deltaTime;
            c.a = 1 - (openCnt / openTime);
            image.color = c;
        }
        
    }

    public void closeCurtain(float second) {
        csec = second;
        StartCoroutine("Cc");
    }
    float csec = 1;
    IEnumerator Cc() {
        for(float t = 0; t < csec; t += 0.02f) {

            image.color = new Color(c.r, c.g, c.b, (t * 1.2f) / csec);
            yield return new WaitForSeconds(0.02f);
        }

    }

}
