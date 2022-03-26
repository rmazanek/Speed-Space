using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageReaction : MonoBehaviour
{
    [SerializeField] int damageReactionColorFrames = 5;
    [SerializeField] Color damageReactionColor = Color.red;
    [SerializeField] AudioClip damageReactionSound;
    [SerializeField] float damageReactionSoundVolume = 0.05f;
    [SerializeField] bool showDamageNumberPopup = true;
    [SerializeField] float damageNumberPopupDuration = 0.2f;
    [SerializeField] GameObject damageNumberPopupPrefab;
    Color originalColor;
    private void Start()
    {
        originalColor = gameObject.GetComponent<SpriteRenderer>().color;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }
    public void ProcessHit(DamageDealer damageDealer)
    {
        damageDealer.Hit();
        StartCoroutine(DamageVisualEffects());
        if(showDamageNumberPopup)
        {
            DamageNumberPopup(damageDealer);
        }
        if(damageReactionSound != null)
        {
            AudioSource.PlayClipAtPoint(damageReactionSound, Camera.main.transform.position, damageReactionSoundVolume);
        }
    }
    IEnumerator DamageVisualEffects()
    {
        gameObject.GetComponent<SpriteRenderer>().color = damageReactionColor;
        yield return new WaitForSeconds(damageReactionColorFrames * Time.deltaTime);
        gameObject.GetComponent<SpriteRenderer>().color = originalColor;
    }
    public void DamageNumberPopup(DamageDealer damageDealer)
    {
        if(damageNumberPopupPrefab != null)
        {
            Vector3 damageNumberPosition;
    
            if(damageDealer.transform != null)
            {   
                damageNumberPosition = new Vector3(damageDealer.transform.position.x, damageDealer.transform.position.y, -1);
            }
            else
            {
                damageNumberPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1);
            }
            GameObject damageNumberPopup = Instantiate(damageNumberPopupPrefab, damageNumberPosition, Quaternion.identity);
            damageNumberPopup.GetComponent<TextMeshPro>().text = damageDealer.GetDamage().ToString();
            Destroy(damageNumberPopup, damageNumberPopupDuration);
        }
    }
}