using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // If using regular UI text
// using TMPro; // If using TextMeshPro

public class BulletCounter : MonoBehaviour
{
    public Text bulletCounterText; // Assign in the Inspector
    // or use TMP_Text if you're using TextMeshPro
    private int currentAmmo; // Track the ammo count

    public void UpdateAmmoCount(int newAmmoCount)
    {
        currentAmmo = newAmmoCount;
        bulletCounterText.text = "Ammo: " + currentAmmo.ToString();
    }

    public void Initialize(int startingAmmo)
    {
        currentAmmo = startingAmmo;
        UpdateAmmoCount(currentAmmo);
    }
}
