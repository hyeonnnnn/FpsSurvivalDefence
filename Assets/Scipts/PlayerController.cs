using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float lookSensitivity; // 카메라 민감도

    [SerializeField]
    private float cameraRotatoionLimit; // 카메라 각도 제한
    private float currentCameraRotationX = 0f; // 카메라가 정면을 바라보도록

    [SerializeField]
    private Camera theCamera;

    private Rigidbody myRigid;

    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CameraRotation();
        CharacterRotation();
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // 좌우 
        float _moveDirZ = Input.GetAxisRaw("Vertical"); // 앞뒤

        Vector3 _moveHorizontal = transform.right * _moveDirX; // (1, 0, 0)
        Vector3 _moveVertical = transform.forward * _moveDirZ; // (0, 0, 1)

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed; // (1, 0, 1) = 2 -> (0.5, 0, 0.5) = 1 
                                                                                      // 1이 나오도록 정규화
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime); // Time.deltaTime 없으면 순간이동하듯이
    }

    private void CameraRotation()
    {
        // 카메라 상하 회전
        float _xRotation = Input.GetAxisRaw("Mouse Y"); // 2차원
        float _cameraRotationX = _xRotation * lookSensitivity; // 카메라 속도 조절
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotatoionLimit, cameraRotatoionLimit); // 카메라 각도 가두기
        
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private void CharacterRotation()
    {
        // 캐릭터 좌우 회전
        float _yRotation = Input.GetAxisRaw("Mouse X"); // 2차원
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity; // 캐릭터 속도 조절
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY)); // 오일러값을 쿼터니언으로 변환
        
        // 쿼터니언과 오일러값 완전 다름
        // Debug.Log(myRigid.rotation);
        // Debug.Log(myRigid.rotation.eulerAngles);

    }
}
