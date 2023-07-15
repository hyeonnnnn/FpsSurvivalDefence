using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    [SerializeField] protected string animalName; // ������ �̸�
    [SerializeField] protected int hp; // ������ ü��

    [SerializeField] protected float walkSpeed; // �ȴ� �ӵ�
    [SerializeField] protected float runSpeed; // �ٴ� �ӵ�

    // protected float direction; // ����
    protected Vector3 destination; // ������

    // ���º���
    protected bool isAction; // �ൿ ������
    protected bool isWalking; // �ȴ� ������
    protected bool isRunning; // �ٴ� ������
    protected bool isDead; // �׾�����

    [SerializeField] protected float waitTime; // ��� �ð� (���, Ǯ���, �θ���)
    [SerializeField] protected float walkTime; // �ȴ� �ð� (�ȱ�)
    [SerializeField] protected float runTime; // �ٴ� �ð� (�ٱ�)
    protected float currentTime;

    // �ʿ��� ������Ʈ
    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected BoxCollider boxCol;
    protected AudioSource theAudio;
    protected NavMeshAgent nav;

    [SerializeField] protected AudioClip[] sound_pig_normal; // �ϻ� ���� 3��
    [SerializeField] protected AudioClip sound_hurt;
    [SerializeField] protected AudioClip sound_dead;

    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        theAudio = GetComponent<AudioSource>();
        currentTime = waitTime; // ó���� ��� ��Ű��
        isAction = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead == false) // ������� ����
        {
            Move(); // �ȱ� �� ���� ��ȯ�ϸ鼭 ������ ���ư���
            ElapseTime(); // �ð� ��� �Լ�
        }
    }

    protected void Move()
    {
        if (isWalking || isRunning)
            // rigid.MovePosition(transform.position + (transform.forward * applySpeed * Time.deltaTime)); // ������ �����̱�
            nav.SetDestination(transform.position + destination * 5f); // 5�� �ӵ��� ��������

    }

    // �ð� ���
    protected void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                ResetBehavior();// ���� �ൿ
            }
        }
    }

    // ���� �ൿ�� ���� �ʱ�ȭ
    protected virtual void ResetBehavior()
    {
        isWalking = false; isRunning = false; isAction = true;
        nav.speed = walkSpeed; // �ȱⰡ �⺻
        nav.ResetPath(); // ������ ���ֱ� (�ε�Ÿ��� ����X)
        anim.SetBool("Walking", isWalking);
        anim.SetBool("Running", isRunning);
        // direction.Set(0f, Random.Range(0f, 360f), 0f); // ���� ����
        destination.Set(Random.Range(-0.2f, 0.2f), 0f, Random.Range(0.5f, 1f)); // ��ǥ ���
    }

    protected void TryWalk()
    {
        isWalking = true;
        anim.SetBool("Walking", isWalking);
        currentTime = walkTime;
        nav.speed = walkSpeed;
        Debug.Log("�ȱ�");
    }

    public virtual void Damage(int _dmg, Vector3 _targetPos) // ������ ������ Run() ȣ��
    {
        if (isDead == false) // ������� ����
        {
            hp -= _dmg;

            if (hp <= 0)
            {
                Dead();
                return;
            }

            PlaySE(sound_hurt);
            anim.SetTrigger("Hurt");
        }
    }

    protected void Dead()
    {
        PlaySE(sound_dead);
        isWalking = false;
        isRunning = false;
        isDead = true;
        anim.SetTrigger("Dead");
    }

    protected void RandomSound()
    {
        int _random = Random.Range(0, 3); // �ϻ� ���� 3��
        PlaySE(sound_pig_normal[_random]);
    }

    protected void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }
}
