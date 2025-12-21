using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class Shotgun : MonoBehaviour
{
    [Header("Shotgun Settings")]
    public int pelletCount = 8;
    public float pelletDamage = 6f;
    public float range = 40f;
    public float fireRate = 1.2f;
    public float spreadAngle = 6f;

    [Header("Ammo Settings")]
    public int magazineSize = 6;
    public float reloadTime = 2.5f;
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
    public AudioClip shotgunBlastClip;
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

    private float nextTimeToFire;

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
                gunAudio.PlayOneShot(emptyClickClip);

            return;
        }

        if (reloadText != null)
            reloadText.gameObject.SetActive(false);

        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        currentAmmo--;
        UpdateUI();

        nextTimeToFire = Time.time + 1f / fireRate;

        if (shotgunBlastClip != null)
            gunAudio.PlayOneShot(shotgunBlastClip);

        if (muzzleFlash != null && muzzlePoint != null)
        {
            muzzleFlash.transform.position = muzzlePoint.position;
            muzzleFlash.transform.rotation = muzzlePoint.rotation;
            muzzleFlash.Play();
        }

        Vector3 origin = fpsCam.transform.position + fpsCam.transform.forward * muzzleOffset;
        int layerMask = ~ignoreLayers.value;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction =
                fpsCam.transform.forward +
                fpsCam.transform.right * Random.Range(-spreadAngle, spreadAngle) * 0.01f +
                fpsCam.transform.up * Random.Range(-spreadAngle, spreadAngle) * 0.01f;

            direction.Normalize();

            RaycastHit hit;
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
                    target.TakeDamage(pelletDamage);
            }

            if (tracerPrefab != null)
                StartCoroutine(SpawnTracer(tracerStart, tracerEnd));
        }
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
