using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool Running = true;
    public float IdleVelocity;
    public int RefreshPopulationEverySteps;
    public List<GameObject> Targets = new List<GameObject>();
    public List<GameObject> Meeples = new List<GameObject>();
    int TargetCount;
    int MeepleCount;
    public static AIManager instance;
  
    void Start()
    {
    }

    public void RefreshAvailableTargets()
    {
        GameObject[] tmpTargets = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject tgo in tmpTargets)
            if (!Targets.Contains(tgo))
                Targets.Add(tgo);

        TargetCount = Targets.Count;
    }
    public void RefreshAvailableMeeeples()
    {
        GameObject[] tmpMeeples = GameObject.FindGameObjectsWithTag("Meeple");
        foreach (GameObject tgo in tmpMeeples)
            if (!Meeples.Contains(tgo))
                Meeples.Add(tgo);
        MeepleCount = Meeples.Count;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        if (instance == null)
            instance = this;

        RefreshAvailableTargets();
        RefreshAvailableMeeeples();
        StartCoroutine(MonitorAI());
        
    }

   
    private IEnumerator MonitorAI()
    {
        int stepCounter = 0;
        yield return null;
        while (Running)
        {
            RefreshAvailableMeeeples();
            RefreshAvailableTargets();

            if (Meeples.Count > 0 && Targets.Count > 0)
            {

                for (int i = 0; i < MeepleCount; i++)
                {
                    if (stepCounter++ > RefreshPopulationEverySteps)
                    {
                        stepCounter = 0;

                    }
                    if (Meeples[i].gameObject.activeInHierarchy)
                    {
                        NavMeshAgent nma = Meeples[i].GetComponent<NavMeshAgent>();
                        Animator Anim = Meeples[i].GetComponentInChildren<Animator>();
                        float vel = nma.velocity.magnitude;
                        Anim.SetFloat("Vel", vel);
                        if (vel < IdleVelocity)
                        {
                            nma.SetDestination(Targets[Random.Range(0, TargetCount)].transform.position);
                        }
                    }

                    yield return new WaitForSeconds(Random.Range(0, .25f));
                }
            }
            yield return new WaitForSeconds(Random.Range(0, .25f));
        }
    }
}
