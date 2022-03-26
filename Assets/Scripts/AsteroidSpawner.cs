using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] int numberOfAsteroids = 3;
    [SerializeField] GameObject[] asteroidPrefab;
    [SerializeField] float spawnVelocityXMin;
    [SerializeField] float spawnVelocityXMax;
    [SerializeField] float spawnVelocityYMin;
    [SerializeField] float spawnVelocityYMax;
    // Start is called before the first frame update
    [SerializeField] bool continuallySpawn = false;

    [Header("Projectile Mode")]
    float shotCounter = 1f;
    [SerializeField] float minTimeBetweenShots = 1f;
    [SerializeField] float maxTimeBetweenShots = 3f;

    private void Update()
    {
        if(continuallySpawn)
        {
            CountDownAndSpawn();
        }
    }

    private void CountDownAndSpawn()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            SpawnAsteroids();
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    public void SpawnAsteroids()
    {
        for (int spawnIndex = 1; spawnIndex <= numberOfAsteroids; spawnIndex++)
        {
            //Debug.Log("X Min/ Max: (" + spawnVelocityXMin + ", " + spawnVelocityXMax + ")");
            //Debug.Log("X Min/ Max: (" + spawnVelocityYMin + ", " + spawnVelocityYMax + ")");
            float asteroidVelocityX = UnityEngine.Random.Range(spawnVelocityXMin, spawnVelocityXMax) * Time.deltaTime;
            float asteroidVelocityY = UnityEngine.Random.Range(spawnVelocityYMin, spawnVelocityYMax) * Time.deltaTime;
            Vector2 asteroidVelocityVector = new Vector2(asteroidVelocityX, asteroidVelocityY);
            int asteroidPrefabIndex = UnityEngine.Random.Range(0,  asteroidPrefab.Length);
            //Debug.Log("Expected velocity: (" + asteroidVelocityX / Time.deltaTime+ ", " + asteroidVelocityY / Time.deltaTime + ")");
            
            GameObject newAsteroid = Instantiate(asteroidPrefab[asteroidPrefabIndex], transform.position, Quaternion.identity);
            newAsteroid.GetComponent<Rigidbody2D>().velocity = asteroidVelocityVector;
            //Debug.Log("Actual velocity: " + newAsteroid.GetComponent<Rigidbody2D>().velocity / Time.deltaTime);
        }
    }
}
