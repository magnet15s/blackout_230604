using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIBGM : MonoBehaviour
{
    // Start is called before the first frame update
    private static bool isLoad = false;// ���g�����łɃ��[�h����Ă��邩�𔻒肷��t���O
    private void Awake() {
        
        
        if (isLoad==true) { // ���łɃ��[�h����Ă�����
            Destroy(this.gameObject); // �������g��j�����ďI��
            return;
        }
        else {
            isLoad = true;
        }
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "menu_03") {
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        else if (SceneManager.GetActiveScene().name == "menu_04") {
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        else if (isLoad == true) {
            isLoad = false;
            Destroy(this.gameObject); // �������g��j�����ďI��
            
            return;
        }
    }
}
