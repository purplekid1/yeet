using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    Rigidbody myRB;
    Camera playerCam;

    Transform cameraHolder;

    Vector2 camRotation;

    [Header("Player Stats")]
    public bool takenDamage = false;
    public float damangeCooldownTimer = .5f;
    public int health = 5;
    public int maxHealth = 10;
    public int healtPickupAmt = 5;

    [Header("Weapon Stats")]
    public AudioSource weaponSpeaker;
    public Transform weaponSlot;
    public GameObject shot;
    public float shotVel = 0;
    public int weaponID = -1;
    public int fireMode = 0;
    public float fireRate = 0;
    public float currentClip = 0;
    public float clipSize = 0;
    public float maxAmmo = 0;
    public float currentAmmo = 0;
    public float reloadAmt = 0;
    public float bulletLifespan = 0;
    public bool canFire = true;


    [Header("Movement Stats")]
    public bool sprinting = false;
    public float speed = 10f;
    public float sprintMult = 1.5f;
    public float jumpHeight = 5f;
    public float groundDetection = 1f;

    [Header("User Settings")]
    public bool sprintToggle = false;
    public float mouseSensitivity = 2.0f;
    public float Xsensitivity = 2.0f;
    public float Ysensitivity = 2.0f;
    public float camRotationLimit = 90f;

    // Start is called before the first frame update
    void Start()
    {
        // Initialized components
        myRB = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        cameraHolder = transform.GetChild(0);

        // Camera setup
        camRotation = Vector2.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.isPaused)
        {
            if (health <= 0)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            // FPS Camera Rotation
            camRotation.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            camRotation.y += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

            // Limit vertical rotation
            camRotation.y = Mathf.Clamp(camRotation.y, -camRotationLimit, camRotationLimit);

            playerCam.transform.position = cameraHolder.position;

            // Set camera rotation on the vertical axis | Set player rotation on horizontal axis
            playerCam.transform.rotation = Quaternion.Euler(-camRotation.y, camRotation.x, 0);
            transform.localRotation = Quaternion.AngleAxis(camRotation.x, Vector3.up);




            // Sprint turn on for toggle & not toggle
            if ((!sprinting) && ((!sprintToggle && Input.GetKey(KeyCode.LeftShift)) || (sprintToggle && Input.GetKey(KeyCode.LeftShift) && (Input.GetAxisRaw("Vertical") > 0))))
                sprinting = true;


            // Movement math calculation velocity measured by input * speed
            Vector3 temp = myRB.velocity;

            temp.x = Input.GetAxisRaw("Horizontal") * speed;
            temp.z = Input.GetAxisRaw("Vertical") * speed;

            // If sprinting, check to see if disable condition flags (also amplify speed if sprinting)
            if (sprinting)
            {
                temp.z *= sprintMult;

                if ((sprintToggle && (Input.GetAxisRaw("Vertical") <= 0)) || (!sprintToggle && Input.GetKeyUp(KeyCode.LeftShift)))
                    sprinting = false;
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, -transform.up, groundDetection))
                temp.y = jumpHeight;

            // Give calculated velocity back to rigidbody
            myRB.velocity = (transform.forward * temp.z) + (transform.right * temp.x) + (transform.up * temp.y);

        }
    }




