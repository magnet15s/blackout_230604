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

    // ������
    void Start() {
        textWindow = this.gameObject;
        text = textWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        DisableTextWindow();
        audioSource = GetComponent<AudioSource>();

    }

    // ���̃��\�b�h���Ăяo���ă��b�Z�[�W�E�B���h�E���X�V����
    public void function    (string str, float sec) {
        if (isRunning) {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(UpdateMessageWindow(str, sec));
    }

    // ���b�Z�[�W�E�B���h�E�̍X�V���R���[�`���Ŏ���
    private IEnumerator UpdateMessageWindow(string str, float sec) {
        isRunning = true;
        text.text = str;
        audioSource.PlayOneShot(sound1);

        EnableTextWindow();

        yield return new WaitForSeconds(sec);

        DisableTextWindow();
        isRunning = false;
    }

    // ���b�Z�[�W�E�B���h�E��\������
    private void EnableTextWindow() {
        Image img = textWindow.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
    }

    // ���b�Z�[�W�E�B���h�E���\���ɂ���
    private void DisableTextWindow() {
        Image img = textWindow.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }
}
