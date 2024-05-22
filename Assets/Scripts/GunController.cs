using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireMode
{
    Semi,
    Auto,
    Burst,
}

public class GunController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int ammo = 30;
    [SerializeField] private int ammoCapacity = 30;
    [SerializeField] private float timeBetweenShot = 0.1f;
    [SerializeField] private FireMode fireMode = FireMode.Auto;

    [Header("Bullet")]
    public float BulletSpeed = 10f;

    [Header("Reload")]
    public float ReloadTime = 3f;

    [Header("Status")]
    [SerializeField] private bool canFire;

    [Header("Positions")]
    public Transform FirePosition;
    public GameObject Projectile;

    // [Header("Particle Systems")]
    // public ParticleSystem FireSparks;

    private void Start()
    {
        canFire = true;
    }

    private void Update()
    {

    }

    public void Fire()
    {
        if (!canFire)
            return;

        // firing
        if (ammo > 0)
        {
            ammo--;
            FireBullet();
            StartCoroutine(StopFire(timeBetweenShot));
        }
        else
        {
            Reload();
        }
    }

    private void FireBullet()
    {
        GameObject bullet = Instantiate(Projectile, FirePosition.position, FirePosition.rotation);
        bullet.GetComponent<Bullet>().Direction = FirePosition.right;
    }

    private void FireProjectile()
    {
        
    }

    public void Reload()
    {
        ammo = ammoCapacity;
        StopFire(ReloadTime);
    }

    private IEnumerator StopFire(float time)
    {
        canFire = false;
        yield return new WaitForSeconds(time);
        canFire = true;
    }
}
