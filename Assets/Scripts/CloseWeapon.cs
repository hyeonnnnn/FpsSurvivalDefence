using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWeapon : MonoBehaviour
{
    public string closeWeaponName; // ���� ���� �̸�

    public bool isHand; // �Ǽ�����
    public bool isAxe; // ��������
    public bool isPickaxe; // �������

    public float range; // ���� ����
    public int damage; // ���ݷ�
    public float workSpeed; // �۾� �ӵ�
    public float attackDelay; // ���� ������
    public float attackDelayA; // ���� Ȱ��ȭ ���� ������ (������O)
    public float attackDelayB; // ���� ��Ȱ��ȭ ���� ������ (������X)

    // ��ŸƮ, ������Ʈ �Լ� �ִ� �͸����� �ڿ� �Ҹ�

    public Animator anim;
    // ������ ������ �÷��̾� ��������. �ڽ� �ݶ��̴� ���X
}
