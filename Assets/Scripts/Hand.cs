using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public string handName; // 맨손과 너클 구분
    public float range; // 공격 범위
    public int damage; // 공격력
    public float workSpeed; // 작업 속도
    public float attackDelay; // 공격 딜레이
    public float attackDelayA; // 공격 활성화 시점 딜레이 (데미지O)
    public float attackDelayB; // 공격 비활성화 시점 딜레이 (데미지X)

    // 스타트, 업데이트 함수 있는 것만으로 자원 소모

    public Animator anim;
    // 데미지 범위를 플레이어 시점으로. 박스 콜라이더 사용X
}
