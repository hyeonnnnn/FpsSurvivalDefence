using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item; // ȹ���� ������
    public int itemCount; // ȹ���� �������� ����
    public Image itemImage; // �������� �̹���

    // �ʿ��� ������Ʈ
    [SerializeField]
    private TMP_Text text_Count;
    [SerializeField]
    private GameObject go_Count_Image;

    private ItemEffectDatabase theItemEffectDatabase;
    // private Rect baseRect;
    // private InputNumber theInputNumber;

    void Start()
    {
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
        // baseRect = transform.parent.parent.GetComponent<RectTransform>().rect;
        // theInputNumber = FindObjectOfType<InputNumber>();
    }

    // �̹����� ���� ����
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color; // 0�̸� ����, 1�̸� ���̵���

    }

    // ������ ȹ��
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        // ��� �ƴ� ���
        if(item.itemType != Item.ItemType.Equipment)
        {
            text_Count.text = itemCount.ToString();
            go_Count_Image.SetActive(true);
        }
        // ����� ���
        else
        {
            text_Count.text = "0";
            go_Count_Image.SetActive(false);
        }
        SetColor(1);
    }

    // ������ ���� ����
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // ���� �ʱ�ȭ
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        go_Count_Image.SetActive(false);
        text_Count.text = "0";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // �� ��ũ��Ʈ�� ����� ��ü�� ��Ŭ���� 
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(item != null)
            {
                theItemEffectDatabase.UseItem(item);

                if (item.itemType == Item.ItemType.Used)
                    SetSlotCount(-1);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage); // �̹��� �ٲٱ�

            DragSlot.instance.transform.position = eventData.position; // �巡�� �� ������ ���콺�� ��ġ�� 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position; // ������ ��� ���콺�� ��ġ�� 
        }
    }

    // OnEndDrag: �巡�װ� �����⸸ �ϸ�
    // OnDrop: �ٸ� ���� ������ �巡�װ� ���� ��� (ChangeSlot())

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0); // �� ���̵���
        DragSlot.instance.dragSlot = null; // �巡�׸� ������ ���� ��ġ��
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(DragSlot.instance.dragSlot != null)
            ChangeSlot();
    }

    private void ChangeSlot()
    {
        // B�� �ڱ��ڽ� ����
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        // B�ڸ��� A
        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        // A�ڸ��� B
        if (_tempItem != null)
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        else
            DragSlot.instance.dragSlot.ClearSlot();
    }

    // ���콺�� ���Կ� �� �� �ߵ�
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
            theItemEffectDatabase.ShowToolTip(item, transform.position); // ���� ���̱�
    }

    // ���콺�� ���Կ��� ���� �� �ߵ�
    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToolTip(); // ���� �����
    }
}
