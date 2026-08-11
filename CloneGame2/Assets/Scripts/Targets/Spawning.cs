using UnityEngine;

public class Spawning : MonoBehaviour
{
    public GameObject[] ballPrefabs;
    public Transform ballPos;

    private float timer;
   

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
        int index = Random.Range(0, ballPrefabs.Length);
        GameObject prefabToSpawn = ballPrefabs[index];
        Instantiate(prefabToSpawn, ballPos.position, Quaternion.identity);
    }
}
