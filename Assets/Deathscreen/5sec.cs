using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public float delayBeforeMenu = 5f; // seconds

    void Start()
    {
        Invoke("LoadMenu", delayBeforeMenu);
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
