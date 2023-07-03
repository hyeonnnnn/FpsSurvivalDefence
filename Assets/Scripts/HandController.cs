using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    // 현재 장착된 Hand형 타입 무기
    [SerializeField]
    private Hand currentHand; // 애니메이션 가져오기
                              // Hand 변수들: public

    // 공격 중인지
    private bool isAttack = false;
    private bool isSwing = false;

    private RaycastHit hitinfo;

    // Update is called once per frame
    void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        if(Input.GetButton("Fire1")) // Input Manager에서 수정 (왼쪽 컨트롤은 삭제. 좌클릭만)
        {
            if(isAttack)
            {
                StartCoroutine(AttackCoroutine()); // 코루틴 실행
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttack = true;
        currentHand.anim.SetTrigger("Attack"); // Attack 애니메이션 실행

        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        // 공격 활성화 시점
        HitCoroutine();

        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);
        isAttack = false;
    }

    IEnumerator HitCoroutine() // 공격 적중을 알아보는 코루틴, 하나 적중하면 두 번 다시 실핼X
    {
        while (isSwing) // A와 B 사이에서 계속 공격 체크
        {
            if(checkObject())
            {
                isSwing = false;
                Debug.Log(hitinfo.transform.name); // 충돌체의 이름 출력
            }
            yield return null;
        }
    }

    private bool checkObject() // 조건문에 쓰여서 bool
    {
        // 전방에 무엇이 있는지
        // 캐릭터의 위치에서, 정면으로, 충돌체가 있다면 충돌체의 정보를 hitfo에서, 손 범위만큼 레이저
        if (Physics.Raycast(transform.position, transform.forward, out hitinfo, currentHand.range))
        {
            return true;
        }
        return false;
    }
}
