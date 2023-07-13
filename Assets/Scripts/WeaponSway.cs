using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    // ���� ��ġ
    private Vector3 originPos;

    // ���� ��ġ
    private Vector3 currentPos;

    // sway �Ѱ�
    [SerializeField]
    private Vector3 limitPos;
    [SerializeField]
    private Vector3 fineSightLimitPos; // ������ ���¿����� �� ��鸮��

    // ������ �ε巯�� ����
    [SerializeField]
    private Vector3 smoothSway;

    // �ʿ��� ������Ʈ
    [SerializeField]
    private GunController theGunController; // ������ ���� �޾ƿ���

    // Start is called before the first frame update
    void Start()
    {
        originPos = this.transform.localPosition; // ���� �ڱ� �ڽ��� ��ġ���� �״�� ����
    }

    // Update is called once per frame
    void Update()
    {
        // �κ��丮�� Ȱ��ȭ�Ǹ� ������ ������
        if(Inventory.inventoryActivated == false)
            TrySway();
    }

    private void TrySway()
    {
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0) // ���콺�� �����¿� �����̸�
            Swaying();
        else // ���콺�� ���߸�
            BackToOriginPos();
    }
    
    private void Swaying()
    {
        // ���콺�� ������ �� �� ���� ������ ���� ����
        float _moveX = Input.GetAxisRaw("Mouse X");
        float _moveY = Input.GetAxisRaw("Mouse Y");

        if(!theGunController.isFineSightMode) // ������ ���°� �ƴ� ���� ��鸲
        {
            // ȭ������� ����� �ʵ��� Math.Clamp, �ڿ������� �����̵��� Math.Lerp
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -limitPos.x, limitPos.x),
                           Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.x), -limitPos.y, limitPos.y),
                           originPos.z);
        }
        else // ������ ������ ���� ��鸲
        {
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.y), -fineSightLimitPos.x, fineSightLimitPos.x),
                           Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.y), -fineSightLimitPos.y, fineSightLimitPos.y),
                           originPos.z);
        }

        transform.localPosition = currentPos; // ���� ����
    }

    private void BackToOriginPos()
    {
        // �� ��ġ�� ���ƿ���
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x);
        transform.localPosition = currentPos; 
    }
}
