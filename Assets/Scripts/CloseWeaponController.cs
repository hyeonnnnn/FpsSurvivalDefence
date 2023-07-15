using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �̿ϼ� Ŭ����. �߻� Ŭ����.
public abstract class CloseWeaponController : MonoBehaviour
{
    // ���� ������ Hand�� Ÿ�� ����
    [SerializeField]
    protected CloseWeapon currentCloseWeapon; // �ִϸ��̼� ��������, ������: public 
                                              // �ڽĵ鿡�Ը� ���� ���

    // ���� ������
    protected bool isAttack = false;
    protected bool isSwing = false;

    protected RaycastHit hitInfo;
    [SerializeField]
    protected LayerMask layerMask; // �÷��̾�(�ڱ��ڽ�)���� �浹�� ���ϱ� ����

    protected void TryAttack()
    {
        // �κ��丮�� Ȱ��ȭ�Ǹ� ���� �Ұ���
        if(Inventory.inventoryActivated == false)
        {
            if (Input.GetButton("Fire1")) // ��Ŭ���ϸ�
            {
                if (!isAttack)
                {
                    StartCoroutine(AttackCoroutine()); // �����ϴ� �ڷ�ƾ ����
                }
            }
        }
    }

    protected IEnumerator AttackCoroutine()
    {
        isAttack = true;
        currentCloseWeapon.anim.SetTrigger("Attack"); // Attack �ִϸ��̼� ����

        yield return new WaitForSeconds(currentCloseWeapon.attackDelayA); // ���� �ð� �Ŀ�

        // �ֵθ� �������� ���ߵǾ�� ��
        isSwing = true;
        StartCoroutine(HitCoroutine()); // ���߽�Ű�� �ڷ�ƾ
        yield return new WaitForSeconds(currentCloseWeapon.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentCloseWeapon.attackDelay - currentCloseWeapon.attackDelayA - currentCloseWeapon.attackDelayB); // ���� �ð� �Ŀ�
        isAttack = false; // �ٽ� ������ �� ����
    }

    // �߻� �ڷ�ƾ. �ڽ� Ŭ�������� �ϼ���ų ��
    protected abstract IEnumerator HitCoroutine(); // ���� ������ �˾ƺ��� �ڷ�ƾ

    protected bool checkObject() // ���ǹ��� ������ bool
    {
        // ���濡 ������ �ִ���
        // ĳ������ ��ġ����, ��������, �浹ü�� �ִٸ� �浹ü�� ������ hitInfo����, �� ������ŭ ������
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range, layerMask))
        {
            return true;
        }
        return false;
    }

    // ��ü
    // ���� �Լ�. (�ϼ������� �߰� ������ ������ �Լ�)
    public virtual void CloseWeaponChange(CloseWeapon _CloseWeapon) // WeaponManager���� HandChange ȣ�� -> public
    {
        if (WeaponManager.currentWeapon != null) // ���𰡸� ��� �ִ� ���
            WeaponManager.currentWeapon.gameObject.SetActive(false); // ��Ȱ��ȭ

        currentCloseWeapon = _CloseWeapon; // �ٲ� ���� ���� ������
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim; // �� ��Ʈ�ѷ����� ������ �ʿ�X

        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true); // Ȱ��ȭ
    }
}
