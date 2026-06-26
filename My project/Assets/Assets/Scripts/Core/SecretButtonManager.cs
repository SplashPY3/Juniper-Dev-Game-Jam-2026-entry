using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretButtonManager : MonoBehaviour
{
    public void LoadLastScene()
    {
        SceneManager.LoadScene("ThankYouScene");
    }
}
