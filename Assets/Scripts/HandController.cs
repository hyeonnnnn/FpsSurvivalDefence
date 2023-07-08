using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController // CloseWeaponController 상속 받음
{
    // 활성화 여부
    public static bool isActivate = false; // 초기에 비활성화

    private void Update() // Update 함수는 자식 클래스에서 
    {
        if (isActivate)
            TryAttack();
    }

    protected override IEnumerator HitCoroutine() // 재정의
    {
        while (isSwing) // A와 B 사이에서 계속 공격 체크
        {
            if (checkObject())
            {
                isSwing = false;
                Debug.Log(hitInfo.transform.name); // 충돌체의 이름 출력
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _CloseWeapon)
    {
        base.CloseWeaponChange(_CloseWeapon);
        isActivate = true;
    }
}
