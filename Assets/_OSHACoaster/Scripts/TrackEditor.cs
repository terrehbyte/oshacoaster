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
        {
            Debug.LogWarning("Duplicate instance of TrackEditor found! Now deleting...");
            Destroy(gameObject);
        }

        if (instance == null)
            instance = this;

        Debug.Assert(directions != null, this);
        foreach(var dir in directions)
        {
            Debug.Assert(dir != null, this);
        }

    }

    public void Displayer(string str)
    {
        if (str == null) return;
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
        #if UNITY_EDITOR
        StockList.instance.Save();
        #endif
    }
}
