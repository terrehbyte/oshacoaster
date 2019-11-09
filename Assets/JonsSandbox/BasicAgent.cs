using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicAgent : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject Target;
    public bool searching = true;
    // Start is called before the first frame update
    void MoveToLocation()
    {
        agent.destination = Target.transform.position ;
        agent.isStopped = false;
    }

    // Update is called once per frame
    void Awake()
    {
        StartCoroutine(RefreshDest());
    }

    public IEnumerator RefreshDest()
    {
       while (searching)
        {
            MoveToLocation();
            yield return new WaitForSeconds(2);

        }
    }
}
