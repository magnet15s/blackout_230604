using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

public class M02_first : MissionEventNode {
    // Start is called before the first frame update
    public float starttime;
    private string[] missionItems;
    private string[] messageText = new string[]{
        "[�I�y���[�^�[:���C��]\n�ڕW���W�ɐڋ߁A�^�[�Q�b�g�U���J�n�B\n�ϑ����x��3�B�A���A�I�[���O���[���B",
        "[�I�X�J�[����]\n�ڕW�̃T�[�o�[���́A����NMU�ɂ����\n�펞�x���Ԑ����~����Ă���B",
�@�@�@�@"[�I�X�J�[����]\n����̍��ł́A\n���т����͂�NMU�̑|����A��̓h���[�����f�[�^�T�[�o�[���֓˓�����B",
        "[�I�X�J�[����]\n�N�̓������ł��d�v���B\n���҂��Ă���B",
        "[�L�[�X]\n�����B"
    };
    [SerializeField] private string missionTitle = "�ڋ�";
    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    public override void EventFire() {
        starttime = Time.time;
        missionItems = new string[] {
            $"�G�f�[�^�T�[�o�[����\n�ڋ߂���"
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
    IEnumerator Event() {
        yield return new WaitForSeconds(1);
        message.function(messageText[0], 4.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[1], 4.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[2], 6f);
        mDisp.SetMissionTitle(missionTitle);
        foreach (string s in missionItems) mDisp.GetMissionList().AddMissionItem(s);
        yield return new WaitForSeconds(6);
        message.function(messageText[3], 3.7f);
        yield return new WaitForSeconds(3);
        message.function(messageText[4], 1.5f);
        ParmitNext();

    }
}
