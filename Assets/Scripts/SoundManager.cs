using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound // MonoBehaviour�� ��� ��ü�� ������Ʈ�� �߰��� �� ����
{
    public string name; // ���� �̸�
    public AudioClip clip; // ��
}

public class SoundManager : MonoBehaviour
{
    // Awake(): ��ü ���� �� ���� ����
    // OnEnable(): Ȱ��ȭ�� ������ ���� (�ڷ�ƾX)
    // Start(): Ȱ��ȭ�� ������ ���� (�ڷ�ƾO)

    static public SoundManager instance;
    #region singleton
    void Awake() // ��ü ������ ���� ����
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �ٸ� ������ �Ѿ �� �ı�X
        }

        else
            Destroy(gameObject);
    }
    #endregion singleton

    public AudioSource[] audioSourceEffects;
    public AudioSource audioSourceBgm;

    public string[] playSoundName;

    public Sound[] effectSounds;
    public Sound[] bgmSounds;

    void Start()
    {
        // ����� �ҽ���ŭ �ڵ����� ������ ������
        playSoundName = new string[audioSourceEffects.Length];
    }

    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if (_name == effectSounds[i].name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("��� ���� AudioSource�� ������Դϴ�.");
                return;
            }
        }
        Debug.Log(_name + "���尡 SoundManager�� ��ϵ��� �ʾҽ��ϴ�.");
    }

    // ȿ���� ��� ���
    public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (playSoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                return;
            }
        }
        Debug.Log("��� ����" + _name + "���尡 �����ϴ�.");
    }

}
