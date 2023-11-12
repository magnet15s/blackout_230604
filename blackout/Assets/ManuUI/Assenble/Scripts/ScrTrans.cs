using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScrTrans : MonoBehaviour
{
    [SerializeField] protected string loadScreenPrefabName;
    
    public void LoadNextScreen()
    {
        MenuScreen next;
        GameObject nextObj;
        if((nextObj = (GameObject)Resources.Load(loadScreenPrefabName)).TryGetComponent<MenuScreen>(out next))
        {
            //プレハブの取得及びMenuScreenの取得に成功した場合
            next = Instantiate(nextObj, MenuScreen.activeScreen.transform.parent).GetComponent<MenuScreen>();
            MenuScreen.OpenScreen(next);
        }
        else
        {
            if (nextObj != null) Debug.LogError($"{this} > MenuScreenの取得に失敗しました : [{nextObj}]");
            else Debug.LogError($"{this} > プレハブの取得に失敗しました");
        }
        
    }
}
