using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// �_�C�A���O�̃A�j���[�V����
/// </summary>
public class AnimatedDialog : MonoBehaviour {
    // �A�j���[�^�[
    [SerializeField] private Animator _animator;

    // �A�j���[�^�[�R���g���[���[�̃��C���[(�ʏ��0)
    [SerializeField] private int _layer;

    // IsOpen�t���O(�A�j���[�^�[�R���g���[���[���Œ�`�����t���O)
    private static readonly int ParamIsOpen = Animator.StringToHash("isOpen");

    // �_�C�A���O�͊J���Ă��邩�ǂ���
    public bool IsOpen => gameObject.activeSelf;
    public AudioClip sound1;
    public AudioClip sound2;
    AudioSource audioSource;
    // �A�j���[�V���������ǂ���
    public bool IsTransition { get; private set; }
    void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    // �_�C�A���O���J��
    public void Open() {
        // �s������h�~
        //if (IsOpen || IsTransition) return;

        // �p�l�����̂��A�N�e�B�u�ɂ���
        //gameObject.SetActive(true);

        // IsOpen�t���O���Z�b�g
        audioSource.PlayOneShot(sound1);
        _animator.SetBool(ParamIsOpen, true);

        // �A�j���[�V�����ҋ@
        StartCoroutine(WaitAnimation("Shown"));
    }

    // �_�C�A���O�����
    public void Close() {
        // �s������h�~
        //if (!IsOpen || IsTransition) return;

        // IsOpen�t���O���N���A
        audioSource.PlayOneShot(sound2);
        _animator.SetBool(ParamIsOpen, false);

        // �A�j���[�V�����ҋ@���A�I�������p�l�����̂��A�N�e�B�u�ɂ���
        //StartCoroutine(WaitAnimation("Hidden", () => gameObject.SetActive(false)));
    }

    // �J�A�j���[�V�����̑ҋ@�R���[�`��
    private IEnumerator WaitAnimation(string stateName, UnityAction onCompleted = null) {
        IsTransition = true;

        yield return new WaitUntil(() =>
        {
            // �X�e�[�g���ω����A�A�j���[�V�������I������܂Ń��[�v
            var state = _animator.GetCurrentAnimatorStateInfo(_layer);
            return state.IsName(stateName) && state.normalizedTime >= 1;
        });

        IsTransition = false;

        onCompleted?.Invoke();
    }
}