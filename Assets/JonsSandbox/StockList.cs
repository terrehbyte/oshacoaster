﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using DG.Tweening;
using TMP = TMPro.TextMeshProUGUI;
using UnityEngine.SceneManagement;
using System;
using System.IO;

//TODO: Redesign stock items to derive from a base class, with two (or more) subclasses for various types.
//These should be desplayed with the main base variables displayed in  a header, and the sub variables in a custom
//view for each type??
//we could do with a UI designer on this but thems ya thing.

public class StockList : MonoBehaviour
{
    public static StockList instance;
    public Sprite[] thumbs;

    internal void UpdateDirections(string str)
    {
        CurrentItem.connections = str;
    }
#if UNITY_EDITOR
    internal void Save()
    {
      
        String TracksToSave = JsonConvert.SerializeObject(trackRoot.Property1);
        Debug.Log(TracksToSave);
        string fPath = UnityEditor.AssetDatabase.GetAssetPath(trackInventory);
        using (StreamWriter writer = File.CreateText(fPath))
        {
            writer.Write(TracksToSave);
        }


    }
#endif

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

    public GameObject PreviewParent;
    public GameObject PreviewTemp;
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
            bi.AssetType = AssetTypes.TRACK;
            tmpBtn = tmp.GetComponent<StockButton>();
            tmpBtn.itemNameLbl.text = bi.itemname;
            tmpBtn.itemThumb.sprite = thumbs.FirstOrDefault<Sprite>(x => x.name == bi.prefab);
            tmpBtn.OriginalObject = bi;
        }
        Invoke("DisableDesign", .25f); 
    }
    void DisableDesign() { 
        DesignController.instance.gameObject.SetActive(false);
    }

    public void ShowDetails(BaseItem Bi)
    {
        if (detailsPane.alpha == 0)
            detailsPane.DOFade(1, .3f);
        wallet.text = $"Balance: {GamePlay.coin}";
        mainDetails[0].text = Bi.itemname;
        mainDetails[1].text = $"{Bi.desc}\nPurchase price: {Bi.purchasecost.ToString()}\nRunning costs: {Bi.maintcost.ToString()}";
        CurrentItem = Bi;
        TrackEditor.instance.Displayer(CurrentItem.connections);
        int SaveLayer = PreviewTemp.layer;

        Destroy(PreviewTemp);

        PreviewTemp=   Instantiate(Resources.Load(Bi.prefab) as GameObject, PreviewParent.transform);
        PreviewTemp.transform.localPosition = Vector3.zero;
        PreviewTemp.layer = SaveLayer;
        for (int i = 0; i < PreviewTemp.transform.childCount;i++)
        
            PreviewTemp.transform.GetChild(i).gameObject.layer = SaveLayer;
        
        PreviewTemp.AddComponent<BasicRotation>();
          }
    BaseItem CurrentItem;
    public void CloseDetails()
    {
        detailsPane.DOFade(0, .3f);
    }

    public void BuyItem(bool _cheatMode)
    {
        if (GamePlay.inventory == null)
            GamePlay.inventory = new Dictionary<string, BaseItem>();
        if (_cheatMode || GamePlay.coin > CurrentItem.purchasecost)
        {
            if (!CanBuild(CurrentItem.itemname))
            {
                BuildTile bt = new BuildTile();
                bt.buildPrefab = Resources.Load(CurrentItem.prefab) as GameObject;
                bt.myName = CurrentItem.itemname;
                bt.tileType = CurrentItem.AssetType == AssetTypes.TRACK ? BuildTile.TileTypes.RAIL : BuildTile.TileTypes.SCENARY;
                CurrentItem.qtyInStock = 1;
               BuildTile. TileConnections[] baseConnections = new BuildTile. TileConnections[CurrentItem.connections.Length];
                //Decode connections.
                int r = -1;
                string cons = CurrentItem.connections.ToUpper();
                for(int i=0;i<CurrentItem.connections.Length;i++)
                {
                    switch (cons[i])
                    {
                        case 'N':
                            r = 0;
                            break;
                        case 'E':
                            r = 1;
                            break;
                        case 'S':
                            r = 2;
                            break;
                        case 'W':
                            r = 3;
                            break;

                        default:
                            break;
                    }
                    baseConnections[i] = (BuildTile.TileConnections)r ;
                }
                bt.baseConnections = baseConnections;


                bt.ForceValidate();
                GamePlay.inventory.Add(CurrentItem.itemname, CurrentItem);
                DesignController.instance.AddBuildTile(bt);
            }
            else
            {
                incInventory(CurrentItem.itemname);
            }
            GamePlay.coin -= !_cheatMode? CurrentItem.purchasecost:0;
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
    public void TrumpMode()
    {
        //Will purchase ALL STOCK ITEMS x 10 ??
        foreach (StockButton sb in contentHolder.GetComponentsInChildren<StockButton>())
        {
            CurrentItem = sb.OriginalObject;
            for (int i = 0; i < 10; i++)
            {
                Debug.Log($"Buying {CurrentItem.itemname}");
                BuyItem(true);

            }
        }
    }

    public void incInventory(string _name)
    {
        if (GamePlay.inventory.ContainsKey(_name))
            GamePlay.inventory[_name].qtyInStock++;
       
    }
    public void decInvenctory(string _name)
    {
        GamePlay.inventory[_name].qtyInStock -= GamePlay.inventory[_name].qtyInStock > 0 ? 1 : 0;
    }
    public int CheckInventory(BuildTile _item)
    {
        return GamePlay.inventory[_item.name].qtyInStock;
    }
    public bool CanBuild(BuildTile _item)
    {
        return GamePlay.inventory[_item.name].qtyInStock > 0;
    }
    public bool CanBuild(string  _name)
    {
        if(GamePlay.inventory.ContainsKey(_name))
        return GamePlay.inventory[_name].qtyInStock > 0;
        return false;
    }

    public void ToggleDesign(bool _toggle)
    {
        DesignController.instance.gameObject.SetActive(_toggle);
    }

}