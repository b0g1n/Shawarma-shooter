using System.Collections;
using UnityEngine;
using TMPro;

public class RandomMemePopup : MonoBehaviour
{
    [Header("UI")]
    public GameObject memeImage;
    public TMP_Text promptText;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Timing")]
    public float minTime = 40f;
    public float maxTime = 60f;

    [Header("Allowed Letters (A–Z only)")]
    public char[] allowedLetters; // example: A B C D

    private char currentLetter;
    private bool isActive = false;

    void Start()
    {
        memeImage.SetActive(false);
        promptText.gameObject.SetActive(false);
        StartCoroutine(MemeLoop());
    }

    void Update()
    {
        if (!isActive) return;

        // Check typed characters (handles lower + upper case)
        foreach (char c in Input.inputString)
        {
            if (char.ToUpper(c) == currentLetter)
            {
                CloseMeme();
                break;
            }
        }
    }

    IEnumerator MemeLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);

            ShowMeme();

            while (isActive)
                yield return null;
        }
    }

    void ShowMeme()
    {
        currentLetter = allowedLetters[Random.Range(0, allowedLetters.Length)];
        currentLetter = char.ToUpper(currentLetter);

        memeImage.SetActive(true);
        promptText.gameObject.SetActive(true);
        promptText.text = $"Press {currentLetter} to close";

        audioSource.Play();
        isActive = true;
    }

    void CloseMeme()
    {
        memeImage.SetActive(false);
        promptText.gameObject.SetActive(false);

        audioSource.Stop();
        isActive = false;
    }
}
