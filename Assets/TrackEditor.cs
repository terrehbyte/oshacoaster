using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TrackEditor : MonoBehaviour
{
    public Toggle[] directions;
    // Start is called before the first frame update
    public static TrackEditor instance;
    public RenderTexture ThumbnailGrabber;

    public void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);

        if (instance == null)
            instance = this;



    }
    public void GrabThumbNail()
    {
    //   Texture2D thumbNail=ThumbnailGrabber.

    }
    public void Displayer(string str)
    {
        str = str.ToUpper();
        directions[0].isOn = str.IndexOf('N') > 0;
        directions[1].isOn = str.IndexOf('E') > 0;
        directions[2].isOn = str.IndexOf('S') > 0;
        directions[3].isOn = str.IndexOf('W') > 0;
    }
    // Update is called once per frame
    public void  Save()
    {
        string str = "";
        if (directions[0].isOn)
            str += "N";
        if (directions[1].isOn)
            str += "E";
        if (directions[2].isOn)
            str += "S";
        if (directions[3].isOn)
            str += "W";
        StockList.instance.UpdateDirections(str);
        StockList.instance.Save();
    }
}
