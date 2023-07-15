using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 미완성 클래스. 추상 클래스.
public abstract class CloseWeaponController : MonoBehaviour
{
    // 현재 장착된 Hand형 타입 무기
    [SerializeField]
    protected CloseWeapon currentCloseWeapon; // 애니메이션 가져오기, 변수들: public 
                                              // 자식들에게만 접근 허용

    // 공격 중인지
    protected bool isAttack = false;
    protected bool isSwing = false;

    protected RaycastHit hitInfo;
    [SerializeField]
    protected LayerMask layerMask; // 플레이어(자기자신)와의 충돌을 피하기 위해

    protected void TryAttack()
    {
        // 인벤토리가 활성화되면 공격 불가능
        if(Inventory.inventoryActivated == false)
        {
            if (Input.GetButton("Fire1")) // 좌클릭하면
            {
                if (!isAttack)
                {
                    StartCoroutine(AttackCoroutine()); // 공격하는 코루틴 실행
                }
            }
        }
    }

    protected IEnumerator AttackCoroutine()
    {
        isAttack = true;
        currentCloseWeapon.anim.SetTrigger("Attack"); // Attack 애니메이션 실행

        yield return new WaitForSeconds(currentCloseWeapon.attackDelayA); // 일정 시간 후에

        // 휘두른 순간에만 적중되어야 함
        isSwing = true;
        StartCoroutine(HitCoroutine()); // 적중시키는 코루틴
        yield return new WaitForSeconds(currentCloseWeapon.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentCloseWeapon.attackDelay - currentCloseWeapon.attackDelayA - currentCloseWeapon.attackDelayB); // 일정 시간 후에
        isAttack = false; // 다시 공격할 수 있음
    }

    // 추상 코루틴. 자식 클래스에서 완성시킬 것
    protected abstract IEnumerator HitCoroutine(); // 공격 적중을 알아보는 코루틴

    protected bool checkObject() // 조건문에 쓰여서 bool
    {
        // 전방에 무엇이 있는지
        // 캐릭터의 위치에서, 정면으로, 충돌체가 있다면 충돌체의 정보를 hitInfo에서, 손 범위만큼 레이저
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range, layerMask))
        {
            return true;
        }
        return false;
    }

    // 교체
    // 가상 함수. (완성이지만 추가 편집이 가능한 함수)
    public virtual void CloseWeaponChange(CloseWeapon _CloseWeapon) // WeaponManager에서 HandChange 호출 -> public
    {
        if (WeaponManager.currentWeapon != null) // 무언가를 들고 있는 경우
            WeaponManager.currentWeapon.gameObject.SetActive(false); // 비활성화

        currentCloseWeapon = _CloseWeapon; // 바꿀 것을 현재 손으로
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim; // 각 컨트롤러마다 설정할 필요X

        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true); // 활성화
    }
}
