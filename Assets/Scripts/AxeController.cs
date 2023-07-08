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
                if (hitInfo.transform.tag == "Rock") // ������ �ε�����
                {
                    hitInfo.transform.GetComponent<Rock>().Mining();
                }
                isSwing = false;
                Debug.Log(hitInfo.transform.name); // �浹ü�� �̸� ���
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
