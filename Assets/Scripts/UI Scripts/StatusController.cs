using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // ü��
    [SerializeField]
    private int hp;
    private int currentHp;

    // ���׹̳�
    [SerializeField]
    private int sp;
    private int currentSp;

    //���׹̳� ������
    [SerializeField]
    private int spIncreaseSpeed;

    // ���׹̳� ��ȸ�� ������
    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    // ���׹̳� ���� ����
    private bool spUsed;

    // ����
    [SerializeField]
    private int dp;
    private int currentDp;

    // �����
    [SerializeField]
    private int hungry;
    private int currentHungry;

    // ����� ���� �ӵ�
    [SerializeField]
    private int hungryDecreaseTime;
    private int currentHungryDecreaseTime;

    // �񸶸�
    [SerializeField]
    private int thirsty;
    private int currentThirsty;

    // �񸶸� ���� �ӵ�
    [SerializeField]
    private int thirstyDecreaseTime;
    private int currentThirstyDecreaseTime;

    // ������
    [SerializeField]
    private int satisfy;
    private int currentSatisfy;

    // �ʿ��� �̹���
    [SerializeField]
    private Image[] images_Gauge;

    private const int HP = 0, DP = 1, SP = 2, HUNGRY = 3, THIRSTY = 4, SATISFY = 5;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = hp;
        currentDp = dp;
        currentSp = sp;
        currentHungry = hungry;
        currentThirsty = thirsty;
        currentSatisfy = satisfy;
    }

    // HP ȸ��
    public void IncreaseHP(int _count)
    {
        if (currentHp + _count < hp)
            currentHp += _count;
        else
            currentHp = hp;
    }

    // HP ����
    public void DecreaseHP(int _count)
    {
        if (currentDp > 0) // ������ ������
        {
            DecreaseDP(_count); // ������ ����
            return;
        }

        currentHp -= _count;

        if(currentHp <= _count)
        {
            Debug.Log("ü��: 0");
        }
    }

    // DP ȸ��
    public void IncreaseDP(int _count)
    {
        if (currentDp + _count < hp)
            currentDp += _count;
        else
            currentDp = dp;
    }

    // DP ����
    public void DecreaseDP(int _count)
    {
        currentDp -= _count;

        if (currentDp <= _count)
        {
            Debug.Log("����: 0");
        }
    }

    // Hungry ȸ��
    public void IncreaseHungry(int _count)
    {
        if (currentHungry + _count < hp)
            currentHungry += _count;
        else
            currentHungry = hungry;
    }

    // Hungry ����
    public void DecreaseHungry(int _count)
    {
        if (currentHungry - _count < 0)
            currentHungry = 0;
        else
            currentHungry -= _count;
    }

    // Thirsty ȸ��
    public void IncreaseThirsty(int _count)
    {
        if (currentThirsty + _count < hp)
            currentThirsty += _count;
        else
            currentThirsty = dp;
    }

    // Thirsty ����
    public void DecreaseThirsty(int _count)
    {
        if (currentThirsty - _count < 0)
            currentThirsty = 0;
        else
            thirsty -= _count;
    }

    // Update is called once per frame
    void Update()
    {
        Hungry();
        Thirsty();
        SPRechargeTime();
        SPRecover();
        GuageUpdate();
    }

    // ���׹̳� ���� �ð� ���
    private void SPRechargeTime()
    {
        if (spUsed) // sp�� ������ ��
        {
            if (currentSpRechargeTime < spRechargeTime)
                currentSpRechargeTime++;
            else
                spUsed = false;
        }
    }

    // ���׹̳� ����
    private void SPRecover()
    {
        if (!spUsed && (currentSp < sp))
        {
            currentSp += spIncreaseSpeed;
        }
    }

    private void Hungry()
    {
        if (currentHungry > 0)
        {
            if (currentHungryDecreaseTime <= hungryDecreaseTime)
                currentHungryDecreaseTime++;
            else // hungryDecreaseTime�� ������ currentHungry�� 1 ����
            {
                currentHungry--;
                currentHungryDecreaseTime = 0;
            }
        }
        else
            Debug.Log("���");
    }

    private void Thirsty()
    {
        if (currentThirsty > 0)
        {
            if (currentThirstyDecreaseTime <= thirstyDecreaseTime)
                currentThirstyDecreaseTime++;
            else
            {
                currentThirsty--;
                currentThirstyDecreaseTime = 0;
            }
        }
        else
            Debug.Log("Ż��");
    }

    private void GuageUpdate()
    {
        images_Gauge[HP].fillAmount = (float)currentHp / hp;
        images_Gauge[SP].fillAmount = (float)currentSp / sp;
        images_Gauge[DP].fillAmount = (float)currentDp / dp;
        images_Gauge[HUNGRY].fillAmount = (float)currentHungry / hungry;
        images_Gauge[THIRSTY].fillAmount = (float)currentThirsty / thirsty;
        images_Gauge[SATISFY].fillAmount = (float)currentSatisfy / satisfy;
    }

    // ���׹̳� ����
    public void DecreaseStamina(int _count)
    {
        spUsed = true;
        currentSpRechargeTime = 0;

        if (currentSp - _count > 0)
            currentSp -= _count;
        else
            currentSp = 0;
    }

    public int GetCurrentSP()
    {
        return currentSp;
    }
}
