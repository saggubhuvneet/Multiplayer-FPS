using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class RoomListItem : MonoBehaviour
{
    public static RoomListItem Instance;
    private void Awake()
    {
        Instance = this;
    }
    public bool isPos4anim = false;
    [SerializeField] TMP_Text text;
    public RoomInfo info;

    public void Setup(RoomInfo _info)
    {
        info = _info;
        text.text = _info.Name;
    }

    public void OnClick()
    {
        PunLauncher.Instance.JoinRoom(info);
        isPos4anim = true;
      
    }
}
