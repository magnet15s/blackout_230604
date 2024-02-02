using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : MonoBehaviour {
    public static MessageWindow instance;

    Coroutine coroutine;
    bool isRunning = false;

    GameObject textWindow;
    TextMeshProUGUI text;
    public AudioClip sound1;
    AudioSource audioSource;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    // 初期化
    void Start() {
        textWindow = this.gameObject;
        text = textWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        DisableTextWindow();
        audioSource = GetComponent<AudioSource>();

    }

    // このメソッドを呼び出してメッセージウィンドウを更新する
    public void function    (string str, float sec) {
        if (isRunning) {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(UpdateMessageWindow(str, sec));
    }

    // メッセージウィンドウの更新をコルーチンで実装
    private IEnumerator UpdateMessageWindow(string str, float sec) {
        isRunning = true;
        text.text = str;
        audioSource.PlayOneShot(sound1);

        EnableTextWindow();

        yield return new WaitForSeconds(sec);

        DisableTextWindow();
        isRunning = false;
    }

    // メッセージウィンドウを表示する
    private void EnableTextWindow() {
        Image img = textWindow.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
    }

    // メッセージウィンドウを非表示にする
    private void DisableTextWindow() {
        Image img = textWindow.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }
}
