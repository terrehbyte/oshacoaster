using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMP = TMPro.TextMeshProUGUI;
using UnityEngine.EventSystems;

public  class StockButton: MonoBehaviour, IPointerClickHandler
{

    public BaseItem OriginalObject;
    public string myType;

    public TMP itemNameLbl;
    public Image itemThumb;




    public void OnPointerClick(PointerEventData eventData)
    {
        //TODO: show the details panel for whichever kind of object this is.
        //TODO: provide purchase mechanism for item.
        StockList.instance.ShowDetails(OriginalObject);
    }
}