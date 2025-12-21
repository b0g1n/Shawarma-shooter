using UnityEngine;
using TMPro;

public class ShopItems2 : MonoBehaviour
{
    public Shotgun gun;
    public GameManager money;
    public SourceMovement playerMovement;
    public PlayerHealth playerHealth;

    [Header("Upgrade Prices")]
    public int pelletDamagePrice = 100;
    public int fireRatePrice = 100;
    public int pelletCountPrice = 200;
    public int reloadSpeedPrice = 150;
    public int speedPrice = 50;
    public int jumpPrice = 25;

    [Header("Button Texts")]
    public TMP_Text pelletDamageText;
    public TMP_Text fireRateText;
    public TMP_Text pelletCountText;
    public TMP_Text reloadSpeedText;
    public TMP_Text speedText;
    public TMP_Text jumpText;

    private const float PRICE_MULTIPLIER = 1.25f;
    private const float MIN_RELOAD_TIME = 0.3f;

    void Start()
    {
        UpdateAllTexts();
    }

    bool TrySpendCash(int cost)
    {
        if (money.playerCash >= cost)
        {
            money.playerCash -= cost;
            return true;
        }
        return false;
    }

    int ScalePrice(int price)
    {
        return Mathf.CeilToInt(price * PRICE_MULTIPLIER);
    }

    void UpdateAllTexts()
    {
        pelletDamageText.text = "$" + pelletDamagePrice;
        fireRateText.text     = "$" + fireRatePrice;
        pelletCountText.text  = "$" + pelletCountPrice;
        reloadSpeedText.text  = "$" + reloadSpeedPrice;
        speedText.text        = "$" + speedPrice;
        jumpText.text         = "$" + jumpPrice;
    }

    // PELLET DAMAGE
    public void UpgradePelletDamage()
    {
        if (!TrySpendCash(pelletDamagePrice)) return;

        gun.pelletDamage += 1f;
        pelletDamagePrice = ScalePrice(pelletDamagePrice);
        UpdateAllTexts();
    }

    // FIRE RATE
    public void UpgradeFireRate()
    {
        if (!TrySpendCash(fireRatePrice)) return;

        gun.fireRate += 0.2f;
        fireRatePrice = ScalePrice(fireRatePrice);
        UpdateAllTexts();
    }

    // PELLET COUNT
    public void UpgradePelletCount()
    {
        if (!TrySpendCash(pelletCountPrice)) return;

        gun.pelletCount += 1;
        pelletCountPrice = ScalePrice(pelletCountPrice);
        UpdateAllTexts();
    }

    // RELOAD SPEED (LOWER = FASTER)
    public void UpgradeReloadSpeed()
    {
        if (!TrySpendCash(reloadSpeedPrice)) return;

        gun.reloadTime = Mathf.Max(
            MIN_RELOAD_TIME,
            gun.reloadTime - 0.5f
        );

        reloadSpeedPrice = ScalePrice(reloadSpeedPrice);
        UpdateAllTexts();
    }

    // SPEED
    public void UpgradePlayerSpeed()
    {
        if (!TrySpendCash(speedPrice)) return;

        playerMovement.sprintSpeed += 1.5f;
        speedPrice = ScalePrice(speedPrice);
        UpdateAllTexts();
    }

    // JUMP
    public void UpgradeJumpForce()
    {
        if (!TrySpendCash(jumpPrice)) return;

        playerMovement.jumpForce += 1f;
        jumpPrice = ScalePrice(jumpPrice);
        UpdateAllTexts();
    }
}
