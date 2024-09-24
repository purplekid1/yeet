using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerController : MonoBehaviour
{
    public Rigidbody myRB;
    public Camera playerCam;


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
    public bool isReloading = false;
    public BulletCounter bulletCounter;


    [Header("Player Stats")]
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
        playerCam = transform.GetChild(0).GetComponent<Camera>();

        camRotation = Vector2.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        bulletCounter.Initialize((int)currentAmmo);

    }

    // Update is called once per frame
    void Update()
    {

        playerHeight = transform.localScale.y;

        camRotation.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        camRotation.y += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        camRotation.y = Mathf.Clamp(camRotation.y, -camRotationLimit, camRotationLimit);


        playerCam.transform.localRotation = Quaternion.AngleAxis(camRotation.y, Vector3.left);
        transform.localRotation = Quaternion.AngleAxis(camRotation.x, Vector3.up);

        if (Input.GetMouseButton(0) && canFire && currentClip >  0 && weaponID >= 0)
        {
            GameObject s = Instantiate(shot, weaponSlot.position, weaponSlot.rotation);
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
                case "Weapon1":
                
                    weaponID = 0;
                    shotVel = 10000;
                    fireMode = 0;
                    fireRate = 0.1f;
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



    public void reloadClip()
    {
        // Check if already reloading or if the clip is full
        if (isReloading || currentClip >= clipSize)
        {
            return;
        }

        isReloading = true; // Set reloading state
        canFire = false; // Disable firing while reloading

        // Calculate how much ammo to reload
        float reloadCount = clipSize - currentClip;

        // Perform the reload
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

        bulletCounter.UpdateAmmoCount((int)currentAmmo);

        // Simulate a reload time (for example, 1 second)
        StartCoroutine(ReloadCooldown());
    }

    private IEnumerator ReloadCooldown()
    {
        yield return new WaitForSeconds(1f); // Adjust this duration as needed
        isReloading = false; // Reset reloading state
        canFire = true; // Enable firing again
    }

    // Call this method when the reload button is pressed
    public void OnReloadButtonPressed()
    {
        reloadClip();
    }

    // Example of firing method
    public void Fire()
    {
        if (!canFire) return; // Prevent firing if reloading

        if (currentClip > 0)
        {
            currentClip--; // Decrease the ammo in the clip
            bulletCounter.UpdateAmmoCount((int)currentAmmo);
        }
        // Firing logic here
    }

    
    
}