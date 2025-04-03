using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Transform enemyUIPanel;
    [SerializeField] public Transform enemySpawnPoint;
    [SerializeField] private TurnHandler turnHandler;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private CardManager cardManager; // Reference to CardManager


    [Header("Enemy Data")]
    public EnemyData enemyToSpawn;

    public void StartBattle(){
        SpawnEnemy();

        cardManager.InitializeGame();
    }

    private void SpawnEnemy()
    {
        GameObject enemyObj = Instantiate(enemyToSpawn.enemyPrefab, enemySpawnPoint.position, Quaternion.identity, enemySpawnPoint);
        EnemyController enemyController = enemyObj.GetComponent<EnemyController>();
        enemyController.enemyData = enemyToSpawn;
        enemyController.currentHealth = enemyToSpawn.maxHealth;
        //enemyController.currentHealth = 25;
        cardManager.SetEnemyController(enemyController);

        GameObject enemyUIObj = Instantiate(enemyToSpawn.enemyUIPrefab, enemyUIPanel);
        EnemyUI enemyUI = enemyUIObj.GetComponent<EnemyUI>();

        enemyController.enemyUI = enemyUI;
        enemyUI.UpdateStats(enemyController.currentHealth, enemyToSpawn.maxHealth, enemyController.currentBlock);
        enemyController.playerStats = playerStats;   

        // Notify TurnHandler about the enemy
        List<EnemyController> enemyList = new List<EnemyController> { enemyController };
        turnHandler.SetEnemies(enemyList);
        Debug.Log($"Enemy {enemyController.enemyData.enemyName} spawned and added to turn handler.");
    }

}
