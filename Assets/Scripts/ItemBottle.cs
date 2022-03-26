using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemBottle : MonoBehaviour
{
    [Header("Bottle")]
    [SerializeField] float health = 500f;
    [SerializeField] GameObject explosionVfxPrefab;
    [SerializeField] float explosionDuration = 0.3f;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] float explosionSoundVolume = 0.05f;
    [SerializeField] int obstacleScoreValue = 0;
    [SerializeField] GameObject[] haloColors;
    
    [Header("Damage Reaction")]
    [SerializeField] Sprite[] brokenSpriteLevels;
    [SerializeField] int damageReactionColorFrames = 20;
    [SerializeField] Color damageReactionColor;
    [SerializeField] AudioClip damageReactionSound;
    [SerializeField] float damageReactionSoundVolume = 0.02f;
    [Header("Drops")]
    [SerializeField] GameObject[] items;
    [SerializeField] int[] weights;
    [SerializeField] GameObject itemInside;
    [SerializeField] TextMeshPro priceTagDisplay;
    int itemCost;
    float extentsX;
    //Cached refs
    GameSession gameSession;
    Wallet playerWallet;
    float maxHealth = 100f;
    Color originalColor;
    GameObject prefabToInstantiate;
    bool isDestroyed = false;
    TooltipDisplay tooltipDisplay;
    string itemName;
    string tooltipText;
    Sprite tooltipSprite;

    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        maxHealth = health;
        originalColor = gameObject.GetComponent<SpriteRenderer>().color;
        RemoveShipFromPoolIfAtMaxPlayerShips();
        ChooseItem();
        itemInside.GetComponent<SpriteRenderer>().sprite = GetItemSprite();
        itemCost = prefabToInstantiate.GetComponent<Purchasable>().ItemCost;
        priceTagDisplay.text = itemCost.ToString();
        playerWallet = GameObject.FindObjectOfType<Wallet>();
        tooltipDisplay = GameObject.FindObjectOfType<TooltipDisplay>();
        itemName = prefabToInstantiate.GetComponent<Item>().GetItemName();
        tooltipText = prefabToInstantiate.GetComponent<Item>().GetTooltipText();
        tooltipSprite = GetItemSprite();
        AddHalo();
        prefabToInstantiate.GetComponent<Item>().GetTier();
    }
    private void Update()
    {
        if(PlayerIsDetected())
        {
            UpdateTooltipText();
        }   
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
        StartCoroutine(DamageReaction());
        AudioSource.PlayClipAtPoint(damageReactionSound, Camera.main.transform.position, damageReactionSoundVolume);
        damageReactionSoundVolume /= 1.1f;

        if ((health <= 0) && (playerWallet.GetCoinTotal() >= itemCost) && !isDestroyed)
        {
          isDestroyed = true;
          DestroyEnemy();
          playerWallet.SubtractFromWallet(itemCost);
          gameSession.AddToScore(obstacleScoreValue);
        }
        else
        {
            SpriteChange();
        }
    }

    private void DestroyEnemy()
    {
        GameObject explosionVfx = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionSoundVolume);
        DropItem();
        Destroy(gameObject);
        Destroy(explosionVfx, explosionDuration);
    }
    IEnumerator DamageReaction()
    {
        gameObject.GetComponent<SpriteRenderer>().color = damageReactionColor;
        yield return new WaitForSeconds(damageReactionColorFrames * Time.deltaTime);
        gameObject.GetComponent<SpriteRenderer>().color = originalColor;
    }
    private void SpriteChange()
    {
        int spriteIndex = Mathf.Clamp(Mathf.RoundToInt((brokenSpriteLevels.Length - 1) * (1 - (health / maxHealth))), 0, brokenSpriteLevels.Length - 1);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = brokenSpriteLevels[spriteIndex];
    }
    private int GetTotalWeight()
    {
        int totalWeight = 0;

        foreach(int w in weights)
        {
            totalWeight += w;
        }

        return totalWeight;
    }
    private GameObject GetItem(int rand)
    {
        for(int index = 0; index < weights.Length; index++)
        {
            if(rand <= weights[index])
            {
                return items[index];
            }
            else
            {
                rand -= weights[index];
                continue;
            }
        }

        return null;
    }
    private void RemoveShipFromPoolIfAtMaxPlayerShips()
    {
        if(FindObjectsOfType<Player>().Length >= GameSession.MaxPlayerShips)
        {
            weights[0] = 0;
        }
    }
    private void ChooseItem()
    {
        int rand = Random.Range(0, GetTotalWeight());
        prefabToInstantiate = GetItem(rand);
    }
    private Sprite GetItemSprite()
    {
        Sprite itemSprite = prefabToInstantiate.GetComponent<SpriteRenderer>().sprite;
        extentsX = gameObject.GetComponent<SpriteRenderer>().bounds.extents.x;
        if(extentsX < itemSprite.bounds.extents.x)
        {
            itemInside.transform.localScale = new Vector3 (0.5f, 0.5f, 1);
        }
        return itemSprite;
    }
    public void DropItem()
    {
        if(prefabToInstantiate.tag != "EmptyItem")
        {
            GameObject dropItem = Instantiate(prefabToInstantiate, gameObject.transform.position, Quaternion.identity);
            //dropItem.GetComponent<Rigidbody2D>().velocity = new Vector2 (0f, -gameSession.DropSpeed);
        }
    }
    public void UpdateTooltipText()
    {
        tooltipDisplay.DisplayChange(itemName, tooltipText, tooltipSprite);
    }
    private bool PlayerIsDetected()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(gameObject.transform.position, Vector2.down);
        for(int index = 0; index < hit.Length; index++)
        {
            Player player = hit[index].collider.GetComponent<Player>();
            if(player != null)
            {
                return true;
            }
            else
            {
                continue;
            }
        }
        return false;
    }
    private void AddHalo()
    {
        GameObject halo = Instantiate(haloColors[prefabToInstantiate.GetComponent<Item>().GetTier()], transform);
        halo.transform.SetParent(gameObject.transform);
    }
}
