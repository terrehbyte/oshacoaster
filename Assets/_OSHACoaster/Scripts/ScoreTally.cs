using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ScoreTally : MonoBehaviour
{
    public float desiredScore = 9999;
    public float tallyTime = 10;
    public TMPro.TextMeshProUGUI myTmp;
    float temp;
    bool SlowedDown = false;
    // Start is called before the first frame update
    void Start()
    {
        myTmp = GetComponent<TMPro.TextMeshProUGUI>();
        temp = 0;
    }

    // Update is called once per frame
    void Update()
    {
        temp = Mathf.MoveTowards(temp, 1.0f, Time.deltaTime / tallyTime);
        int scoreCalc = (int)Mathf.Lerp(0, desiredScore, temp);

        string displayedScore = string.Format("{0:D4}", scoreCalc);
        myTmp.text = displayedScore.ToString();
        if (!SlowedDown && scoreCalc > desiredScore *.95f)
        {
            SlowedDown = true;
            tallyTime -= 5f;
        }
    }
}
