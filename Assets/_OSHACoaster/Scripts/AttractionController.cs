using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class AttractionController : MonoBehaviour
{
    [SerializeField]
    List<GameObject> riders = new List<GameObject>();
    [SerializeField]
    List<GameObject> queue = new List<GameObject>();
    public int minRiders;

    public int riderCount
    {
        get
        {
            return riders.Count;
        }
    }

    public UnityEvent OnRideBegins;
    public UnityEvent OnRideExit;

    public UnityEvent OnRiderEntered;

    public float rideDuration = 6.0f;

    public void AddRider(GameObject rider)
    {
        riders.Add(rider.gameObject);
        rider.gameObject.SetActive(false);
    }

    public void AddQueue(GameObject rider)
    {
        queue.Add(rider.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Meeple") { return; }
        if (riders.Contains(other.gameObject)) { return; } // don't add existing riders
        if (queue.Contains(other.gameObject)) { return; } // don't add existing riders

        if(riderCount < minRiders)
        {
            AddRider(other.gameObject);
        }
        else
        {
            AddQueue(other.gameObject);
        }

        OnRiderEntered.Invoke();

        if(riderCount >= minRiders)
        {
            OnRideBegins.Invoke();
            StartCoroutine(DoRide(rideDuration));
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.activeSelf) { return; }
        
        riders.Remove(other.gameObject);
        queue.Remove(other.gameObject);
    }

    IEnumerator DoRide(float duration)
    {
        yield return new WaitForSeconds(duration);

        OnRideExit.Invoke();

        foreach(var rider in riders)
        {
            rider.SetActive(true);
        }

        // try to source more riders
        // exit when...
        //  QUEUE is exhausted
        //  CAPACITY is met
        while(queue.Count > 0 && riderCount < minRiders)
        {
            AddRider(queue[0]);
            queue.Remove(queue[0]);
        }
    }
}
