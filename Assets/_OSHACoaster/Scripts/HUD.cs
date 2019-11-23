using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMP = TMPro.TextMeshProUGUI;

public class HUD : MonoBehaviour
{
    public TMP monies;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        monies.text =  "Balance:" + GamePlay.coin.ToString();
    }
}
