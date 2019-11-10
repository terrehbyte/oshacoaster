using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ScoreTally : MonoBehaviour
{
    public float desiredScore = 9999;
    public float tallyTime = 10;
    float temp;
    // Start is called before the first frame update
    void Start()
    {
        temp = 0;
    }

    // Update is called once per frame
    void Update()
    {
        temp = Mathf.MoveTowards(temp, 1.0f, Time.deltaTime / tallyTime);
        int scoreCalc = (int)Mathf.Lerp(0, desiredScore, temp);
        string displayedScore = string.Format("{0:D4}", scoreCalc);
        GetComponent<TMPro.TextMeshProUGUI>().text = displayedScore.ToString();
        if (scoreCalc > desiredScore / 1.1f)
            tallyTime += 0.75f;
    }
}
