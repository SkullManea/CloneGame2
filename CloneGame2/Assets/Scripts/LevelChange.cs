using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChange : MonoBehaviour
{
    public string nextSceneName ="Level 2";
public void NextLevel()
    {
        SceneManager.LoadScene(nextSceneName);
    }

}
