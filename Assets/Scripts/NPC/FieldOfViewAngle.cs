using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle; // �þ߰� (120��)
    [SerializeField] private float viewDistance; // �þ߰Ÿ� (10����)
    [SerializeField] private LayerMask targetMask; // Ÿ�ٸ���ũ (�÷��̾�)

    private Pig thePig;

    void Start()
    {
        thePig = GetComponent<Pig>(); 
    }

    // Update is called once per frame
    void Update()
    {
        View();
    }

    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y; // �ޱۿ� y���� ������
                                           // �þ߰��� z�� �����ε� z�� ����� �� y�൵ �����̱� ����
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad)); // �ﰢ�Լ�. P1(x1, y1), P2(x2, y2) ���ϱ�
    }

    private void View()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f); // �������� ��
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f); // ���������� ��

        // ���������� ������ ����
        // ���� ��ĥ �� �����Ƿ� transform.up ���ϱ�
        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red); // ���� ��輱
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.red); // ���� ��輱

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask); // ���� �ݰ� �ȿ� �ִ� �ݶ��̴����� _target�� ����

        // �з�
        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform; // Ÿ���� transform ���� ������
            if(_targetTf.name == "Player") // �þ߰Ÿ� ���� �ִٸ�
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward); // ������ ������ ���� �÷��̾���� �� ������ ����

                if (_angle < viewAngle * 0.5f) // �þ߰� ���� �ִٸ�
                {
                    RaycastHit _hit;
                    if(Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance)) // ��ֹ��� �ִ���
                    {
                        if(_hit.transform.name == "Player") // �������� ���� ����� ���� ��� �� ���� ����
                        {
                            Debug.Log("�÷��̾ ������ �þ� ���� �ֽ��ϴ�");
                            thePig.Run(_hit.transform.position); // Ÿ���� ��ġ �Ѱܼ� �� �ݴ� �������� �޸���
                        }
                    }
                }
            }
        }
    }
}
