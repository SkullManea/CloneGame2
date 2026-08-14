using UnityEngine;

public class EndScenario : MonoBehaviour
{
     public GameObject gameOverScreen;
     public Spawning spawning;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Collision");
            //GameOver();
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        gameOverScreen.SetActive(true);
        Debug.Log("GameOver");
    }
}
