using System.Collections;
using UnityEngine;

public class Spawning : MonoBehaviour
{
    public GameObject[] ballPrefabs;
    public Transform ballPos;
    public float spawnTimer;
    public float endCondition;

    private float timer;
    private bool isSpawning = true;
    public bool endScenario = false;
   
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

        yield return new WaitForSeconds(endCondition);
        endScenario = true;
        Debug.Log("End");
    }
}
