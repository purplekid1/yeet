using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody myRB;
    Camera playerCam;


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


    [Header("Player Stats")]
    public int maxHealth = 5;
    public int health = 10;
    public int healthPickUp = 5;

    Vector2 camRotation;

    [Header("Movement Stats")]
    public bool sprinting = false;
    public float speed = 10f;
    public float jumpHeight = 5f;
    public float groundDetection = 1.0f;
    public float sprintMult = 1.25f;

    [Header("Settings")]
    public bool sprintToggle = false;
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

    }

    // Update is called once per frame
    void Update()
    {
        


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

        if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, -transform.up, groundDetection))
        {
            temp.y = jumpHeight;
        }

        myRB.velocity = (transform.forward * temp.z) + (transform.right * temp.x) + (transform.up * temp.y);
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
        if (currentClip >= clipSize)
        {
            return;
        } 
        else
        {
            float reloadCount = clipSize - currentClip;

            if (currentClip < reloadCount)
            {
                currentClip += currentAmmo;
                currentAmmo = 0;
                return;
            
            }
            else
            {
                currentClip += reloadCount;
                currentAmmo = reloadCount;
                return;
            }
        }
    }   
}