using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    public AudioClip ShootingAudio;
    public float MoveSpeed = 5f;
    public float FireRate = 0.75f;
    public GameObject BulletPrefab;
    public Transform BulletPosition;
    public GameObject ShootingVFX;
    public Slider HealthBar;
    [HideInInspector]
    public int Health = 100;
    float nextFire;
    Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        //getting rigidbody for the player (rhino)
        rigidbody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Always update player movement
        PlayerMovement();

        //If space key is clicked, update the fire function
        if (Input.GetKey(KeyCode.Space))
            Fire();
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EnemyBullet")
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
            PlayerDied();
    }

    private void PlayerDied()
    {
        gameObject.SetActive(false);
    }
    

    //Player movement
    private void PlayerMovement()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            return;

        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        var rotation = Quaternion.LookRotation(new Vector3(horizontalInput, 0, verticalInput));
        transform.rotation = rotation;

        Vector3 movementDir = transform.forward * MoveSpeed * Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + movementDir);
    }

    //Firing the bullet
    public void Fire()
    {
        if(Time.time > nextFire)
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
