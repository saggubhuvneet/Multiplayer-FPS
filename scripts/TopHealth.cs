//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using Photon.Pun;


//public class TopHealth : MonoBehaviour
//{
//    public SampleRun playerHealth;
//    public Image healthUp;
//    //private float health;


//    PhotonView PV;

//    private void Start()
//    {
//        PV = GetComponent<PhotonView>();
//    }

//    private void Update()
//    {
//        if (!PV.IsMine)
//        {
//            playerHealth.healthImage.fillAmount = healthUp.fillAmount;
//        }
//        if (PV.IsMine)
//        {
//            healthUp.fillAmount = playerHealth.healthImage.fillAmount;
//        }
//    }

//    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
//    {
//        if (stream.IsWriting)
//        {
//            stream.SendNext(healthUp.fillAmount);
//        }
//        else if (stream.IsReading)
//        {
//            healthUp.fillAmount = (float)stream.ReceiveNext();
//        }
//    }
//}
