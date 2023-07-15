using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    [SerializeField] protected string animalName; // 동물의 이름
    [SerializeField] protected int hp; // 동물의 체력

    [SerializeField] protected float walkSpeed; // 걷는 속도
    [SerializeField] protected float runSpeed; // 뛰는 속도

    // protected float direction; // 방향
    protected Vector3 destination; // 목적지

    // 상태변수
    protected bool isAction; // 행동 중인지
    protected bool isWalking; // 걷는 중인지
    protected bool isRunning; // 뛰는 중인지
    protected bool isDead; // 죽었는지

    [SerializeField] protected float waitTime; // 대기 시간 (대기, 풀뜯기, 두리번)
    [SerializeField] protected float walkTime; // 걷는 시간 (걷기)
    [SerializeField] protected float runTime; // 뛰는 시간 (뛰기)
    protected float currentTime;

    // 필요한 컴포넌트
    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected BoxCollider boxCol;
    protected AudioSource theAudio;
    protected NavMeshAgent nav;

    [SerializeField] protected AudioClip[] sound_pig_normal; // 일상 사운드 3개
    [SerializeField] protected AudioClip sound_hurt;
    [SerializeField] protected AudioClip sound_dead;

    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        theAudio = GetComponent<AudioSource>();
        currentTime = waitTime; // 처음에 대기 시키기
        isAction = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead == false) // 살아있을 때만
        {
            Move(); // 걷기 때 방향 전환하면서 앞으로 나아가기
            ElapseTime(); // 시간 경과 함수
        }
    }

    protected void Move()
    {
        if (isWalking || isRunning)
            // rigid.MovePosition(transform.position + (transform.forward * applySpeed * Time.deltaTime)); // 앞으로 움직이기
            nav.SetDestination(transform.position + destination * 5f); // 5배 속도로 도망가기

    }

    // 시간 경과
    protected void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                ResetBehavior();// 다음 행동
            }
        }
    }

    // 다음 행동을 위한 초기화
    protected virtual void ResetBehavior()
    {
        isWalking = false; isRunning = false; isAction = true;
        nav.speed = walkSpeed; // 걷기가 기본
        nav.ResetPath(); // 목적지 없애기 (부들거리는 현상X)
        anim.SetBool("Walking", isWalking);
        anim.SetBool("Running", isRunning);
        // direction.Set(0f, Random.Range(0f, 360f), 0f); // 랜덤 방향
        destination.Set(Random.Range(-0.2f, 0.2f), 0f, Random.Range(0.5f, 1f)); // 좌표 찍기
    }

    protected void TryWalk()
    {
        isWalking = true;
        anim.SetBool("Walking", isWalking);
        currentTime = walkTime;
        nav.speed = walkSpeed;
        Debug.Log("걷기");
    }

    public virtual void Damage(int _dmg, Vector3 _targetPos) // 데미지 받으면 Run() 호출
    {
        if (isDead == false) // 살아있을 때만
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
        int _random = Random.Range(0, 3); // 일상 사운드 3개
        PlaySE(sound_pig_normal[_random]);
    }

    protected void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }
}
