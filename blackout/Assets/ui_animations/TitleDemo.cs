using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleDemo : MonoBehaviour
{
    [SerializeField] VideoPlayer vp;
    [SerializeField] List<VideoClip> demoList;
    [SerializeField] Image screen;
    [SerializeField] long videoFadeFrame = 30;
    [SerializeField] float titleFadeTime = 1;
    [SerializeField] float replayDuration = 15;
    [SerializeField] GameObject top;
    [SerializeField] GameObject bottom;  


    int videoIdx = -1;
    void Start()
    {
        StartCoroutine(Sleep());
    }

    // Update is called once per frame
    void Update()
    {
        if(vp.frame != -1 && vp.frame < (long)vp.frameCount - 1) {//ビデオがまだ終わっていない間
            Debug.LogWarning(vp.frame + " / " + vp.frameCount);
            if(vp.frame <= videoFadeFrame) {
                float f = (float)vp.frame / (float)videoFadeFrame;
                Debug.Log("vi " + f);
                screen.color = new Color(f, f, f, 1);
            }

            if(vp.frame >= (long)vp.frameCount-1 - videoFadeFrame) {
                float f = (float)((long)vp.frameCount - 1 - vp.frame) / (float)videoFadeFrame;
                Debug.Log("vo " + f);
                screen.color = new Color(f, f, f, 1);
            }
        } else  if(vp.frame >= (long)vp.frameCount - 1 && vp.isPlaying){
            StartCoroutine(Sleep());
        }
    }

    IEnumerator Sleep() {
        top.SetActive(false);
        bottom.SetActive(false);
        vp.Stop();
        vp.frame = (long)vp.frameCount - 1;
        for(float t = 0; t < titleFadeTime; t += 0.02f) {
            screen.color = new Color(0, 0, 0, 1 - (t / titleFadeTime));
            Debug.Log("ti " + (1 - (t / titleFadeTime)));
            yield return new WaitForSeconds(0.02f);
        }
        screen.color = new Color(0, 0, 0, 0);
        yield return new WaitForSeconds(replayDuration - (titleFadeTime * 2));
        for (float t = 0; t < titleFadeTime; t += 0.02f) {
            screen.color = new Color(0, 0, 0, t / titleFadeTime);
            Debug.Log("to " + (t / titleFadeTime));
            yield return new WaitForSeconds(0.02f);
        }
        screen.color = new Color(0, 0, 0, 1);
        videoIdx++;
        if (videoIdx >= demoList.Count) videoIdx = 0;
        vp.clip = demoList[videoIdx];
        vp.frame = 0;
        vp.Play();

        top.SetActive(true);
        bottom.SetActive(true);
        yield return null;
    }
}
