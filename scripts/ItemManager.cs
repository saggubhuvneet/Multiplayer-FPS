using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector] public int itemIndex;

    public GameObject _carbine;
    public GameObject _cal_50;

    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        itemIndex = Random.Range(0, 2);
    }
    private void Update()
    {
        if (!PV.IsMine)
            return;

        if (itemIndex == 0)Carbine();

        if (itemIndex == 1)Cal_50();
    }

    public void Carbine()
    {
        _carbine.SetActive(true);_cal_50.SetActive(false);
    }
    public void Cal_50()
    {
        _carbine.SetActive(false); _cal_50.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(itemIndex);
        }
        else
        {
            itemIndex = (int)stream.ReceiveNext();
        }
    }
}
