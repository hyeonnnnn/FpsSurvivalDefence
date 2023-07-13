using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    // 기존 위치
    private Vector3 originPos;

    // 현재 위치
    private Vector3 currentPos;

    // sway 한계
    [SerializeField]
    private Vector3 limitPos;
    [SerializeField]
    private Vector3 fineSightLimitPos; // 정조준 상태에서는 덜 흔들리게

    // 움직임 부드러움 정도
    [SerializeField]
    private Vector3 smoothSway;

    // 필요한 컴포넌트
    [SerializeField]
    private GunController theGunController; // 정조준 상태 받아오기

    // Start is called before the first frame update
    void Start()
    {
        originPos = this.transform.localPosition; // 현재 자기 자신의 위치값을 그대로 대입
    }

    // Update is called once per frame
    void Update()
    {
        // 인벤토리가 활성화되면 스웨이 가만히
        if(Inventory.inventoryActivated == false)
            TrySway();
    }

    private void TrySway()
    {
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0) // 마우스가 상하좌우 움직이면
            Swaying();
        else // 마우스가 멈추면
            BackToOriginPos();
    }
    
    private void Swaying()
    {
        // 마우스가 움직일 때 그 값을 변수에 각각 대입
        float _moveX = Input.GetAxisRaw("Mouse X");
        float _moveY = Input.GetAxisRaw("Mouse Y");

        if(!theGunController.isFineSightMode) // 정조준 상태가 아닐 때의 흔들림
        {
            // 화면밖으로 벗어나지 않도록 Math.Clamp, 자연스럽게 움직이도록 Math.Lerp
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -limitPos.x, limitPos.x),
                           Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.x), -limitPos.y, limitPos.y),
                           originPos.z);
        }
        else // 정조준 상태일 때의 흔들림
        {
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.y), -fineSightLimitPos.x, fineSightLimitPos.x),
                           Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.y), -fineSightLimitPos.y, fineSightLimitPos.y),
                           originPos.z);
        }

        transform.localPosition = currentPos; // 실제 적용
    }

    private void BackToOriginPos()
    {
        // 원 위치로 돌아오기
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x);
        transform.localPosition = currentPos; 
    }
}
