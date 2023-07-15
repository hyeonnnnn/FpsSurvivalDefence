using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : CloseWeaponController // CloseWeaponController ��� ����
{
    // Ȱ��ȭ ����
    public static bool isActivate = true; // �ʱ⿡ Ȱ��ȭ

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

    protected override IEnumerator HitCoroutine() // ������
    {
        while (isSwing) // A�� B ���̿��� ��� ���� üũ
        {
            if (checkObject())
            {
                if (hitInfo.transform.tag == "Rock") // ������ ������
                    hitInfo.transform.GetComponent<Rock>().Mining();
                
                else if (hitInfo.transform.tag == "WeakAnimal") // WeakAnimal�� ������
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
