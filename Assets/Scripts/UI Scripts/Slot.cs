using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item; // 획득한 아이템
    public int itemCount; // 획득한 아이템의 개수
    public Image itemImage; // 아이템의 이미지

    // 필요한 컴포넌트
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

    // 이미지의 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color; // 0이면 투명, 1이면 보이도록

    }

    // 아이템 획득
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        // 장비가 아닐 경우
        if(item.itemType != Item.ItemType.Equipment)
        {
            text_Count.text = itemCount.ToString();
            go_Count_Image.SetActive(true);
        }
        // 장비일 경우
        else
        {
            text_Count.text = "0";
            go_Count_Image.SetActive(false);
        }
        SetColor(1);
    }

    // 아이템 개수 조정
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // 슬롯 초기화
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
        // 이 스크립트가 적용된 객체를 우클릭시 
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
            DragSlot.instance.DragSetImage(itemImage); // 이미지 바꾸기

            DragSlot.instance.transform.position = eventData.position; // 드래그 시 슬롯이 마우스의 위치로 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position; // 슬롯이 계속 마우스의 위치로 
        }
    }

    // OnEndDrag: 드래그가 끝나기만 하면
    // OnDrop: 다른 슬롯 위에서 드래그가 끝날 경우 (ChangeSlot())

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0); // 안 보이도록
        DragSlot.instance.dragSlot = null; // 드래그를 놓으면 원래 위치로
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(DragSlot.instance.dragSlot != null)
            ChangeSlot();
    }

    private void ChangeSlot()
    {
        // B가 자기자신 복사
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        // B자리에 A
        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        // A자리에 B
        if (_tempItem != null)
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        else
            DragSlot.instance.dragSlot.ClearSlot();
    }

    // 마우스가 슬롯에 들어갈 때 발동
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
            theItemEffectDatabase.ShowToolTip(item, transform.position); // 툴팁 보이기
    }

    // 마우스가 슬롯에서 나올 때 발동
    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToolTip(); // 툴팁 숨기기
    }
}
