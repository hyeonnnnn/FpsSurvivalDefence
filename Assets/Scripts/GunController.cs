using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // 활성화 여부
    public static bool isActivate = false; // 초기에 비활성화

    // 현재 장착된 총
    [SerializeField]
    private Gun currentGun;

    // 연사 속도 계산
    private float currentFireRate;

    //상태 변수
    private bool isReload = false;
    [HideInInspector]
    public bool isFineSightMode = false;

    // 본래 포지션 값
    private Vector3 originPos;

    // 효과음 재생
    private AudioSource audioSource;

    // 레이저 충돌 정보 가져옴
    private RaycastHit hitinfo;

    // 필요한 컴포넌트
    [SerializeField]
    private Camera theCam;
    private Crosshair theCrosshair;

    // 피격 이펙트
    [SerializeField]
    private GameObject hit_effect_prefab;

    void Start()
    {
        originPos = Vector3.zero;
        audioSource = GetComponent<AudioSource>();
        theCrosshair = FindObjectOfType<Crosshair>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivate)
        {
            GunFireRateCalc();
            TryFire();
            TryReload();
            TryFineSight();
        }
    }

    // 연사 속도 재계산
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; // 1/60을 60번 실행 -> 1초에 1씩 감소
    }

    // 발사 시도
    private void TryFire()
    {
        if(Input.GetButton("Fire1") && (currentFireRate <= 0) && isReload == false)
        {
            Fire();
        }
    }

    // 발사 전 계산
    private void Fire()
    {
        if (!isReload)
        {
            if (currentGun.currentBulletCount > 0)
            {
                shoot(); // 발사
            }
            else // 발사할 총알이 없으면
            {
                CancelFineSight();
                StartCoroutine(ReloadCoroutine()); // 재장전

            }
        }
    }

    // 발사 후 계산
    private void shoot()
    {
        theCrosshair.FireAnimation();
        currentGun.currentBulletCount--; // 총알 -1
        currentFireRate = currentGun.fireRate; // 연사 속도 재계산
        PlaySE(currentGun.fire_Sound); // 사운드
        currentGun.muzzleFlash.Play(); // 파티클
        Hit(); // 발사 족족 맞추기
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine()); // 총기 반동 코루틴
    }

    // 전방에 레이저
    private void Hit()
    {
        // 카메라의 현재 위치에서 전방으로 레이저를 쏘는데, 이때 랜덤값이 들어감
        // x축과 y축 각각에 랜덤값 (최솟값, 최댓값) 할당
        // 사정 거리 충돌하는 물체가 있다면 hitinfo에 저장하고 true 반환
        // accuracy가 높을수록 정확도는 줄어듦
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward + 
            new Vector3(Random.Range(-theCrosshair.GetAccuarcy() - currentGun.accuracy, theCrosshair.GetAccuarcy() - currentGun.accuracy),
                        Random.Range(-theCrosshair.GetAccuarcy() - currentGun.accuracy, theCrosshair.GetAccuarcy() - currentGun.accuracy),
                        0) // 나아가는 방향은 항상 1
            , out hitinfo, currentGun.range))
        {
            GameObject clone = Instantiate(hit_effect_prefab, hitinfo.point, Quaternion.LookRotation(hitinfo.normal)); // 충돌한 객체의 표면이 바라보는 방향으로 생성
            Destroy(clone, 2f); // 2초 후에 삭제
        }
    }

    // 재장전 시도
    private void TryReload()
    {
        // R을 누르고, 재장전 중이 아니고, 탄알집에 공간이 남아 있을 때
        if(Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancelFineSight(); // 정조준을 해제하면서
            StartCoroutine(ReloadCoroutine()); // 재장전
        }
    }

    // 재장전
    IEnumerator ReloadCoroutine()
    {
        if(currentGun.carryBulletCount > 0)
        {
            isReload = true;

            currentGun.anim.SetTrigger("Reload");

            currentGun.carryBulletCount += currentGun.currentBulletCount; // 총알을 버리지 X
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime); // 재장전 시간 동안 대기


            if(currentGun.carryBulletCount >= currentGun.reloadBulletCount) // 소유한 총알이 장전할 수 있는 총알의 최대 개수보다 많을 때
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount; // 풀로 장전하고
                currentGun.carryBulletCount -= currentGun.reloadBulletCount; // 장전한 만큼 빠짐
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;

            }

            isReload = false;
        }

        else
        {
            Debug.Log("총알 부족");
        }
    }

    public void CancelReload()
    {
        if (isReload)
        {
            StopAllCoroutines();
            isReload = false;
        }
    }

    // 정조준 시도
    private void TryFineSight()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            FineSight();
        }
    }

    // 정조준 취소
    public void CancelFineSight() // PlayerController 클래스에서 호출될 것
    {
        if (isFineSightMode)
        {
            FineSight();
        }
    }

    // 정조준 로직 가동
    private void FineSight()
    {
        isFineSightMode = !isFineSightMode; // FineSight가 실행될 때마다 스위치
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        theCrosshair.FineSightAnimation(isFineSightMode);

        if (isFineSightMode)
        {
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());
        }
    }

    // 정조준 활성화
    IEnumerator FineSightActivateCoroutine()
    {
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos) // 총의 위치가 정조준 위치가 될 때까지
        {
            // 목표 지점까지 점진적으로 이동
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return 0;
        }
    }

    // 정조준 비활성화
    IEnumerator FineSightDeactivateCoroutine()
    {
        while (currentGun.transform.localPosition != originPos) // 총의 위치가 원래 위치가 될 때까지
        {
            // 원래 지점까지 점진적으로 이동
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return 0;
        }
    }

    // 반동 코루틴
    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoilBack = new Vector3(currentGun.retroActionFineSightForce, originPos.y, originPos.z); // 정조준 아닐 시의 최대 반동
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z) ; // 정조준 시의 최대 반동

        // 정조준이 아닐 때의 반동
        if(!isFineSightMode)
        {
            currentGun.transform.localPosition = originPos; // 두 번째 반동도 원래 위치에서 시작해야 함

            // 반동 시작
            while(currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f) // 0.02f: 여유값
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null; // 대기. 매 프레임마다 반복
            }

            //원 위치
            while(currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null; // 대기. 매 프레임마다 반복
            }
        }

        // 정조준일 때의 반동
        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            // 반동 시작
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null; // 대기. 매 프레임마다 반복
            }

            //원 위치
            while (currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null; // 대기. 매 프레임마다 반복
            }
        }
    }

    // 사운드 재생
    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public Gun GetGun()
    {
        return currentGun; // 함수를 이용하여 Gun 클래스의 currentGun 가져오기
    }

    public bool GetFineSightMode()
    {
        return isFineSightMode;
    }

    // 무기 교체
    public void GunChange(Gun _gun)
    {
        if(WeaponManager.currentWeapon != null) // 무언가를 들고 있는 경우
            WeaponManager.currentWeapon.gameObject.SetActive(false); // 비활성화

        currentGun = _gun; // 바꿀 무기가 현재 무기로
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim; // 각 컨트롤러마다 설정할 필요X

        currentGun.transform.localPosition = Vector3.zero; // 총을 정조준하고 있다면 위치가 바뀔 가능성O, 이를 방지
        currentGun.gameObject.SetActive(true); // 활성화
        isActivate = true;
    }
}
