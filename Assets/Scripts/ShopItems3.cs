using UnityEngine;
using TMPro;

public class ShopItems3 : MonoBehaviour
{
    public SMG gun;
    public GameManager money;
    public SourceMovement playerMovement;
    public PlayerHealth playerHealth;

    [Header("Upgrade Prices")]
    public int damagePrice = 100;
    public int fireRatePrice = 150;
    public int speedPrice = 50;
    public int jumpPrice = 25;
    public int healthPrice = 40;
    public int ammoPrice = 150;

    [Header("Button Texts")]
    public TMP_Text damageText;
    public TMP_Text fireRateText;
    public TMP_Text speedText;
    public TMP_Text jumpText;
    public TMP_Text healthText;
    public TMP_Text ammoText;

    private const float PRICE_MULTIPLIER = 1.25f;

    private void Start()
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
        damageText.text   = "$" + damagePrice;
        fireRateText.text = "$" + fireRatePrice;
        speedText.text    = "$" + speedPrice;
        jumpText.text     = "$" + jumpPrice;
        healthText.text   = "$" + healthPrice;
        ammoText.text     = "$" + ammoPrice;
    }


    // DAMAGE
    public void UpgradeDamage()
    {
        if (!TrySpendCash(damagePrice)) return;

        gun.damage += 5f;
        damagePrice = ScalePrice(damagePrice);
        UpdateAllTexts();
    }

    // FIRE RATE
    public void UpgradeFireRate()
    {
        if (!TrySpendCash(fireRatePrice)) return;

        gun.fireRate += 1.5f;
        fireRatePrice = ScalePrice(fireRatePrice);
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

    // HEALTH
    public void BuyInstantHealth()
    {
        if (!TrySpendCash(healthPrice)) return;

        playerHealth.currentHealth = Mathf.Min(
            playerHealth.currentHealth + 50,
            playerHealth.maxHealth
        );

        healthPrice = ScalePrice(healthPrice);
        UpdateAllTexts();
    }

    // AMMO
    public void BuyAmmo()
    {
        if (!TrySpendCash(ammoPrice)) return;

        gun.SetMagazineSize(gun.magazineSize + 15);
        ammoPrice = ScalePrice(ammoPrice);
        UpdateAllTexts();
    }
}
