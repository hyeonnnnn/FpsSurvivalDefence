using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float lookSensitivity; // ī�޶� �ΰ���

    [SerializeField]
    private float cameraRotatoionLimit; // ī�޶� ���� ����
    private float currentCameraRotationX = 0f; // ī�޶� ������ �ٶ󺸵���

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
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // �¿� 
        float _moveDirZ = Input.GetAxisRaw("Vertical"); // �յ�

        Vector3 _moveHorizontal = transform.right * _moveDirX; // (1, 0, 0)
        Vector3 _moveVertical = transform.forward * _moveDirZ; // (0, 0, 1)

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed; // (1, 0, 1) = 2 -> (0.5, 0, 0.5) = 1 
                                                                                      // 1�� �������� ����ȭ
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime); // Time.deltaTime ������ �����̵��ϵ���
    }

    private void CameraRotation()
    {
        // ī�޶� ���� ȸ��
        float _xRotation = Input.GetAxisRaw("Mouse Y"); // 2����
        float _cameraRotationX = _xRotation * lookSensitivity; // ī�޶� �ӵ� ����
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotatoionLimit, cameraRotatoionLimit); // ī�޶� ���� ���α�
        
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private void CharacterRotation()
    {
        // ĳ���� �¿� ȸ��
        float _yRotation = Input.GetAxisRaw("Mouse X"); // 2����
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity; // ĳ���� �ӵ� ����
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY)); // ���Ϸ����� ���ʹϾ����� ��ȯ
        
        // ���ʹϾ�� ���Ϸ��� ���� �ٸ�
        // Debug.Log(myRigid.rotation);
        // Debug.Log(myRigid.rotation.eulerAngles);

    }
}
