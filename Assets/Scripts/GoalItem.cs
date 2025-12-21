using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GoalItem : MonoBehaviour
{
    [Header("References")]
    public Image fadeImage;          
    public TMP_Text goalText;        
    public AudioSource audioSource;  
    public AudioClip goalSound;      

    [Header("Fade Settings")]
    public float fadeDuration = 2f;
    public float waitAfterGoal = 15f;

    [Header("Spin Settings")]
    public float spinSpeed = 180f;

    private bool activated = false;

    private void Update()
    {
        if (!activated)
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;
            StartCoroutine(HandleGoal());
        }
    }

    private IEnumerator HandleGoal()
    {
        float timer = 0f;
        Color imgColor = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = new Color(imgColor.r, imgColor.g, imgColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(imgColor.r, imgColor.g, imgColor.b, 1f);

        if (goalText != null)
            goalText.gameObject.SetActive(true);

        if (audioSource != null && goalSound != null)
        {
            audioSource.clip = goalSound;
            audioSource.Play();
        }

        yield return new WaitForSeconds(waitAfterGoal);

        SceneManager.LoadScene("Menu");
    }
}
