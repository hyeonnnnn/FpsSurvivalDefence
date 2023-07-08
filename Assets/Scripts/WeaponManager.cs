using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(GunController))]
public class WeaponManager : MonoBehaviour
{
    // ���� �ߺ� ��ü ���� ����
    public static bool isChangeWeapon = false; // static ������ ���� �ڿ�. Ŭ���� ���� == ���� ����
                                               // ���� �� �޸� ����
    // ���� ����� �װ��� �ִϸ��̼�
    public static Transform currentWeapon; // ���⸦ ���ٰ� �Ѵ� ���Ҹ� ����
    public static Animator currentWeaponAnim;

    // ���� ������ Ÿ��
    [SerializeField]
    private string currentWeaponType;

    // ���� ��ü ������, ���� ��ü�� ������ ���� ����
    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;

    // ���� ���� ����
    [SerializeField]
    private Gun[] guns;
    [SerializeField]
    private CloseWeapon[] hands;
    [SerializeField]
    private CloseWeapon[] axes;

    // ���� �������� ���⿡ ���� ������ �� �ֵ���
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, CloseWeapon> handDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> axeDictionary = new Dictionary<string, CloseWeapon>();

    // �ʿ��� ������Ʈ
    [SerializeField]
    private GunController theGunController;
    [SerializeField]
    private HandController theHandController;
    [SerializeField]
    private AxeController theAxeController;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]);
        }
        for (int i = 0; i < hands.Length; i++)
        {
            handDictionary.Add(hands[i].closeWeaponName, hands[i]);
        }
        for (int i = 0; i < axes.Length; i++)
        {
            axeDictionary.Add(axes[i].closeWeaponName, axes[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // �ѹ�Ű�� ���� ��ü
        if (!isChangeWeapon) // ��ü ���� �ƴ� ����
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                StartCoroutine(ChangeWeaponcoroutine("HAND", "�Ǽ�")); // ���� ��ü ���� (�Ǽ�)
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                StartCoroutine(ChangeWeaponcoroutine("GUN", "SubMachineGun1")); // (����ӽŰ�1) 
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                StartCoroutine(ChangeWeaponcoroutine("AXE", "Axe")); // (����)
        }
    }

    public IEnumerator ChangeWeaponcoroutine(string _type, string _name) // �����Կ��� ���, _type, _name: �ٲٰ��� �ϴ� ��
    {
        isChangeWeapon = true; // ���� ��ü�� �ߺ��� ���� �ʵ���
        currentWeaponAnim.SetTrigger("Weapon_Out"); // ���̳� �� ����ֱ�

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction(); // ������, ������ ���� ����
        WeaponChange(_type, _name); // ���� �ٲٱ�

        yield return new WaitForSeconds(changeWeaponEndDelayTime); // ������ �ִϸ��̼Ǳ��� ���� �Ŀ�

        currentWeaponType = _type; // ���� �ǿ� �־��ֱ�
        isChangeWeapon = false;
    }

    // ������, ������ ���� ����
    private void CancelPreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "GUN":
                theGunController.CancelFineSight(); // ������ ���� ����
                theGunController.CancelReload(); // ������ ���� ����
                GunController.isActivate = false;
                break;
            case "HAND":
                HandController.isActivate = false;
                break;
            case "AXE":
                AxeController.isActivate = false;
                break;
        }
    }

    // ���� ��ü �Լ�
    private void WeaponChange(string _type, string _name)
    {
        if (_type == "GUN")
            theGunController.GunChange(gunDictionary[_name]); // guns[0]�� ���� �������� �ĺ��ϱ� �����
        else if (_type == "HAND")
            theHandController.CloseWeaponChange(handDictionary[_name]);
        else if (_type == "AXE")
            theAxeController.CloseWeaponChange(axeDictionary[_name]);
    }
}
