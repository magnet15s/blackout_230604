using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public static MenuScreen activeScreen { get; private set; } = null;
    public static Stack<MenuScreen> ScreenStack = new();

    /// <summary>
    /// 新しいメニュスクリーンを開く
    /// 現在のactiveScreenを非活化し、次のメニュースクリーンをactiveScreenに設定する
    /// </summary>
    /// <param name="nextScreen">次のスクリーン</param>
    public static void OpenScreen(MenuScreen nextScreen)
    {
        if (nextScreen.Equals(activeScreen))
        {
            Debug.LogWarning($"MenuScreen.SetScreen() > すでに開いているスクリーンです{nextScreen}");
            return;
        }

        ScreenStack.Push(nextScreen);
        if(activeScreen != null)
        {
            activeScreen.gameObject.SetActive(false);
        }
        if(activeScreen != null)
        {
            nextScreen.transform.SetParent(activeScreen.transform.parent);
            nextScreen.parentScreen = activeScreen;
        }
        
        activeScreen = nextScreen;
    }

    public static void CloseScreen()
    {
        Destroy(ScreenStack.Pop().gameObject);
        activeScreen = ScreenStack.Peek();
        activeScreen.gameObject.SetActive(true);
        
    }

    public MenuScreen parentScreen;

    void Start()
    {
        if(activeScreen == null)
        {
            OpenScreen(this);
            Debug.Log("initial screen is " + this);
        }
    }

    
}
