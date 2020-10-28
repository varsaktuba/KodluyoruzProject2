using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] float moveSpeed = 10f; // hız
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 0.1f;
    [SerializeField] int health = 200;

    Coroutine firingCoroutine;

    float xMin;
    float xMax;
    float yMin;
    float yMax;
   
    void Start()
    {
        
        SetUpMoveBoundaries();
    }

    private void SetUpMoveBoundaries()
    {
        // Kameranın sınırlarını belirle
        Camera myCamera = Camera.main;
        xMin = myCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x; //0,0
        xMax = myCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x; //1,0
        yMin = myCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y; //0,0
        yMax = myCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y; //0,1

    }

    void Update()
    {
        Move();
        Fire();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
          firingCoroutine = StartCoroutine(FireContinuously()); // space'e basılı tutunca laser atmaya devam et
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine); // spaceden kaldırınca atmayı durdur
        }
    }
    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laser = Instantiate(
                   laserPrefab, transform.position, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            yield return new WaitForSeconds(projectileFiringPeriod); 
        }
    }
    private void Move()
    {
        //Time.deltaTime = frame rate independent (her bilgisayarda eşit deneyim olması için kullandık)

        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed; // yatay yönde hareket için pozisyon belirleme
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed; // dikey yönde hareket için pozisyon belirleme

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax); // Player yatay sınırlar dışına çıkamaz
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);// Player dikey sınırlar dışına çıkamaz

        transform.position = new Vector2(newXPos, newYPos);
    }
}
