using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Transform enemyUIPanel;
    [SerializeField] public Transform enemySpawnPoint;
    [SerializeField] private TurnHandler turnHandler;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private CardManager cardManager;
    [SerializeField] private GameManager gameManager;


    [Header("Enemy Data")]
    private EnemyData enemyToSpawn;
    [SerializeField] private List<EnemyData> enemyList; // List of enemies
    private int currentEnemyIndex = 0; // Track which enemy is next

    public void StartBattle(bool shouldResetDeck = true){
        if (currentEnemyIndex >= enemyList.Count) 
        {
            Debug.Log("All battles completed!"); 
            return; // No more battles
        }

        enemyToSpawn = enemyList[currentEnemyIndex]; // Select the next enemy
        SpawnEnemy();
        cardManager.InitializeGame(shouldResetDeck);
    }

    private void SpawnEnemy()
    {
        // Instantiate the enemy at the enemySpawnPoint's position
        GameObject enemyObj = Instantiate(enemyToSpawn.enemyPrefab, enemySpawnPoint.position, Quaternion.identity, enemySpawnPoint);

        // Get the EnemyController component
        EnemyController enemyController = enemyObj.GetComponent<EnemyController>();
        enemyController.enemyData = enemyToSpawn;
        // enemyController.currentHealth = 10; // Set initial health for the enemy
        enemyController.currentHealth = enemyToSpawn.maxHealth; // Set initial health for the enemy

        // Setting references
        cardManager.SetEnemyController(enemyController);
        enemyController.SetGameManager(gameManager);

        // Instantiating the enemy UI
        GameObject enemyUIObj = Instantiate(enemyToSpawn.enemyUIPrefab, enemyUIPanel);
        EnemyUI enemyUI = enemyUIObj.GetComponent<EnemyUI>();

        // Set the enemy UI reference in the enemy controller
        enemyController.enemyUI = enemyUI;
        enemyUI.UpdateStats(enemyController.currentHealth, enemyToSpawn.maxHealth, enemyController.currentBlock);

        // Allowing the enemy to use the player stats
        enemyController.playerStats = playerStats;

        // Set the damage display reference and spawn point for the enemy
        enemyController.damageDisplay = enemyObj.GetComponentInChildren<DamageDisplay>();

        // Ensure spawnTarget is assigned to the correct location
        if (enemyController.damageDisplay != null){
            enemyController.damageDisplay.spawnTarget = enemyObj.transform;
        }

        // Assign the damage text prefab to the DamageDisplay
        if (enemyController.damageDisplay != null && enemyToSpawn.damageTextPrefab != null)
        {
            enemyController.damageDisplay.damageTextPrefab = enemyToSpawn.damageTextPrefab;
        }

        // Notify TurnHandler about the enemy
        List<EnemyController> enemyList = new List<EnemyController> { enemyController };
        turnHandler.SetEnemies(enemyList);
        Debug.Log($"Enemy {enemyController.enemyData.enemyName} spawned and added to turn handler.");
    }


    //Function that will load the next battle
    public void initNextBattle()
    {
        currentEnemyIndex++; // Move to the next enemy
        StartBattle(false); // Start the next battle
    }

}
