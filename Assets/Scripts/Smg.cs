using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class SMG : MonoBehaviour
{
    [Header("SMG Stats")]
    public float damage = 12f;
    public float range = 100f;
    public float fireRate = 18f;
    public float bulletSpread = 1.5f;

    [Header("Ammo")]
    public int magazineSize = 30;
    public float reloadTime = 2f;
    private int currentAmmo;
    private bool isReloading;

    [Header("UI")]
    public TMP_Text ammoText;
    public TMP_Text reloadText;

    [Header("References")]
    public Camera fpsCam;
    public Transform muzzlePoint;
    public ParticleSystem muzzleFlash;

    [Header("Audio")]
    public AudioClip gunShotClip;
    public AudioClip emptyClickClip;
    public AudioClip reloadClip;
    private AudioSource audioSource;

    [Header("Reload Animation")]
    public Vector3 reloadOffset = new Vector3(0f, -6f, 0f);
    private Vector3 originalLocalPos;

    [Header("Tracer")]
    public GameObject tracerPrefab;
    public float tracerLifetime = 0.02f;

    [Header("Raycast")]
    public LayerMask ignoreLayers;
    public float cameraForwardOffset = 0.15f;

    private float nextFireTime;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        currentAmmo = magazineSize;
        originalLocalPos = transform.localPosition;
        UpdateUI();
    }

    void Update()
    {
        if (fpsCam == null || isReloading)
            return;

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize)
        {
            StartCoroutine(Reload());
            return;
        }

        if (currentAmmo <= 0)
        {
            if (reloadText != null)
                reloadText.gameObject.SetActive(true);

            if (Input.GetButtonDown("Fire1") && emptyClickClip != null)
                audioSource.PlayOneShot(emptyClickClip);

            return;
        }

        if (reloadText != null)
            reloadText.gameObject.SetActive(false);

        // FULL AUTO
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / fireRate);
            Shoot();
        }
    }

    void Shoot()
    {
        currentAmmo--;
        UpdateUI();

        if (gunShotClip != null)
            audioSource.PlayOneShot(gunShotClip);

        if (muzzleFlash != null && muzzlePoint != null)
        {
            muzzleFlash.transform.SetPositionAndRotation(
                muzzlePoint.position,
                muzzlePoint.rotation
            );
            muzzleFlash.Play();
        }

        // Raycast from camera (for accuracy)
        Vector3 rayOrigin = fpsCam.transform.position + fpsCam.transform.forward * cameraForwardOffset;

        float spreadX = Random.Range(-bulletSpread, bulletSpread);
        float spreadY = Random.Range(-bulletSpread, bulletSpread);
        Vector3 direction =
            Quaternion.Euler(spreadY, spreadX, 0f) * fpsCam.transform.forward;

        RaycastHit hit;
        int layerMask = ~ignoreLayers.value;

        bool didHit = Physics.Raycast(
            rayOrigin,
            direction,
            out hit,
            range,
            layerMask,
            QueryTriggerInteraction.Ignore
        );

        // Tracer ALWAYS starts from muzzle
        Vector3 tracerStart = muzzlePoint.position;
        Vector3 tracerEnd = didHit ? hit.point : rayOrigin + direction * range;

        if (didHit)
        {
            TargetBoss targetBoss = hit.collider.GetComponent<TargetBoss>();
            Target target = hit.collider.GetComponent<Target>();
            if (target != null)
                target.TakeDamage(damage);
            else if(targetBoss != null)
            targetBoss.TakeDamage(damage);
        }

        if (tracerPrefab != null)
            StartCoroutine(SpawnTracer(tracerStart, tracerEnd));
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (reloadText != null)
        {
            reloadText.text = "Reloading...";
            reloadText.gameObject.SetActive(true);
        }

        if (reloadClip != null)
            audioSource.PlayOneShot(reloadClip);

        Vector3 downPos = originalLocalPos + reloadOffset;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            transform.localPosition = Vector3.Lerp(originalLocalPos, downPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(reloadTime);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            transform.localPosition = Vector3.Lerp(downPos, originalLocalPos, t);
            yield return null;
        }

        currentAmmo = magazineSize;
        isReloading = false;

        if (reloadText != null)
        {
            reloadText.text = "Press R to Reload";
            reloadText.gameObject.SetActive(false);
        }
        UpdateUI();
    }

    public void SetMagazineSize(int newSize)
    {
        magazineSize = Mathf.Max(1, newSize);
        currentAmmo = Mathf.Clamp(currentAmmo, 0, magazineSize);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (ammoText != null)
            ammoText.text = currentAmmo + " / " + magazineSize;
    }

    IEnumerator SpawnTracer(Vector3 start, Vector3 end)
    {
        GameObject tracer = Instantiate(tracerPrefab, start, Quaternion.identity);
        LineRenderer lr = tracer.GetComponent<LineRenderer>();

        if (lr == null)
        {
            Destroy(tracer);
            yield break;
        }

        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        yield return new WaitForSeconds(tracerLifetime);
        Destroy(tracer);
    }
}
