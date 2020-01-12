using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public enum PickupType
{
    Health,
    Ammo
}

public class Pickups : MonoBehaviour
{
    public PickupType type;
    public int value;

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = GameManager.instance.GetPlayer(other.gameObject);

            if(type == PickupType.Health)
                player.photonView.RPC("Heal", player.photonPlayer, value);
            else if(type == PickupType.Ammo)
                player.photonView.RPC("GiveAmmo", player.photonPlayer, value);

            // destroy the object
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
