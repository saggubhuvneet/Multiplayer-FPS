using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.IO;

public class SampleRun : MonoBehaviourPunCallbacks, IPunObservable, IDamageable
{
	[Header("MOVEMENT")]
	[SerializeField] float sptintSpeed;
	[SerializeField] float walkSpeed;
	[SerializeField] float smoothtime;
	[SerializeField] Animator anim;

	[Header("Look")]
	[SerializeField] float mouseSensivity;
	[SerializeField] GameObject cameraHolder;
	
	//------------------------------------character controller 

	[SerializeField] float gravity = -9.8f;
    [SerializeField] float jumpForce = 5;
	[SerializeField] GameObject GumMesh;
	[SerializeField] GameObject deathCamera;
	[SerializeField] GameObject handIK;
	[SerializeField] GameObject ui;

	[Header("Gun info")]
	[SerializeField] GameObject _gunPickInfo_UI;
	[SerializeField] GameObject _carbine_Info;
	[SerializeField] GameObject _cal50_info;
	[SerializeField] GameObject _smg9_info;
	[SerializeField] GameObject _ak47_info;

	//public GameObject Ak45Pickup;
	CharacterController characterController;
	float verticalLookRotation;
	bool isAimed;
	bool isDead = false;
	
	private Vector3 playerInput;
	private Vector3 velocity;

	//---------------------------------------- Health
	public Image healthImage;
	public Image top_healthimage;
	[SerializeField] TextMeshProUGUI healthTExt;
	const float maxHealth = 300f;
	float currentHealth = maxHealth;
	PlayerManager playerManager;
	public GunSelection _gunSelection;

	PhotonView PV;

	private void Awake()
    {
		PV = GetComponent<PhotonView>();
       
		characterController = GetComponent<CharacterController>();
		isAimed = false;
		isDead = false;
		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();

	}

    private void Start()
    {
        if (!PV.IsMine)
        {
			Destroy(ui);
			Destroy(cameraHolder);
	    }
		//Cursor.lockState = CursorLockMode.;
	}
    private void Update()
    {
		if (!PV.IsMine)
			return;
		if (isDead == true)
			return;
		Move();
		Look();
		Aim();
		
	}

    
    void Move()
	{

		playerInput = Vector3.zero;
		playerInput += transform.forward * Input.GetAxis("Vertical");
		playerInput += transform.right * Input.GetAxis("Horizontal");

        if (characterController.isGrounded)//----------  J U M P
        {
			velocity.y = -1f;
            if (Input.GetButton("Jump"))
            {
                velocity.y = jumpForce;
				anim.SetTrigger("jump");
               
            }
        }

		velocity.y += gravity * Time.deltaTime;
		characterController.Move(playerInput * walkSpeed * Time.deltaTime);
		characterController.Move(velocity * Time.deltaTime);

		
		anim.SetFloat("X", Input.GetAxis("Horizontal"));
        anim.SetFloat("Y", Input.GetAxis("Vertical"));

	}

    void Look()
	{
		transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensivity);
		//verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensivity;
		//verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60f, 30f);
		cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;


		anim.SetFloat("AimVertical", Input.mousePosition.y);

	}

	void Aim()
    {
		if (isAimed && Input.GetButtonUp("Fire2"))
        {
			anim.SetBool("aim", false);
			isAimed = false;
			return;
		}

		if (Input.GetButtonUp("Fire2")) 
		{
			anim.SetBool("aim", true);
			Debug.Log("aimed");
			isAimed = true;
		}
		
	}

	public void TakeDamage(float damage)
    {
		PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

	[PunRPC]
	public void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
		if (isDead == true)
			return;
        currentHealth -= damage; 

		healthImage.fillAmount = currentHealth / maxHealth;

		top_healthimage.fillAmount = currentHealth / maxHealth;

		healthTExt.text = currentHealth.ToString();

		if (currentHealth <= 0)
		{
			StartCoroutine(Die());
			PlayerManager.Find(info.Sender).GetKill();

		}


	}
	IEnumerator Die()
    {
		isDead = true;
		Debug.Log("player deeeeead");
		anim.SetTrigger("Dead");
		GumMesh.SetActive(false);
		deathCamera.SetActive(true);
	
		yield return new WaitForSeconds(4f);
		GunDrop();
		playerManager.PlayerDie();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(top_healthimage.fillAmount);
        }
        else
        {
            top_healthimage.fillAmount = (float)stream.ReceiveNext();
        }
    }

	// Gun Droping system;
	public void GunDrop()
    {
		//PhotonNetwork.Instantiate(Path.Combine("GunPickUps", "Ak47Pick"), transform.position, Quaternion.identity, 0, new object[] { PV.ViewID });
		Debug.Log("A gun is dropes");
		if(_gunSelection.itemIndex == 0){
			PhotonNetwork.Instantiate(Path.Combine("GunPickUps", "CarbinePick"), transform.position, Quaternion.identity, 0, new object[] { PV.ViewID });
		}if(_gunSelection.itemIndex == 1){
			PhotonNetwork.Instantiate(Path.Combine("GunPickUps", "Cal50Pick"), transform.position, Quaternion.identity, 0, new object[] { PV.ViewID });
		}if(_gunSelection.itemIndex == 2){
			PhotonNetwork.Instantiate(Path.Combine("GunPickUps", "Ak47Pick"), transform.position, Quaternion.identity, 0, new object[] { PV.ViewID });
		}if(_gunSelection.itemIndex == 3){
			PhotonNetwork.Instantiate(Path.Combine("GunPickUps", "SMGPick"), transform.position, Quaternion.identity, 0, new object[] { PV.ViewID });
		}
	}
	// gun Picking  UI system
    public void OnTriggerExit(Collider other)
    {
		
        if(other.gameObject.CompareTag("_carbine_") || other.gameObject.CompareTag("_cal_") || other.gameObject.CompareTag("_ak_") || other.gameObject.CompareTag("_smg_"))
        {
			_carbine_Info.SetActive(false);_cal50_info.SetActive(false);_smg9_info.SetActive(false);_ak47_info.SetActive(false);
			_gunPickInfo_UI.GetComponent<Animator>().Play("idle");
		}
	}

    public void OnTriggerStay(Collider other)
    {
		if (!PV.IsMine)
			return;
		Debug.Log("Pick the gun up you idiot");
		if (other.gameObject.CompareTag("_carbine_")) {
			_gunPickInfo_UI.GetComponent<Animator>().Play("play");
			_carbine_Info.SetActive(true);
            if (Input.GetKeyDown(KeyCode.P)){
				_gunSelection.itemIndex = 0;
                _carbine_Info.SetActive(false);
                _gunPickInfo_UI.GetComponent<Animator>().Play("idle");
				//PhotonNetwork.Destroy(other.gameObject);

				int viewID = other.gameObject.GetComponent<PhotonView>().ViewID;
				PV.RPC("DestroyGunPickups", RpcTarget.MasterClient, viewID);
            }
        }
        if (other.gameObject.CompareTag("_cal_"))
        {
            _gunPickInfo_UI.GetComponent<Animator>().Play("play");
            _cal50_info.SetActive(true);
            if (Input.GetKeyDown(KeyCode.P))
            {
                _gunSelection.itemIndex = 1;
                _cal50_info.SetActive(false);
                _gunPickInfo_UI.GetComponent<Animator>().Play("idle");
				//PhotonNetwork.Destroy(other.gameObject);

				int viewID = other.gameObject.GetComponent<PhotonView>().ViewID;
				PV.RPC("DestroyGunPickups", RpcTarget.MasterClient, viewID);
			}
        }
        if (other.gameObject.CompareTag("_ak_"))
        {
            _gunPickInfo_UI.GetComponent<Animator>().Play("play");
            _ak47_info.SetActive(true);
            if (Input.GetKeyDown(KeyCode.P))
            {
                _gunSelection.itemIndex = 2;
                _ak47_info.SetActive(false);
                _gunPickInfo_UI.GetComponent<Animator>().Play("idle");
				//PhotonNetwork.Destroy(other.gameObject);

				int viewID = other.gameObject.GetComponent<PhotonView>().ViewID;
				PV.RPC("DestroyGunPickups", RpcTarget.MasterClient, viewID);

			}
        }
        if (other.gameObject.CompareTag("_smg_"))
        {
            _gunPickInfo_UI.GetComponent<Animator>().Play("play");
            _smg9_info.SetActive(true);
            if (Input.GetKeyDown(KeyCode.P))
            {
                _gunSelection.itemIndex = 3;
                _smg9_info.SetActive(false);
                _gunPickInfo_UI.GetComponent<Animator>().Play("idle");
				//PhotonNetwork.Destroy(other.gameObject);

				int viewID = other.gameObject.GetComponent<PhotonView>().ViewID;
				PV.RPC("DestroyGunPickups", RpcTarget.MasterClient, viewID);
			}
        }
		
	}

	//Gun pick Up destroy
	[PunRPC]
	public void DestroyGunPickups(int viewID)
    {
		PhotonNetwork.Destroy(PhotonView.Find(viewID).gameObject);
    }
	
}
