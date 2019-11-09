using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using DG.Tweening;
using TMP = TMPro.TextMeshProUGUI;


//TODO: Redesign stock items to derive from a base class, with two (or more) subclasses for various types.
//These should be desplayed with the main base variables displayed in  a header, and the sub variables in a custom
//view for each type??
//we could do with a UI designer on this but thems ya thing.

public class StockList : MonoBehaviour
{
    public static StockList instance;
    public Sprite[] thumbs;
    public GameObject shopItemButton;
    public TextAsset carInventory;
    public TextAsset trackInventory;

    public CarRoot carRoot = new CarRoot();
    public TrackRoot trackRoot = new TrackRoot();
    public Transform contentHolder;
    public TMP[] mainDetails;
    public CanvasGroup DetailsPane;
    
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);

        if (instance == null)
            instance = this;
        GameObject tmp;
        StockButton tmpBtn;
        carRoot.Property1 = JsonConvert.DeserializeObject<Car[]>(carInventory.ToString());
        trackRoot.Property1 = JsonConvert.DeserializeObject<Track[]>(trackInventory.ToString());

        foreach(BaseItem bi in carRoot.Property1)
        {
            tmp = Instantiate(shopItemButton, contentHolder);
            tmpBtn = tmp.GetComponent<StockButton>();
            tmpBtn.itemNameLbl .text = bi.itemname;
            tmpBtn.itemThumb.sprite = thumbs.FirstOrDefault<Sprite>(x => x.name == bi.prefab);
            tmpBtn.OriginalObject = bi;
        }
        foreach (BaseItem bi in trackRoot.Property1)
        {
            tmp = Instantiate(shopItemButton, contentHolder);
            tmpBtn = tmp.GetComponent<StockButton>();
            tmpBtn.itemNameLbl.text = bi.itemname;
            tmpBtn.itemThumb.sprite = thumbs.FirstOrDefault<Sprite>(x => x.name == bi.prefab);
            tmpBtn.OriginalObject = bi;
        }
    }

    public void ShowDetails(BaseItem Bi)
    {
        if (DetailsPane.alpha == 0)
            DetailsPane.DOFade(1, .3f);
            
        mainDetails[0].text = Bi.itemname;
        mainDetails[1].text = $"{Bi.desc}\nPurchase price: {Bi.purchasecost.ToString()}\nRunning costs: {Bi.maintcost.ToString()}";

    }

    public void CloseDetails()
    {
        DetailsPane.DOFade(0, .3f);
    }



}
