using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public static string TransitionArgs = null;

    [SerializeField] private string transitionSceneName;
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private float fadeTime = 3f;
    public void OnSceneTransition()
    {
        if (transitionSceneName == null)
        {
            Debug.Log($"{this} > 遷移先シーン名未設定");
            return;
        }
        try
        {
            Initiate.Fade(transitionSceneName, fadeColor, fadeTime);
        }
        catch (Exception e)
        {
            Debug.LogError($"シーン遷移に失敗しました\n{e}");
        }
    }
}
