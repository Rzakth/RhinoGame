using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
public class Multiplayer : MonoBehaviour, IPunObservable
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
    PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        //getting rigidbody for the player (rhino)
        rigidbody = GetComponent<Rigidbody>();

        //getting player position
        photonView = GetComponent<PhotonView>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        //Always update player movement
        PlayerMovement();

        //If space key is clicked, update the fire function
        if (Input.GetKey(KeyCode.Space))
            photonView.RPC("Fire", RpcTarget.AllViaServer);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "MultiplayerBullet")
        {
            var bullet = collision.gameObject.GetComponent<MultiplayerBullet>();
            TakeDamage(bullet);
        }
    }

    private void TakeDamage(MultiplayerBullet bullet)
    {
        Health -= bullet.Damage;
        HealthBar.value = Health;
        if (Health <= 0)
        {
            bullet.Owner.AddScore(1);
            PlayerDied();
        }
    }

    private void PlayerDied()
    {

        Health = 100;
        HealthBar.value = Health;
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
    [PunRPC]
    public void Fire()
    {
        if(Time.time > nextFire)
        {
            nextFire = Time.time + FireRate;
            GameObject bullet = Instantiate(BulletPrefab, BulletPosition.position, Quaternion.identity);
            bullet.GetComponent<MultiplayerBullet>().InitializeBullet(transform.rotation * Vector3.forward, photonView.Owner);

            //call audio from AudioManager script
            AudioManager.Instance.Play3D(ShootingAudio, BulletPosition.position);
            //call VFX
            VFXManager.Instance.Play(ShootingVFX, BulletPosition.position);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
      if(stream.IsWriting)
        {
            stream.SendNext(Health);
        }
      else
        {
            Health = (int)stream.ReceiveNext();
            HealthBar.value = Health;
        }
    }
}
