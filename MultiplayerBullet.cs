using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerBullet : MonoBehaviour
{
    public AudioClip BulletHitAudio;
    public GameObject BulletHitVFX;
    public int Damage = 5;
    [HideInInspector]
    public Photon.Realtime.Player Owner;

    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    //Initializing bullet
    public void InitializeBullet(Vector3 originalDirection, Photon.Realtime.Player player)
    {
        transform.forward = originalDirection;
        rigidbody.velocity = transform.forward * 18f;
        Owner = player;
    }

    //Bullet collision
    private void OnCollisionEnter(Collision collision)
    {
        //audio
        AudioManager.Instance.Play3D(BulletHitAudio, transform.position);
        //vfx
        VFXManager.Instance.Play(BulletHitVFX, transform.position);
        Destroy(gameObject);
    }
    
}
