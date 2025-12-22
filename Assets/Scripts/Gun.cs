using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 10f;

    [Header("Ammo Settings")]
    [SerializeField] public int magazineSize = 15;
    public float reloadTime = 2f;
    private int currentAmmo;
    private bool isReloading = false;
    public bool infiniteAmmo;
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
    private AudioSource gunAudio;

    [Header("Reload Animation")]
    public Vector3 reloadOffset = new Vector3(0f, -6f, 0f);
    private Vector3 originalLocalPos;

    [Header("Tracer Settings")]
    public GameObject tracerPrefab;
    public float tracerLifetime = 0.02f;
    public Vector3 tracerOffset = new Vector3(0f, -0.15f, 0.25f);

    [Header("Raycast Settings")]
    public LayerMask ignoreLayers;
    public float muzzleOffset = 0.15f;

    private float nextTimeToFire = 0f;

    void Awake()
    {
        gunAudio = GetComponent<AudioSource>();
        gunAudio.playOnAwake = false;

        currentAmmo = magazineSize;
        originalLocalPos = transform.localPosition;
        UpdateUI();
    }

    void Update()
    {
        if (fpsCam == null) return;
        if (isReloading) return;

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
                gunAudio.PlayOneShot(emptyClickClip);

            return;
        }

        if (reloadText != null)
            reloadText.gameObject.SetActive(false);

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if(!infiniteAmmo)
        currentAmmo--;
        UpdateUI();

        if (gunShotClip != null)
            gunAudio.PlayOneShot(gunShotClip);

        if (muzzleFlash != null && muzzlePoint != null)
        {
            muzzleFlash.transform.position = muzzlePoint.position;
            muzzleFlash.transform.rotation = muzzlePoint.rotation;
            muzzleFlash.Play();
        }

        Vector3 origin = fpsCam.transform.position + fpsCam.transform.forward * muzzleOffset;
        Vector3 direction = fpsCam.transform.forward;

        RaycastHit hit;
        int layerMask = ~ignoreLayers.value;

        bool didHit = Physics.Raycast(origin, direction, out hit, range, layerMask, QueryTriggerInteraction.Ignore);

        Vector3 tracerStart =
            fpsCam.transform.position +
            fpsCam.transform.forward * tracerOffset.z +
            fpsCam.transform.right * tracerOffset.x +
            fpsCam.transform.up * tracerOffset.y;

        Vector3 tracerEnd = didHit ? hit.point : origin + direction * range;

        if (didHit)
        {
            Target target = hit.collider.GetComponent<Target>();
            if (target != null)
                target.TakeDamage(damage);
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
            gunAudio.PlayOneShot(reloadClip);

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
    // Call this from shop / upgrades
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

        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        yield return new WaitForSeconds(tracerLifetime);
        Destroy(tracer);
    }
}
