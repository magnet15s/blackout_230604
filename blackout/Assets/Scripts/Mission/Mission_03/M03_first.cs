using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M03_first : MissionEventNode
{
    // Start is called before the first frame update
    private string[] missionItems;
    private string[] messageText = new string[]{
        "[�I�X�J�[����]\n�~�b�V�����J�n�B",
        "[�I�X�J�[����]\n����̍��͓G��n�U���̕z�΂��B\n�A�����̖��l�@�֊�P�������āA�Ō���^����B",
"[�I�X�J�[����]\n�G�����͂��̒n�̂ǂ����ɂ���B\n�܂��͑{������n�߂�񂾁B",
        "[�L�[�X]\n�����B"
    };
    [SerializeField] private string missionTitle = "�ڋ�";
    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    public override void EventFire()
    {
        missionItems = new string[] {
            $"�G�̕����ɐڋ߂���"
        };
        mDisp = HUDMissionDisplay.mainDisplay;
        StartCoroutine("Event");
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator Event()
    {
        yield return new WaitForSeconds(1);
        message.function(messageText[0], 2.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[1], 4.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[2], 3.7f);
        mDisp.SetMissionTitle(missionTitle);
        foreach (string s in missionItems) mDisp.GetMissionList().AddMissionItem(s);
        yield return new WaitForSeconds(5);
        message.function(messageText[3], 1.5f);
        ParmitNext();

    }
}
