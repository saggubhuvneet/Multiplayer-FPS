using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GunMech : MonoBehaviour
{
	[Header("Gun Specs")]
    public float fireRate ;
    public float weaponRange;
	public float damage ;

	[Header("Ammo and Reload")]
	public int maxAmmo ;
	public int totalAmmo ;
	private int currentAmmo;

	public float ReloadTime ;
	bool isReloading = false;

	public TextMeshProUGUI AmmoText;
	public TextMeshProUGUI ReloadText;
	public GameObject AmmoUi;

	[Header(" ")]
	public Transform gunEnd;
    public ParticleSystem muzzleFlash;
    public ParticleSystem cartridgeEjection;

	[Header("Hit effect")]
	public GameObject metalHitEffect;
	public GameObject sandHitEffect;
	public GameObject stoneHitEffect;
	public GameObject waterLeakEffect;
	public GameObject waterLeakExtinguishEffect;
	public GameObject[] fleshHitEffects;
	public GameObject woodHitEffect;

	[Header("Misslenous")]
	public GameObject mainCamera;
	public GameObject aimCamera;

	private float nextFire;
	bool isAimed;

	PhotonView PV;

    private void Awake()
    {
		PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
		isAimed = false;

        if (!PV.IsMine)
        {
			Destroy(mainCamera);
			Destroy(aimCamera);
			Destroy(AmmoUi);
        }
		currentAmmo = maxAmmo;
    }

    private void Update()
    {
		if (!PV.IsMine)
			return;


		if (isReloading)
        {
			AmmoText.gameObject.SetActive(false);
			ReloadText.gameObject.SetActive(true);
			return;
		}

		AimLock();

		AmmoText.gameObject.SetActive(true);
		ReloadText.gameObject.SetActive(false);


        AmmoText.text = currentAmmo.ToString() + "/" + totalAmmo.ToString();

        if (Input.GetKeyDown(KeyCode.R) || currentAmmo <= 0)
        {
			if (currentAmmo == maxAmmo || totalAmmo == 0)
				return;
			StartCoroutine(Reloading());
        }
        if(Input.GetButton("Fire1") && Time.time > nextFire)
        {
			Shoot();
        }    

    }


	public void Shoot()
    {
		currentAmmo --;

		nextFire = Time.time + fireRate;
		muzzleFlash.Play();
		cartridgeEjection.Play();

		Vector3 rayOrigin = gunEnd.position;
		RaycastHit hit;
		if (Physics.Raycast(rayOrigin, gunEnd.forward, out hit, weaponRange))
		{
			//Debug.Log(hit.collider);
			hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
			HandleHit(hit);
		}
	}

	IEnumerator Reloading()
    {
		isReloading = true;

		if(currentAmmo + totalAmmo <= maxAmmo)
        {
			currentAmmo = currentAmmo + totalAmmo;
			totalAmmo = 0;
        }
        else
		{
			totalAmmo = totalAmmo - maxAmmo + currentAmmo;
			currentAmmo = maxAmmo;
		}
		mainCamera.SetActive(true);
		aimCamera.SetActive(false);
		Debug.Log("Reloading.........");
		yield return new WaitForSeconds(ReloadTime);

		isReloading = false;
    }

	void HandleHit(RaycastHit hit)
	{
		if (hit.collider.tag != null)
		{
			string materialName = hit.collider.tag;

			switch (materialName)
			{
				case "test":
					SpawnDecal(hit, metalHitEffect);
					break;
				case "Sand":
					SpawnDecal(hit, sandHitEffect);
					break;
				case "Stone":
					SpawnDecal(hit, stoneHitEffect);
					break;
				case "WaterFilled":
					SpawnDecal(hit, waterLeakEffect);
					SpawnDecal(hit, metalHitEffect);
					break;
				case "Wood":
					SpawnDecal(hit, woodHitEffect);
					break;
				case "Meat":
					SpawnDecal(hit, fleshHitEffects[Random.Range(0, fleshHitEffects.Length)]);
					break;
				case "test001":
					SpawnDecal(hit, fleshHitEffects[Random.Range(0, fleshHitEffects.Length)]);
					break;
				case "WaterFilledExtinguish":
					SpawnDecal(hit, waterLeakExtinguishEffect);
					SpawnDecal(hit, metalHitEffect);
					break;
			}
		}
	}

	void SpawnDecal(RaycastHit hit, GameObject prefab)
	{
		GameObject spawnedDecal = GameObject.Instantiate(prefab, hit.point, Quaternion.LookRotation(hit.normal));
		spawnedDecal.transform.SetParent(hit.collider.transform);
	}


	void AimLock()
    {
		if (isAimed && Input.GetButtonUp("Fire2"))
		{
			mainCamera.SetActive(true);
			aimCamera.SetActive(false);
			isAimed = false;
			return;
		}

		if (Input.GetButtonUp("Fire2"))
		{
			mainCamera.SetActive(false);
			aimCamera.SetActive(true);
			isAimed = true;

		}
	}
}
