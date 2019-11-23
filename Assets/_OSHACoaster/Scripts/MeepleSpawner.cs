using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeepleSpawner : MonoBehaviour
{
    public float spawnInterval;
    private float spawnTimer;
    public float spawnRange;

    public GameObject meeplePrefab;
    public int spawnMax;

    public LayerMask mask;

    void Start()
    {
        SpawnWave(20);
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer < spawnInterval) { return; }

        spawnTimer -= spawnInterval;

        SpawnWave(Random.Range(1, spawnMax+1));
    }

    void SpawnWave(int waveSize)
    {
        RaycastHit hit;

        Vector3 raycastOrigin = transform.position + new Vector3(Random.Range(0.0f, spawnRange), 0.0f, Random.Range(0.0f, spawnRange));

        bool success = Physics.Raycast(raycastOrigin, Vector3.down, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore);

        if (success)
        {
            Vector3 meeplePosition = hit.point;

            int spawnCount = Random.Range(1, spawnMax + 1);
            for (int i = 0; i < waveSize; ++i)
            {
                Instantiate(meeplePrefab, meeplePosition, Quaternion.identity);
                Debug.Log("Meeple spawned!");
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}
