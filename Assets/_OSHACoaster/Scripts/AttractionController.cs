using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class AttractionController : MonoBehaviour
{
    List<GameObject> riders = new List<GameObject>();
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

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Meeple") { return; }
        
        riders.Add(other.gameObject);
        other.gameObject.SetActive(false);

        OnRiderEntered.Invoke();

        if(riderCount >= minRiders)
        {
            OnRideBegins.Invoke();
            StartCoroutine(DoRide(rideDuration));
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Meeple") { return; }
        
        riders.Remove(other.gameObject);
    }

    IEnumerator DoRide(float duration)
    {
        yield return new WaitForSeconds(duration);

        OnRideExit.Invoke();

        foreach(var rider in riders)
        {
            rider.SetActive(true);
        }

        riders.Clear();
    }
}
