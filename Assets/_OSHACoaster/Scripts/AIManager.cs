﻿using System;
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
    List<GameObject> Targets = new List<GameObject>();
    List<GameObject> Meeples = new List<GameObject>();
    int TargetCount;
    int MeepleCount;
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
            for (int i = 0; i < MeepleCount; i++)
            {
                if (stepCounter++ > RefreshPopulationEverySteps)
                {
                    stepCounter = 0;
                    RefreshAvailableMeeeples();
                    RefreshAvailableTargets();

                }
                NavMeshAgent nma = Meeples[i].GetComponent<NavMeshAgent>();
                Animator Anim = Meeples[i].GetComponentInChildren<Animator>();
                float vel = nma.velocity.magnitude;
                Anim.SetFloat("Vel", vel);
                if (vel< IdleVelocity)
                {
                    nma.SetDestination(Targets[Random.Range(0, TargetCount)].transform.position);
                }
              

                yield return new WaitForSeconds(Random.Range(0, 3.1f));
            }
        }
    }
}
