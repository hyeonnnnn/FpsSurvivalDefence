using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    // �ʿ��� ������Ʈ
    [SerializeField]
    private GunController theGunController;
    private Gun currentGun;

    // �ʿ��ϸ� HUD Ȱ��ȭ, �ʿ� ������ ��Ȱ��ȭ
    [SerializeField]
    private GameObject go_BulletHUD;

    // �Ѿ� ���� �ݿ�
    [SerializeField]
    private TMP_Text[] text_Bullet;

    // Update is called once per frame
    void Update()
    {
        CheckBullet();
    }

    private void CheckBullet()
    {
        currentGun = theGunController.GetGun();
        text_Bullet[0].text = currentGun.carryBulletCount.ToString(); // int -> string
        text_Bullet[1].text = currentGun.reloadBulletCount.ToString();
        text_Bullet[2].text = currentGun.currentBulletCount.ToString();
    }
}
