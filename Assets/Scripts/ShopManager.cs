using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public ShopTrigger trigger;
    public GameObject shopUI;
    public SourceMovement playerMovement;
    void Update()
    {
        if (trigger.playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            bool isOpening = !shopUI.activeSelf;

            shopUI.SetActive(isOpening);

            if (isOpening)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                playerMovement.canMove = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
                playerMovement.canMove = true;
            }
        }
    }
}
