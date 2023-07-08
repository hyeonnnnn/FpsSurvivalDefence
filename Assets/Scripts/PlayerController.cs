using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // 스피드 조정 변수
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    private float applySpeed; // walk, run 따로 작성할 필요 X
    [SerializeField]
    private float crouchSpeed;

    [SerializeField]
    private float jumpForce;

    // 상태 변수
    private bool isWalk = false; // 걷고 있는지 아닌지 구분
    private bool isRun = false; // 뛰고 있는지 아닌지 구분
    private bool isCrouch = false; // 앉아있는지 아닌지 구분
    private bool isGround = true; // 땅에 있는지 아닌지 구분

    // 움직임 체크 변수
    private Vector3 lastPos; // 전 프레임의 현재 위치

    // 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float crouchPosY; // 앉을 때 위치
    private float originPosY; // 원래 위치
    private float applyCrouchPosY;

    // 땅 착지 여부를 위한 컴포넌트
    private CapsuleCollider capsuleCollider;

    // 카메라 민감도
    [SerializeField]
    private float lookSensitivity; 

    //카메라 한계
    [SerializeField]
    private float cameraRotatoionLimit;
    private float currentCameraRotationX = 0f; // 카메라가 정면을 바라보도록

    // 필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;
    private GunController theGunController;
    private Crosshair theCrosshair;
    private StatusController theStatusController;

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        theGunController = FindObjectOfType<GunController>(); // 하이어라키에 있는, GunController 있는 객체 찾기
        theCrosshair = FindObjectOfType<Crosshair>();
        theStatusController = FindObjectOfType<StatusController>();

        //초기화
        applySpeed = walkSpeed; // 달리기 전에 걷기 (기본값)
        originPosY = theCamera.transform.localPosition.y; // 카메라 위치 변경, 로컬좌표 사용
        applyCrouchPosY = originPosY; // 앉기 전에 원래 위치 (기본값)
    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        TryCrouch();
        Move();
        CameraRotation();
        CharacterRotation();
    }

    private void FixedUpdate()
    {
        MoveCheck(); // 업데이트 간격이 여유로워짐
    }

    // 앉기 시도
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    // 앉기 동작
    private void Crouch()
    {
        isCrouch = !isCrouch; // 스위치역할. isCrouch가 true면 false로, false면 true로
        theCrosshair.CrouchingAnimation(isCrouch);

        if (isCrouch) // 앉았을 때
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else // 일어났을 때
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());

    }

    // 부드러운 앉기 동작 실행
    IEnumerator CrouchCoroutine() // 다른 코드와 병렬 처리
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;


        while(_posY != applyCrouchPosY) // 목적지까지의 카메라 이동을 부드럽게
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f); // 보간
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15) // 보간으로 인한 오차 줄이기
                break;
            yield return null; // 한 프레임 대기
        }

        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
    }

    // 지면 체크
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.2f); // Vector3.down 대신 -transform.up 사용하지 X 
                                                                                                               // capsuleCollider.bounds.extents.y : 캡슐 콜라이더 y 값의 절반 크기만큼 레이저
                                                                                                               // + 0.1f : 경사면에 있을 때 대비
        theCrosshair.JumpAnimation(!isGround);
    }

    // 점프 시도
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround && theStatusController.GetCurrentSP() > 0) 
        {
            Jump();
        }
    }

    // 점프 동작
    private void Jump()
    {
        if (isCrouch) // 앉기 -> 점프 -> 앉은 상태 해제
            Crouch();
        theStatusController.DecreaseStamina(100);
        myRigid.velocity = transform.up * jumpForce;
    }

    // 달리기 시도
    private void TryRun()
    {
        if(Input.GetKey(KeyCode.LeftShift) && theStatusController.GetCurrentSP() > 0) // 키 누를 때
        {
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift) || theStatusController.GetCurrentSP() <= 0) // 키에서 뗄 때
        {
            RunningCancle();
        }
    }

    // 달리기 동작
    private void Running()
    {
        if (isCrouch) // 달리기 -> 점프 -> 달리는 상태 해제
            Crouch();

        theGunController.CancelFineSight(); // 뛸 때 정조준 모드 해제

        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        theStatusController.DecreaseStamina(2);
        applySpeed = runSpeed; // 달리기 속도로 바꾸기
    }

    // 달리기 동작 취소
    private void RunningCancle()
    {
        isRun = false;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = walkSpeed; // 걷기 속도로 바꾸기
    }

    // 움직이기 동작
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // 좌우 
        float _moveDirZ = Input.GetAxisRaw("Vertical"); // 앞뒤

        Vector3 _moveHorizontal = transform.right * _moveDirX; // (1, 0, 0)
        Vector3 _moveVertical = transform.forward * _moveDirZ; // (0, 0, 1)

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed; // (1, 0, 1) = 2 -> (0.5, 0, 0.5) = 1 
                                                                                       // 1이 나오도록 정규화
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime); // Time.deltaTime 없으면 순간이동하듯이
    }

    // 움직임 체크
    private void MoveCheck()
    {
        // 달리지 않을 때, 움크리고 있지 않을 때, 땅에 있을 때만 걷고 있는지 여부 체크
        if (!isRun && !isCrouch && isGround)
        {
            if (Vector3.Distance(lastPos, transform.position) >= 0.01f) // 경사면에서는 가만히 있어도 미끄러기 때문에 이를 방지
                isWalk = true;
            else
                isWalk = false;

            theCrosshair.WalkingAnimation(isWalk);
            lastPos = transform.position;
        }
    }

    // 카메라 상하 회전
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y"); // 2차원
        float _cameraRotationX = _xRotation * lookSensitivity; // 카메라 속도 조절
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotatoionLimit, cameraRotatoionLimit); // 카메라 각도 가두기
        
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    // 캐릭터 좌우 회전
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X"); // 2차원
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity; // 캐릭터 속도 조절
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY)); // 오일러값을 쿼터니언으로 변환
        
        // 쿼터니언과 오일러값 완전 다름
        // Debug.Log(myRigid.rotation);
        // Debug.Log(myRigid.rotation.eulerAngles);

    }
}
