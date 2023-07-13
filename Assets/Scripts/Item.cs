using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject // MonoBehaviour�� �޸� ���� ������Ʈ�� ���� �ʿ�X
{
    public string itemName; // �������� �̸�
    [TextArea]
    public string itemDesc; // �������� ����
    public ItemType itemType; // �������� ����
    public Sprite itemImage; // ������ ��������Ʈ (��������Ʈ: ĵ���� �ʿ�X, �̹���: ĵ���������� ��� ����)
    public GameObject itemPrefab; // �������� ������

    public string weaponType; // ���� ����

    public enum ItemType
    {
        Equipment,
        Used,
        Ingredient,
        ETC
    }
}
