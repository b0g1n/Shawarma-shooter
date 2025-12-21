using UnityEngine;

public class LevelUnlocker : MonoBehaviour
{
    public int levelNumber;

    void Start()
    {
        if (PlayerPrefs.GetInt("UnlockedLevel", 1) < levelNumber)
        {
            gameObject.SetActive(false);
        }
    }
}
