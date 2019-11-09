using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using DG.Tweening;
using TMP = TMPro.TextMeshProUGUI;
using UnityEngine.SceneManagement;


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
    public CanvasGroup detailsPane;
    public CanvasGroup fadeDialog;
    public TMP fadeDialogTMP;
    public TMP wallet;

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

        foreach (BaseItem bi in carRoot.Property1)
        {
            tmp = Instantiate(shopItemButton, contentHolder);
            tmpBtn = tmp.GetComponent<StockButton>();
            tmpBtn.itemNameLbl.text = bi.itemname;
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
        if (detailsPane.alpha == 0)
            detailsPane.DOFade(1, .3f);

        wallet.text = $"Balance: {GamePlay.coin}";
        mainDetails[0].text = Bi.itemname;
        mainDetails[1].text = $"{Bi.desc}\nPurchase price: {Bi.purchasecost.ToString()}\nRunning costs: {Bi.maintcost.ToString()}";
        CurrentItem = Bi;
    }
    BaseItem CurrentItem;
    public void CloseDetails()
    {
        detailsPane.DOFade(0, .3f);
    }

    public void BuyItem()
    {
        if (GamePlay.inventory == null)
            GamePlay.inventory = new List<BaseItem>();
        if (GamePlay.coin > CurrentItem.purchasecost)
        {
            //TODO do the thing to say well done you brought soemthign.
            GamePlay.inventory.Add(CurrentItem);
            BuildTile bt = new BuildTile();
            bt.buildPrefab = Resources.Load(CurrentItem.prefab) as GameObject;
            DesignController.instance.AddBuildTile(bt);
            
            GamePlay.coin -= CurrentItem.purchasecost;
            wallet.text = $"Currante Balance:{GamePlay.coin.ToString()}";
        }
        else
        {
            //TODO do a different thing to say not enough cash.\
            Sequence dialogBounceFade = DOTween.Sequence();
            dialogBounceFade.Append(fadeDialog.DOFade(1, .3f));
            dialogBounceFade.AppendInterval(1);
            dialogBounceFade.Append(fadeDialog.DOFade(0, .3f));
            dialogBounceFade.PlayForward();  
        }
    }
    public void LoadScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }
}