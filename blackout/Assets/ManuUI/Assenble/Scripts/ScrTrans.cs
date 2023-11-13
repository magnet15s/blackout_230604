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
            //�v���n�u�̎擾�y��MenuScreen�̎擾�ɐ��������ꍇ
            next = Instantiate(nextObj, MenuScreen.activeScreen.transform.parent).GetComponent<MenuScreen>();
            MenuScreen.OpenScreen(next);
        }
        else
        {
            if (nextObj != null) Debug.LogError($"{this} > MenuScreen�̎擾�Ɏ��s���܂��� : [{nextObj}]");
            else Debug.LogError($"{this} > �v���n�u�̎擾�Ɏ��s���܂���");
        }
        
    }
}
