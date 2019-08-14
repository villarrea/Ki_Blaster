using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // configuration params

    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float xPadding = 1f;
    [SerializeField] float yPadding = 1f;
    [SerializeField] int health = 200;
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0,1)] float deathSoundVolume = 0.75f;
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0,1)] float shootSoundVolume = 0.25f;
    [SerializeField] GameObject deathVFX;
    [SerializeField] Sprite playershooting;
    [SerializeField] Sprite player;
    [SerializeField] Sprite dead;

    [SerializeField] float durationOfExplosion = 1f;



    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 0.1f;


    Coroutine firingCoroutine;

    float xMin;
    float xMax;
    float yMin;
    float yMax;

    bool ableToMove = true;
    bool sound = true;


    void Start()
    {
        SetUpMoveBoundaries();
    }

    void Update()
    {
        if (ableToMove == true)
        {
            Move();
            Fire();
        }
    }

    private void Move(){
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);

        transform.position = new Vector2 (newXPos, newYPos);
    }

     private void SetUpMoveBoundaries(){

        Camera gameCamera = Camera.main;

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + xPadding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - xPadding;

        // xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + xPadding;
        // xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - xPadding;
        
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + xPadding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - yPadding;
    }

    private void Fire()
    {
        if(Input.GetButtonDown("Fire1")){
            this.GetComponent<SpriteRenderer>().sprite = playershooting;
            firingCoroutine = StartCoroutine(FireContinuosly());
        }
        if(Input.GetButtonUp("Fire1")){
            this.GetComponent<SpriteRenderer>().sprite = player;
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuosly(){
        while(true){
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileSpeed,0);
            AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void OnTriggerEnter2D (Collider2D other) 
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer> ();

        if(!damageDealer){ return; }

        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage ();

        damageDealer.Hit();
        if (health <= 0) 
        {
            Die();
        }
    }

    public int GetHealth()
    {
        if (health <= 0) 
        {
            return health = 0;
        }
        else
        {
            return health;
        }
    }
    
    private void Die()
    {
        StopCoroutine(firingCoroutine);
        this.GetComponent<SpriteRenderer>().sprite = dead;
        ableToMove = false;
        FindObjectOfType<Level>().LoadGameOver();
        // Destroy (gameObject);
        // GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        // Destroy(explosion, durationOfExplosion);
        if(sound == true)
        {
            AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
        }
        sound = false;
    }
}
