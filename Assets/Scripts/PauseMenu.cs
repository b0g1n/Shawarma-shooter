using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    public SourceMovement playerMovement;
    public Slider volumeSlider;

    void Start()
    {
        pauseUI.SetActive(false);

        // Load saved volume
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f); // default 1
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;

        volumeSlider.onValueChanged.AddListener(SetVolume);

        LockCursorImmediate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseUI.activeSelf)
                Pause();
            else
                Resume();
        }
    }

    void Pause()
    {
        pauseUI.SetActive(true);

        Time.timeScale = 0f;
        playerMovement.canMove = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Resume()
    {
        pauseUI.SetActive(false);

        Time.timeScale = 1f;
        playerMovement.canMove = true;

        StartCoroutine(LockCursorNextFrame());
    }

    IEnumerator LockCursorNextFrame()
    {
        yield return new WaitForEndOfFrame();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LockCursorImmediate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !pauseUI.activeSelf)
            LockCursorImmediate();
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value); // save volume
        PlayerPrefs.Save();
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("Menu");
    }
}
