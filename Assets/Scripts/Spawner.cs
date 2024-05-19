using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Stats")]
    public int EnemyAmount = 20;
    private int enemyCount = 0;
    public float EnemyInterval = 1f;

    [Header("Enemy Spawn Position")]
    public Transform EnemySpawnPosition;
    public float EnemySpawnRadius = 5f;
    [Header("Player Spawn Position")]
    public Transform PlayerSpawnPosition;

    [Header("Game Objects")]
    public GameObject Player;
    public GameObject Enemy;

    private void Start()
    {
        SpawnPlayer();
        StartCoroutine(SpawnEnemies());
    }

    private void Update()
    {

    }

    private void SpawnPlayer()
    {
        Player.transform.position = PlayerSpawnPosition.position;
    }

    private IEnumerator SpawnEnemies()
    {
        while (enemyCount++ < EnemyAmount)
        {
            Vector2 randomPoint = Random.insideUnitCircle * EnemySpawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y);

            Instantiate(Enemy, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(EnemyInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(EnemySpawnRadius * 2f, 0.1f, EnemySpawnRadius * 2f));
    }
}
