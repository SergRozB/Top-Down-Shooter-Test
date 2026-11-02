using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerFunctions : MonoBehaviour
{
    public float health = 20f;

    public TMPro.TextMeshProUGUI healthText;
    public TMPro.TextMeshProUGUI currentAmmoText;
    public TMPro.TextMeshProUGUI magazineText;

    private WeaponScript weaponScript;

    public GameObject panel;
    public GameObject gameOverMenu;

    public Sprite playerGlock;
    public Sprite playerM4;

    private Animator animator;


    private void Start()
    {
        weaponScript = gameObject.GetComponent<WeaponScript>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        TextManager();
        GameOver();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    public GameObject GetChildWithTag(GameObject parent, string tag) //for finding ONE gameobject with a unique tag
    {
        foreach (Transform tr in parent.transform)
        {
            if (tr.tag == tag)
            {
                return tr.gameObject;
            }

            foreach (Transform tr2 in tr)
            {
                if (tr2.tag == tag)
                {
                    return tr2.gameObject;
                }
            }
        }
        return null;
    }

    public void playerSpriteManager(string weapon)
    {
        SpriteRenderer spriteRenderer = GetChildWithTag(gameObject, "PlayerBody").GetComponent<SpriteRenderer>();
        switch (weapon)
        {
            case "NoWeapon":
                {
                    animator.SetInteger("Weapon", 0);
                    Debug.Log("dis bich1");
                    break;
                }

            case "Glock":
                {
                    animator.SetInteger("Weapon", 1);
                    Sprite glockSprite = playerGlock;
                    spriteRenderer.sprite = glockSprite;
                    Debug.Log("dis bich2");
                    break;
                }

            case "M4":
                {
                    animator.SetInteger("Weapon", 2);
                    Sprite m4Sprite = playerM4;
                    spriteRenderer.sprite = m4Sprite;
                    Debug.Log("dis bich3");
                    break;
                }
        }
    }

    public void TextManager()
    {
        healthText.text = "Health: " + health;

        if (weaponScript.gun != null)
        {
            currentAmmoText.gameObject.SetActive(true);
            magazineText.gameObject.SetActive(true);
            int currentAmmo = weaponScript.currentGunAmmo;
            int mags = weaponScript.mags;
            currentAmmoText.text = currentAmmo.ToString();
            magazineText.text = mags.ToString();
        }

        else
        {
            currentAmmoText.gameObject.SetActive(false);
            magazineText.gameObject.SetActive(false);
        }
    }

    private void GameOver()
    {
        if (health <= 0)
        {
            panel.SetActive(true);
            gameOverMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
