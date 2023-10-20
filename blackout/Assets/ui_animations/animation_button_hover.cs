using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class animation_button_hover : MonoBehaviour
{

    [SerializeField] private Animator _animator;

    // �A�j���[�^�[�R���g���[���[�̃��C���[(�ʏ��0)
    [SerializeField] private int _layer;

    // IsOpen�t���O(�A�j���[�^�[�R���g���[���[���Œ�`�����t���O)
    private static readonly int ParamIsOpen = Animator.StringToHash("isHover");
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var trigger = GetComponent<EventTrigger>();
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;

        // ���X�i�[�͒P����Log���o�͂��邾���̏����ɂ���
        entry.callback.AddListener((data) => { Debug.Log("PointerEnter"); });
    }

    public void OnMouseExit() {
        _animator.SetBool(ParamIsOpen, false);
        Debug.Log("exit");
    }
    public void OnMouseEnter() {
        _animator.SetBool(ParamIsOpen, true);
        Debug.Log("enter");
    }
}
