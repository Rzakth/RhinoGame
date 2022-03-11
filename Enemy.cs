using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float FireRate = 2.75f;
    public GameObject BulletPrefab;
    public Transform BulletPosition;
    public AudioClip ShootingAudio;
    public GameObject ShootingVFX;
    public Slider HealthBar;
    float nextFire;
    [HideInInspector]
    public int Health = 100;

    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;

    private void OnTriggerStay(Collider other)
    {
        //If enemy senses player, look at the player
        if(other.tag == "Player")
        {
            transform.LookAt(other.transform);
            Fire();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            var bullet = collision.gameObject.GetComponent<Bullet>();
            TakeDamage(bullet.Damage);
        }
    }
    
    private void TakeDamage(int damage)
    {
        Health -= damage;
        HealthBar.value = Health;
        if (Health <= 0)
            EnemyDied();
    }

    private void EnemyDied()
    {
        gameObject.SetActive(false);
        if (OnEnemyKilled != null)
            OnEnemyKilled.Invoke();
    }

    //Firing the bullet
    public void Fire()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + FireRate;
            GameObject bullet = Instantiate(BulletPrefab, BulletPosition.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().InitializeBullet(transform.rotation * Vector3.forward);

            //call audio from AudioManager script
            AudioManager.Instance.Play3D(ShootingAudio, BulletPosition.position);
            //call VFX
            VFXManager.Instance.Play(ShootingVFX, BulletPosition.position);
        }
    }


}
