using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManger : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject PauseMenu;
    public PlayerController playerData;

    public Image healthBar;
    public TextMeshProUGUI clipCounter;
    public TextMeshProUGUI ammoCounter;
    // Start is called before the first frame update
    void Start()
    {
        playerData = GameObject.Find("player").GetComponent<PlayerController>();


    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = Mathf.Clamp((float)playerData.health / (float)playerData.maxHealth, 0, 1);

        clipCounter.text = "Mags: " + playerData.currentClip + "/" + playerData.clipSize;
         if (playerData.weaponID > 0 )
        {
            clipCounter.gameObject.SetActive(false);

        } else
        {
            clipCounter.gameObject.SetActive(true);
            clipCounter.text = "Mags: " + playerData.currentClip + "/" + playerData.clipSize;

            ammoCounter.gameObject.SetActive(true);
            ammoCounter.text = "Ammo: " + playerData.currentAmmo;

            if (!isPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                isPaused = true;

                PauseMenu.SetActive(true);

                Time.timeScale = 0;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (isPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                Resume();
            }

        }
    }
    public void Resume()
    {
        isPaused = false;

        PauseMenu.SetActive(false);

        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
    public void LoadLevel(int sceneID)
    {
        SceneManager.LoadScene(sceneID);

    }
}
