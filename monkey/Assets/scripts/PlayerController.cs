using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Cursor = UnityEngine.Cursor;

public class PlayerController : MonoBehaviour
{
    public Rigidbody myRB;
    public Camera playerCam;
    public GameManger gm;

    Transform cameraHolder;


    [Header("Weapon Stats")]
    public Transform weaponSlot;
    public GameObject shot;
    public bool canFire = true;
    public float fireRate = 0;
    public float shotVel = 0;
    public int weaponID = -1;
    public int fireMode = 0;
    public float currentClip = 0;
    public float clipSize = 0;
    public float maxAmmo = 0;
    public float currentAmmo = 0;
    public float reloadAmt = 0;
    public float bulletLifeSpan = 0;
    public int ammoPickUp = 0;
    public float BulletAngle = 90;
    public bool isReloading = false;


    [Header("Player Stats")]
    public bool takenDamage = false;
    public float damageCoolDown = 0.5f;
    public int maxHealth = 5;
    public int health = 10;
    public int healthPickUp = 5;

    Vector2 camRotation;

    [Header("Movement Stats")]
    public bool sprinting = false;
    public float speed = 10f;
    public float wallRunSpeed = 10f;
    public float jumpHeight = 5f;
    public float groundDetection = 1.0f;
    public float sprintMult = 1.25f;
    public bool crouching = false;
    public float playerHeight = 1f;
    public float crouchSpeed = 5f;
    public bool jumpOn = false;
    

    [Header("Settings")]
    public bool sprintToggle = false;
    public bool crouchingToggle = false;
    public float mouseSensitivity = 2.0f;
    public float Xsensitivity = 2.0f;
    public float Ysensitivity = 2.0f;

    public float camRotationLimit = 90f;


    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        cameraHolder = transform.GetChild(0);

        camRotation = Vector2.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        gm = GameObject.Find("gameManager").GetComponent<GameManger>();

    }

    // Update is called once per frame
    void Update()
    {

        if (!gm.isPaused)
        {


            if (health <= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            playerHeight = transform.localScale.y;

            camRotation.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            camRotation.y += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

            camRotation.y = Mathf.Clamp(camRotation.y, -camRotationLimit, camRotationLimit);
            playerCam.transform.position = cameraHolder.position;

            playerCam.transform.localRotation = Quaternion.Euler(-camRotation.y, camRotation.x, 0);
            transform.localRotation = Quaternion.AngleAxis(camRotation.x, Vector3.up);

            if (Input.GetMouseButton(0) && canFire && currentClip > 0 && weaponID >= 0)
            {
                GameObject s = Instantiate(shot, weaponSlot.transform.parent.position + (weaponSlot.transform.parent.forward * 1f), weaponSlot.transform.parent.rotation);
                s.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * shotVel);
                Destroy(s, bulletLifeSpan);

                canFire = false;
                currentClip--;
                StartCoroutine("cooldownFire");
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                reloadClip();
            }

            if (!sprinting)
            {
                if ((!sprintToggle && Input.GetKey(KeyCode.LeftShift)) || (sprintToggle && (Input.GetAxisRaw("vertical") > 0) && Input.GetKey(KeyCode.LeftShift)))
                {
                    sprinting = true;
                }
            }



            Vector3 temp = myRB.velocity;

            temp.x = Input.GetAxisRaw("Horizontal") * speed;
            temp.z = Input.GetAxisRaw("Vertical") * speed;

            if (sprinting)
            {
                temp.z *= sprintMult;

            }

            if (sprinting && !sprintToggle && Input.GetKeyUp(KeyCode.LeftShift))
            {
                sprinting = false;
            }

            if (Physics.Raycast(transform.position, -transform.up, groundDetection))
            {
                jumpOn = true;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {


                if (jumpOn)
                {
                    jumpOn = false;
                    temp.y = jumpHeight;

                }

            }


            if (crouching)
                myRB.velocity = (transform.forward * temp.z * crouchSpeed) + (transform.right * temp.x * crouchSpeed) + (transform.up * temp.y);
            else
                myRB.velocity = (transform.forward * temp.z) + (transform.right * temp.x) + (transform.up * temp.y);

            if (Input.GetKey(KeyCode.LeftControl) || (Input.GetKey(KeyCode.C)))
            {
                if (transform.localScale.y > 0.5f)
                {
                    transform.localScale -= Vector3.up * Time.deltaTime;

                    crouching = true;
                }
                else transform.localScale = Vector3.one - (Vector3.up * 0.5f);
            }
            else
            {
                if (transform.localScale.y < 1)
                {
                    transform.localScale += Vector3.up * Time.deltaTime;

                    crouching = false;
                }
                else transform.localScale = Vector3.one;
            }

        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "HPU") && health < maxHealth)
        {
            if (health + healthPickUp > maxHealth) 
            {
                 
                health = maxHealth;

            } else
            {
                health += healthPickUp;
            }

            Destroy(collision.gameObject);
        }

        if ((collision.gameObject.tag == "APU") && currentAmmo < maxAmmo)
        {
            if (currentAmmo + ammoPickUp > maxAmmo)
            {
                currentAmmo = maxAmmo;
            }
            else
            {
                currentAmmo += ammoPickUp;
            }

            Destroy(collision.gameObject);
        }

       
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "weapon")
        {
            other.transform.position = weaponSlot.position;
            other.transform.rotation = weaponSlot.rotation;


            other.transform.SetParent(weaponSlot);

            switch(other.gameObject.name) 
            {
                case "Rifle":
                
                    weaponID = 0;
                    shotVel = 4500;
                    fireMode = 0;
                    fireRate = 0.5f;
                    currentClip = 20;
                    clipSize = 20;
                    maxAmmo = 400;
                    currentAmmo = 200;
                    reloadAmt = 20;
                    bulletLifeSpan = 5f;
                    ammoPickUp = 20;
                    break;

                default:
                    break;
            }
        }
    }

    IEnumerator cooldownFire()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    public IEnumerator coolDownDamage()
    {
        yield return new WaitForSeconds(damageCoolDown);
        takenDamage = false;
    }



    public void reloadClip()
    {
        if (isReloading || currentClip >= clipSize)
        {
            return;
        }

        isReloading = true;
        canFire = false;

        float reloadCount = clipSize - currentClip;

        if (currentAmmo < reloadCount)
        {
            currentClip += currentAmmo; // Reload with whatever is left
            currentAmmo = 0; // Clear current ammo
        }
        else
        {
            currentClip += reloadCount; // Fill the clip
            currentAmmo -= reloadCount; // Reduce current ammo
        }


        // Simulate a reload time (for example, 1 second)
        StartCoroutine(ReloadCooldown());
    }

    private IEnumerator ReloadCooldown()
    {
        yield return new WaitForSeconds(1f); // Adjust this duration as needed
        isReloading = false; // Reset reloading state
        canFire = true; // Enable firing again
    }


    // Example of firing method
    public void Fire()
    {
        if (!canFire) return; // Prevent firing if reloading

        if (currentClip > 0)
        {
            currentClip--; // Decrease the ammo in the clip
        }
        // Firing logic here
    }

    
    
}