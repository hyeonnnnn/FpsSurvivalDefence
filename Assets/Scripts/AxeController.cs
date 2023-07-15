using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : CloseWeaponController // CloseWeaponController 상속 받음
{
    // 활성화 여부
    public static bool isActivate = true; // 초기에 활성화

    private void Start()
    {
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;
    }

    private void Update()
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
                if (hitInfo.transform.tag == "Rock") // 바위를 때리면
                    hitInfo.transform.GetComponent<Rock>().Mining();
                
                else if (hitInfo.transform.tag == "WeakAnimal") // WeakAnimal을 때리면
                {
                    SoundManager.instance.PlaySE("Animal_Hit");
                    hitInfo.transform.GetComponent<WeakAnimal>().Damage(1, transform.position);
                }

                isSwing = false;
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
