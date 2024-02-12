using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M02_t2 : MissionEventNode 
    {
    // Start is called before the first frame update
    public override void EventFire() {
        StartCoroutine("Event");
    }
    private string[] messageText = new string[]{
        "[�L�[�X]\n�|�������B",
        "[�I�X�J�[�卲]\n����J�B�h���[���̓������J�n����B",
        "[�I�X�J�[�卲]\n�~�b�V�����������B"
    };
    [SerializeField] MessageWindow message;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Event() {
        yield return new WaitForSeconds(1);
        message.function(messageText[0], 2.7f);
        yield return new WaitForSeconds(3);
        message.function(messageText[1], 4.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[2], 3.7f);
        yield return new WaitForSeconds(3);
        message.function("\nMission Complete!", 3);
        yield return new WaitForSeconds(2);
        Initiate.Fade("menu_02", Color.black, 1.0f);
        yield break;

    }
}
