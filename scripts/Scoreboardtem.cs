using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using hashtable = ExitGames.Client.Photon.Hashtable;

public class Scoreboardtem : MonoBehaviourPunCallbacks
{
    public TMP_Text username;
    public TMP_Text deathsText;
    public TMP_Text killsText;

    Player player;

    public void Initilize(Player player)
    {
        username.text = player.NickName;
        this.player = player;
        UpdateStats();
    }

    void UpdateStats()
    {
        if(player.CustomProperties.TryGetValue("Kills", out object kills))
        {
            killsText.text = kills.ToString();
        }

        if (player.CustomProperties.TryGetValue("deaths", out object deaths))
        {
            deathsText.text = deaths.ToString();
        }
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, hashtable changedProps)
    {
        if(targetPlayer == player)
        {
            if (changedProps.ContainsKey("Kills") || changedProps.ContainsKey("deaths"))
            {
                UpdateStats();
            }
        }
    }
}
