using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    // ���� ������ Hand�� Ÿ�� ����
    [SerializeField]
    private Hand currentHand; // �ִϸ��̼� ��������
                              // Hand ������: public

    // ���� ������
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
        if(Input.GetButton("Fire1")) // Input Manager���� ���� (���� ��Ʈ���� ����. ��Ŭ����)
        {
            if(isAttack)
            {
                StartCoroutine(AttackCoroutine()); // �ڷ�ƾ ����
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttack = true;
        currentHand.anim.SetTrigger("Attack"); // Attack �ִϸ��̼� ����

        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        // ���� Ȱ��ȭ ����
        HitCoroutine();

        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);
        isAttack = false;
    }

    IEnumerator HitCoroutine() // ���� ������ �˾ƺ��� �ڷ�ƾ, �ϳ� �����ϸ� �� �� �ٽ� ����X
    {
        while (isSwing) // A�� B ���̿��� ��� ���� üũ
        {
            if(checkObject())
            {
                isSwing = false;
                Debug.Log(hitinfo.transform.name); // �浹ü�� �̸� ���
            }
            yield return null;
        }
    }

    private bool checkObject() // ���ǹ��� ������ bool
    {
        // ���濡 ������ �ִ���
        // ĳ������ ��ġ����, ��������, �浹ü�� �ִٸ� �浹ü�� ������ hitfo����, �� ������ŭ ������
        if (Physics.Raycast(transform.position, transform.forward, out hitinfo, currentHand.range))
        {
            return true;
        }
        return false;
    }
}
