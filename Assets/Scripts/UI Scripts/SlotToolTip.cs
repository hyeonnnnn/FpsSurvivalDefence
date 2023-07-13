using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SlotToolTip : MonoBehaviour
{
    [SerializeField]
    private GameObject go_Base; // 툴팁

    [SerializeField]
    private TMP_Text txt_itemName;
    [SerializeField]
    private TMP_Text txt_itemDesc;
    [SerializeField]
    private TMP_Text txt_itemHowtoUsed;

    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        go_Base.SetActive(true); // 툴팁 활성화
        _pos += new Vector3(go_Base.GetComponent<RectTransform>().rect.width * 0.5f, -go_Base.GetComponent<RectTransform>().rect.height, 0f); // 위치 옮기기
        go_Base.transform.position = _pos;

        txt_itemName.text = _item.itemName;
        txt_itemDesc.text = _item.itemDesc;

        if (_item.itemType == Item.ItemType.Equipment)
            txt_itemHowtoUsed.text = "Right-click to install";
        else if (_item.itemType == Item.ItemType.Equipment)
            txt_itemHowtoUsed.text = "Right-click to eat";
        else
            txt_itemHowtoUsed.text = "";
    }

    public void HideToolTip()
    {
        go_Base.SetActive(false); // 툴팁 비활성화
    }
}
