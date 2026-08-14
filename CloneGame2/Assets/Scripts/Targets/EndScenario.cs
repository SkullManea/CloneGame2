using System.Collections;
using UnityEngine;

public class EndScenario : MonoBehaviour
{
     public GameObject gameOverScreen;
     public Spawning spawning;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball") && spawning.endScenario)
        {
            Pathing balls = collision.gameObject.GetComponent<Pathing>();
            Debug.Log("Collision");
            balls.EndState();
            StartCoroutine(Ending());
           
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        gameOverScreen.SetActive(true);
        Debug.Log("GameOver");
    }

    private IEnumerator Ending()
    {
        yield return new WaitForSeconds (2f);
        GameOver();
    }
}
