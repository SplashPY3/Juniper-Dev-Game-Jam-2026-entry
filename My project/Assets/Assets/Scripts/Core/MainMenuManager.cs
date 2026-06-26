using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Play()
    {
        PlayerManager.Instance.StartNewRun();
        RunManager.Instance.StartNewRun();
        SceneManager.LoadScene("EnemySelection");
    }
}
