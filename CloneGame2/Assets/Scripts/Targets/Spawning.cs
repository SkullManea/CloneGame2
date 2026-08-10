using UnityEngine;

public class Spawning : MonoBehaviour
{
    public GameObject ball;
    public Transform ballPos;

    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 0.7f)
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        Instantiate(ball, ballPos.position, Quaternion.identity);
    }
}
