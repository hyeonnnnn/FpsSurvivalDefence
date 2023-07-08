using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController // CloseWeaponController ��� ����
{
    // Ȱ��ȭ ����
    public static bool isActivate = false; // �ʱ⿡ ��Ȱ��ȭ

    private void Update() // Update �Լ��� �ڽ� Ŭ�������� 
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
