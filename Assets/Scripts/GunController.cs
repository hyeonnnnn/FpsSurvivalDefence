using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // Ȱ��ȭ ����
    public static bool isActivate = false; // �ʱ⿡ ��Ȱ��ȭ

    // ���� ������ ��
    [SerializeField]
    private Gun currentGun;

    // ���� �ӵ� ���
    private float currentFireRate;

    //���� ����
    private bool isReload = false;
    [HideInInspector]
    public bool isFineSightMode = false;

    // ���� ������ ��
    private Vector3 originPos;

    // ȿ���� ���
    private AudioSource audioSource;

    // ������ �浹 ���� ������
    private RaycastHit hitinfo;

    // �ʿ��� ������Ʈ
    [SerializeField]
    private Camera theCam;
    private Crosshair theCrosshair;

    // �ǰ� ����Ʈ
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

    // ���� �ӵ� ����
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; // 1/60�� 60�� ���� -> 1�ʿ� 1�� ����
    }

    // �߻� �õ�
    private void TryFire()
    {
        if(Input.GetButton("Fire1") && (currentFireRate <= 0) && isReload == false)
        {
            Fire();
        }
    }

    // �߻� �� ���
    private void Fire()
    {
        if (!isReload)
        {
            if (currentGun.currentBulletCount > 0)
            {
                shoot(); // �߻�
            }
            else // �߻��� �Ѿ��� ������
            {
                CancelFineSight();
                StartCoroutine(ReloadCoroutine()); // ������

            }
        }
    }

    // �߻� �� ���
    private void shoot()
    {
        theCrosshair.FireAnimation();
        currentGun.currentBulletCount--; // �Ѿ� -1
        currentFireRate = currentGun.fireRate; // ���� �ӵ� ����
        PlaySE(currentGun.fire_Sound); // ����
        currentGun.muzzleFlash.Play(); // ��ƼŬ
        Hit(); // �߻� ���� ���߱�
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine()); // �ѱ� �ݵ� �ڷ�ƾ
    }

    // ���濡 ������
    private void Hit()
    {
        // ī�޶��� ���� ��ġ���� �������� �������� ��µ�, �̶� �������� ��
        // x��� y�� ������ ������ (�ּڰ�, �ִ�) �Ҵ�
        // ���� �Ÿ� �浹�ϴ� ��ü�� �ִٸ� hitinfo�� �����ϰ� true ��ȯ
        // accuracy�� �������� ��Ȯ���� �پ��
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward + 
            new Vector3(Random.Range(-theCrosshair.GetAccuarcy() - currentGun.accuracy, theCrosshair.GetAccuarcy() - currentGun.accuracy),
                        Random.Range(-theCrosshair.GetAccuarcy() - currentGun.accuracy, theCrosshair.GetAccuarcy() - currentGun.accuracy),
                        0) // ���ư��� ������ �׻� 1
            , out hitinfo, currentGun.range))
        {
            GameObject clone = Instantiate(hit_effect_prefab, hitinfo.point, Quaternion.LookRotation(hitinfo.normal)); // �浹�� ��ü�� ǥ���� �ٶ󺸴� �������� ����
            Destroy(clone, 2f); // 2�� �Ŀ� ����
        }
    }

    // ������ �õ�
    private void TryReload()
    {
        // R�� ������, ������ ���� �ƴϰ�, ź������ ������ ���� ���� ��
        if(Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancelFineSight(); // �������� �����ϸ鼭
            StartCoroutine(ReloadCoroutine()); // ������
        }
    }

    // ������
    IEnumerator ReloadCoroutine()
    {
        if(currentGun.carryBulletCount > 0)
        {
            isReload = true;

            currentGun.anim.SetTrigger("Reload");

            currentGun.carryBulletCount += currentGun.currentBulletCount; // �Ѿ��� ������ X
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime); // ������ �ð� ���� ���


            if(currentGun.carryBulletCount >= currentGun.reloadBulletCount) // ������ �Ѿ��� ������ �� �ִ� �Ѿ��� �ִ� �������� ���� ��
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount; // Ǯ�� �����ϰ�
                currentGun.carryBulletCount -= currentGun.reloadBulletCount; // ������ ��ŭ ����
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
            Debug.Log("�Ѿ� ����");
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

    // ������ �õ�
    private void TryFineSight()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            FineSight();
        }
    }

    // ������ ���
    public void CancelFineSight() // PlayerController Ŭ�������� ȣ��� ��
    {
        if (isFineSightMode)
        {
            FineSight();
        }
    }

    // ������ ���� ����
    private void FineSight()
    {
        isFineSightMode = !isFineSightMode; // FineSight�� ����� ������ ����ġ
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

    // ������ Ȱ��ȭ
    IEnumerator FineSightActivateCoroutine()
    {
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos) // ���� ��ġ�� ������ ��ġ�� �� ������
        {
            // ��ǥ �������� ���������� �̵�
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return 0;
        }
    }

    // ������ ��Ȱ��ȭ
    IEnumerator FineSightDeactivateCoroutine()
    {
        while (currentGun.transform.localPosition != originPos) // ���� ��ġ�� ���� ��ġ�� �� ������
        {
            // ���� �������� ���������� �̵�
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return 0;
        }
    }

    // �ݵ� �ڷ�ƾ
    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoilBack = new Vector3(currentGun.retroActionFineSightForce, originPos.y, originPos.z); // ������ �ƴ� ���� �ִ� �ݵ�
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z) ; // ������ ���� �ִ� �ݵ�

        // �������� �ƴ� ���� �ݵ�
        if(!isFineSightMode)
        {
            currentGun.transform.localPosition = originPos; // �� ��° �ݵ��� ���� ��ġ���� �����ؾ� ��

            // �ݵ� ����
            while(currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f) // 0.02f: ������
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null; // ���. �� �����Ӹ��� �ݺ�
            }

            //�� ��ġ
            while(currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null; // ���. �� �����Ӹ��� �ݺ�
            }
        }

        // �������� ���� �ݵ�
        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            // �ݵ� ����
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null; // ���. �� �����Ӹ��� �ݺ�
            }

            //�� ��ġ
            while (currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null; // ���. �� �����Ӹ��� �ݺ�
            }
        }
    }

    // ���� ���
    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public Gun GetGun()
    {
        return currentGun; // �Լ��� �̿��Ͽ� Gun Ŭ������ currentGun ��������
    }

    public bool GetFineSightMode()
    {
        return isFineSightMode;
    }

    // ���� ��ü
    public void GunChange(Gun _gun)
    {
        if(WeaponManager.currentWeapon != null) // ���𰡸� ��� �ִ� ���
            WeaponManager.currentWeapon.gameObject.SetActive(false); // ��Ȱ��ȭ

        currentGun = _gun; // �ٲ� ���Ⱑ ���� �����
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim; // �� ��Ʈ�ѷ����� ������ �ʿ�X

        currentGun.transform.localPosition = Vector3.zero; // ���� �������ϰ� �ִٸ� ��ġ�� �ٲ� ���ɼ�O, �̸� ����
        currentGun.gameObject.SetActive(true); // Ȱ��ȭ
        isActivate = true;
    }
}
