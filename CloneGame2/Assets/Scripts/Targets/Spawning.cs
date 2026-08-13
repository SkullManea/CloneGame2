using System.Collections;
using UnityEngine;

public class Spawning : MonoBehaviour
{
    public GameObject[] ballPrefabs;
    public Transform ballPos;
    public float spawnTimer;

    private float timer;
    private bool isSpawning = true;
   
    void Start()
    {
        StartCoroutine(SpawningTimer());
    }
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 1.3f)
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        if (isSpawning)
        {
         int index = Random.Range(0, ballPrefabs.Length);
        GameObject prefabToSpawn = ballPrefabs[index];
        Instantiate(prefabToSpawn, ballPos.position, Quaternion.identity);   
        }
        
    }
    
    private IEnumerator SpawningTimer()
    {
        yield return new WaitForSeconds(spawnTimer);
        isSpawning = false;
        Debug.Log("stopped");
    }
}
