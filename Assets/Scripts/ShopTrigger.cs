using UnityEngine;
using TMPro;

public class ShopTrigger : MonoBehaviour
{
    public bool playerInRange;
    public TMP_Text pressE;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressE != null)
                pressE.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (pressE != null)
                pressE.gameObject.SetActive(false);
        }
    }
}
