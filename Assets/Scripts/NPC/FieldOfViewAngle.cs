using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle; // 시야각 (120도)
    [SerializeField] private float viewDistance; // 시야거리 (10미터)
    [SerializeField] private LayerMask targetMask; // 타겟마스크 (플레이어)

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
        _angle += transform.eulerAngles.y; // 앵글에 y값을 더해줌
                                           // 시야각이 z축 기준인데 z축 기울일 때 y축도 움직이기 때문
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad)); // 삼각함수. P1(x1, y1), P2(x2, y2) 구하기
    }

    private void View()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f); // 왼쪽으로 반
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f); // 오른쪽으로 반

        // 디버깅용으로 레이저 보기
        // 땅과 겹칠 수 있으므로 transform.up 더하기
        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red); // 좌측 경계선
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.red); // 우측 경계선

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask); // 일정 반경 안에 있는 콜라이더들을 _target에 저장

        // 분류
        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform; // 타겟의 transform 정보 얻어오기
            if(_targetTf.name == "Player") // 시야거리 내에 있다면
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward); // 동물의 정면의 선과 플레이어와의 선 사이의 각도

                if (_angle < viewAngle * 0.5f) // 시야각 내에 있다면
                {
                    RaycastHit _hit;
                    if(Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance)) // 장애물이 있는지
                    {
                        if(_hit.transform.name == "Player") // 레이저를 맞은 대상이 벽인 경우 이 조건 무시
                        {
                            Debug.Log("플레이어가 동물의 시야 내에 있습니다");
                            thePig.Run(_hit.transform.position); // 타겟의 위치 넘겨서 그 반대 방향으로 달리기
                        }
                    }
                }
            }
        }
    }
}
