using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // ���ǵ� ���� ����
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    private float applySpeed; // walk, run ���� �ۼ��� �ʿ� X
    [SerializeField]
    private float crouchSpeed;

    [SerializeField]
    private float jumpForce;

    // ���� ����
    private bool isRun = false; // �ȴ��� �ٴ��� ����
    private bool isCrouch = false; // �ɾ��ִ��� �ƴ��� ����
    private bool isGround = true; // ���� �ִ��� �ƴ��� ����

    // �󸶳� ������ �����ϴ� ����
    [SerializeField]
    private float crouchPosY; // ���� �� ��ġ
    private float originPosY; // ���� ��ġ
    private float applyCrouchPosY;

    // �� ���� ���θ� ���� ������Ʈ
    private CapsuleCollider capsuleCollider;

    // ī�޶� �ΰ���
    [SerializeField]
    private float lookSensitivity; 

    //ī�޶� �Ѱ�
    [SerializeField]
    private float cameraRotatoionLimit;
    private float currentCameraRotationX = 0f; // ī�޶� ������ �ٶ󺸵���

    // �ʿ��� ������Ʈ
    [SerializeField]
    private Camera theCamera;

    private Rigidbody myRigid;

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed; // �޸��� ���� �ȱ� (�⺻��)

        //�ʱ�ȭ
        originPosY = theCamera.transform.localPosition.y; // ī�޶� ��ġ ����, ������ǥ ���
        applyCrouchPosY = originPosY; // �ɱ� ���� ���� ��ġ (�⺻��)
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

    // �ɱ� �õ�
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    // �ɱ� ����
    private void Crouch()
    {
        isCrouch = !isCrouch; // ����ġ����. isCrouch�� true�� false��, false�� true��

        if(isCrouch) // �ɾ��� ��
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else // �Ͼ�� ��
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());

    }

    // �ε巯�� �ɱ� ���� ����
    IEnumerator CrouchCoroutine() // �ٸ� �ڵ�� ���� ó��
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;


        while(_posY != applyCrouchPosY) // ������������ ī�޶� �̵��� �ε巴��
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f); // ����
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15) // �������� ���� ���� ���̱�
                break;
            yield return null; // �� ������ ���
        }

        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
    }

    // ���� üũ
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f); // Vector3.down ��� -transform.up ������� X 
                                                                                                               // capsuleCollider.bounds.extents.y : ĸ�� �ݶ��̴� y ���� ���� ũ�⸸ŭ ������
                                                                                                               // + 0.1f : ���鿡 ���� �� ���
    }

    // ���� �õ�
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround) 
        {
            Jump();
        }
    }

    // ���� ����
    private void Jump()
    {
        if (isCrouch) // �ɱ� -> ���� -> ���� ���� ����
            Crouch();

        myRigid.velocity = transform.up * jumpForce;
    }

    // �޸��� �õ�
    private void TryRun()
    {
        if(Input.GetKey(KeyCode.LeftShift)) // Ű ���� ��
        {
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift)) // Ű���� �� ��
        {
            RunningCancle();
        }
    }

    // �޸��� ����
    private void Running()
    {
        if (isCrouch) // �޸��� -> ���� -> �޸��� ���� ����
            Crouch();

        isRun = true;
        applySpeed = runSpeed; // �޸��� �ӵ��� �ٲٱ�
    }

    // �޸��� ���� ���
    private void RunningCancle()
    {
        isRun = false;
        applySpeed = walkSpeed; // �ȱ� �ӵ��� �ٲٱ�
    }

    // �����̱� ����
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // �¿� 
        float _moveDirZ = Input.GetAxisRaw("Vertical"); // �յ�

        Vector3 _moveHorizontal = transform.right * _moveDirX; // (1, 0, 0)
        Vector3 _moveVertical = transform.forward * _moveDirZ; // (0, 0, 1)

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed; // (1, 0, 1) = 2 -> (0.5, 0, 0.5) = 1 
                                                                                       // 1�� �������� ����ȭ
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime); // Time.deltaTime ������ �����̵��ϵ���
    }

    // ī�޶� ���� ȸ��
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y"); // 2����
        float _cameraRotationX = _xRotation * lookSensitivity; // ī�޶� �ӵ� ����
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotatoionLimit, cameraRotatoionLimit); // ī�޶� ���� ���α�
        
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    // ĳ���� �¿� ȸ��
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X"); // 2����
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity; // ĳ���� �ӵ� ����
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY)); // ���Ϸ����� ���ʹϾ����� ��ȯ
        
        // ���ʹϾ�� ���Ϸ��� ���� �ٸ�
        // Debug.Log(myRigid.rotation);
        // Debug.Log(myRigid.rotation.eulerAngles);

    }
}