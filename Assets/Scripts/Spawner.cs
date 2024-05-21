using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Stats")]
    public int EnemyAmount = 20;
    public float EnemyInterval = 1f;

    [Header("Enemy Spawn Position")]
    public Transform EnemySpawnPosition;
    public float EnemySpawnRadius = 5f;
    [Header("Player Spawn Position")]
    public Transform PlayerSpawnPosition;

    [Header("Game Objects")]
    public GameObject Player;
    public GameObject Enemy;

    [Header("Pools")]
    List<GameObject> _enemies = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < EnemyAmount; i++)
        {
            _enemies.Add(CreateEnemy());
        }
        SpawnPlayer();
        StartCoroutine(SpawnEnemies());
    }

    private GameObject CreateEnemy()
    {
        Vector2 randomPoint = Random.insideUnitCircle * EnemySpawnRadius;
        Vector2 spawnPosition = transform.position + new Vector3(randomPoint.x, randomPoint.y);

        GameObject enemy = Instantiate(Enemy, spawnPosition, Quaternion.identity);

        enemy.GetComponent<EnemyController>().MoveSpeed *= Random.value + 1f;
        enemy.GetComponent<EnemyController>().MoveDirection.x = Random.Range(-1f, 1f);
        enemy.GetComponent<EnemyController>().MoveDirection.y = Random.Range(-1f, 1f);

        enemy.SetActive(false);

        return enemy;
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
        foreach(var enemy in _enemies)
        {
            enemy.SetActive(true);
            yield return new WaitForSeconds(EnemyInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(EnemySpawnRadius * 2f, EnemySpawnRadius * 2f));
    }
}
