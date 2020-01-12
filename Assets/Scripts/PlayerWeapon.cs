using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerWeapon : MonoBehaviourPun
{
    [Header("Stats")]
    public int damage;

    public int curAmmo;
    public int maxAmmo;
    public float bulletSpeed;
    public float shootRate;

    private float lastShootTime;

    public GameObject bulletPrefab;
    public Transform bulletSpawnPos;

    private PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public void TryShoot()
    {
        if (curAmmo <= 0 || Time.time - lastShootTime < shootRate)
            return;

        curAmmo--;
        lastShootTime = Time.time;

        // update ammo ui

        // spawn the bullet
        player.photonView.RPC("SpawnBullet", RpcTarget.All, bulletSpawnPos.position, Camera.main.transform.forward);
    }

    [PunRPC]
    public void SpawnBullet(Vector3 position, Vector3 direction)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, position, Quaternion.identity);
        bulletObj.transform.forward = direction;

        Bullet bullet = bulletObj.GetComponent<Bullet>();

        bullet.Initialize(damage, player.id, player.photonView.IsMine);
        bullet.rig.velocity = direction * bulletSpeed;
    }
}
