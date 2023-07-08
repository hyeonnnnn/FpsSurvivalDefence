using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(GunController))]
public class WeaponManager : MonoBehaviour
{
    // 무기 중복 교체 실행 방지
    public static bool isChangeWeapon = false; // static 변수는 공유 자원. 클래스 변수 == 정적 변수
                                               // 남발 시 메모리 낭비
    // 현재 무기와 그것의 애니메이션
    public static Transform currentWeapon; // 무기를 껐다가 켜는 역할만 수행
    public static Animator currentWeaponAnim;

    // 현재 무기의 타입
    [SerializeField]
    private string currentWeaponType;

    // 무기 교체 딜레이, 무기 교체가 완전히 끝난 시점
    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;

    // 무기 종류 관리
    [SerializeField]
    private Gun[] guns;
    [SerializeField]
    private CloseWeapon[] hands;
    [SerializeField]
    private CloseWeapon[] axes;

    // 관리 차원에서 무기에 쉽게 접근할 수 있도록
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, CloseWeapon> handDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> axeDictionary = new Dictionary<string, CloseWeapon>();

    // 필요한 컴포넌트
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
        // 넘버키로 무기 교체
        if (!isChangeWeapon) // 교체 중이 아닐 때만
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                StartCoroutine(ChangeWeaponcoroutine("HAND", "맨손")); // 무기 교체 실행 (맨손)
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                StartCoroutine(ChangeWeaponcoroutine("GUN", "SubMachineGun1")); // (서브머신건1) 
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                StartCoroutine(ChangeWeaponcoroutine("AXE", "Axe")); // (도끼)
        }
    }

    public IEnumerator ChangeWeaponcoroutine(string _type, string _name) // 퀵슬롯에서 사용, _type, _name: 바꾸고자 하는 것
    {
        isChangeWeapon = true; // 무기 교체가 중복이 되지 않도록
        currentWeaponAnim.SetTrigger("Weapon_Out"); // 손이나 총 집어넣기

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction(); // 정조준, 재장전 상태 해제
        WeaponChange(_type, _name); // 무기 바꾸기

        yield return new WaitForSeconds(changeWeaponEndDelayTime); // 꺼내는 애니메이션까지 끝난 후에

        currentWeaponType = _type; // 현재 탭에 넣어주기
        isChangeWeapon = false;
    }

    // 정조준, 재장전 상태 해제
    private void CancelPreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "GUN":
                theGunController.CancelFineSight(); // 정조준 상태 해제
                theGunController.CancelReload(); // 재장전 상태 해제
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

    // 무기 교체 함수
    private void WeaponChange(string _type, string _name)
    {
        if (_type == "GUN")
            theGunController.GunChange(gunDictionary[_name]); // guns[0]은 무슨 무기인지 식별하기 어려움
        else if (_type == "HAND")
            theHandController.CloseWeaponChange(handDictionary[_name]);
        else if (_type == "AXE")
            theAxeController.CloseWeaponChange(axeDictionary[_name]);
    }
}
