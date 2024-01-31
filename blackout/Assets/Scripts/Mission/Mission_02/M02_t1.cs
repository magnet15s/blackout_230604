using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M02_t1 : MissionEventNode {
    // Start is called before the first frame update
    private string[] missionItems;
    private string[] messageText = new string[]{
        "[�L�[�X]\n�ڕW�n�_�t�߂ɓ����B�|�����J�n����"
    };
    [SerializeField] private string missionTitle = "����";
    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;
    public override void EventFire() {
        missionItems = new string[] {
            $"�S�Ă̓G�̌��j"
        };
        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        StartCoroutine("Event");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Event() {
        yield return new WaitForSeconds(1);
        mList.RemoveMissionItems();
        foreach (string s in missionItems) mList.AddMissionItem(s);
        message.function(messageText[0], 4.7f);
        ParmitNext();

    }
}
