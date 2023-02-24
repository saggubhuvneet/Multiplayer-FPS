using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class PlayerNameManager : MonoBehaviour
{

    [SerializeField] TMP_InputField UserNameInput;

    private void Start()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            UserNameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
        }
        else
        {
            UserNameInput.text = "Player" + Random.Range(0, 100).ToString("00");
            OnUserNameEnter();
        }
    }
    public void OnUserNameEnter()
    {
        PhotonNetwork.NickName = UserNameInput.text;
        PlayerPrefs.SetString("username", UserNameInput.text);
    }
}
