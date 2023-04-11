
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class GunSelection : MonoBehaviourPunCallbacks
{
    public GameObject _carbine, _cal50, _ak_47, _smg;
    public int itemIndex;
    PhotonView PV;

    private void Start()
    {
        itemIndex = Random.Range(0, 4);
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        EquipItem();
    }

    private void EquipItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            itemIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            itemIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            itemIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            itemIndex = 3;
        }

        if (itemIndex == 0)
        {
            Carbine();
        }
        else if (itemIndex == 1)
        {
            Cal50();
        }
        else if(itemIndex == 2)
        {
            AK_47();
        }
        else if(itemIndex == 3)
        {
            SMG();
        }

        Hashtable hash = new Hashtable();
        hash.Add("itemIndex", itemIndex);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (PV != null && !PV.IsMine && targetPlayer == PV.Owner && changedProps.ContainsKey("itemIndex"))
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    public void EquipItem(int _index)
    {
        itemIndex = _index;

        if (itemIndex == 0)
        {
            Carbine();
        }
        else if (itemIndex == 1)
        {
            Cal50();
        }
        else if(itemIndex == 2)
        {
            AK_47();
        }
        else if(itemIndex == 3)
        {
            SMG();
        }
    }

    public void Carbine()
    {
        _carbine.SetActive(true);
        _cal50.SetActive(false);
        _ak_47.SetActive(false);
        _smg.SetActive(false);

    }

    public void Cal50()
    {
        _carbine.SetActive(false);
        _cal50.SetActive(true);
        _ak_47.SetActive(false);
        _smg.SetActive(false);

    }
    public void AK_47()
    {
        _carbine.SetActive(false);
        _cal50.SetActive(false);
        _ak_47.SetActive(true);
        _smg.SetActive(false);
    }
    public void SMG()
    {  
        _carbine.SetActive(false);
        _cal50.SetActive(false);
        _ak_47.SetActive(false);
        _smg.SetActive(true);
    }

}