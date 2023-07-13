using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; // ���� ������ �ִ� �Ÿ�

    private bool pickupActivated = false; // ������ ������ �� true

    private RaycastHit hitInfo; // �浹ü ���� ����

    // ������ ���̾ ���ؼ��� �����ϵ��� ���̾� ����ũ ����
    [SerializeField]
    private LayerMask layerMask;

    // �ʿ��� ������Ʈ
    [SerializeField]
    private TMP_Text actionText;
    [SerializeField]
    private Inventory theInventory;

    // Update is called once per frame
    void Update()
    {
        CheckItem();
        TryAction();
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckItem(); // �������� �ִ��� Ȯ��
            CanPickUp(); // ������ �ݱ�
        }
    }

    private void CanPickUp()
    {
        if (pickupActivated)
        {
            if(hitInfo.transform != null)
            {
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject); // ������ ������� �ϱ�
                InfoDisappear(); // �� �̻� �� �ݰ� �ϱ�
            }
        }
    }

    private void CheckItem()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
        }
        else // �������� ��������
            InfoDisappear();
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = "Picking up a(an) " + hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + "<color=yellow>" + " (E)" + "</color>";
    }

    private void InfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }
}
