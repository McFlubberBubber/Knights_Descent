using UnityEngine;

public class CardLogic : MonoBehaviour
{
    private CardDisplay cardDisplay;
    [SerializeField] private PlayerStats playerStats;  // Reference to PlayerStats
    [SerializeField] private EnergyManager energyManager;  // Reference to EnergyManager
    [SerializeField] private EnemyController enemyController;  // Reference to EnemyController

    private void Awake(){
        cardDisplay = GetComponent<CardDisplay>(); // Get the CardDisplay component
    }

    // Method to set PlayerStats from CardManager
    public void SetPlayerStats(PlayerStats stats){
        playerStats = stats;
    }

    // Method to set EnergyManager from CardManager
    public void SetEnergyManager(EnergyManager manager){
        energyManager = manager;
    }

    // Method to set EnemyController from CardManager
    public void SetEnemyController(EnemyController controller){
        enemyController = controller;
    }

    public void PlayCard()
    {
        if (cardDisplay == null || cardDisplay.card == null) return;
        Card card = cardDisplay.card; // Retrieve card data

        switch (card.type)
        {
            case Card.cardType.Attack:
                // Apply damage to the enemy
                if (enemyController != null)
                {
                    enemyController.TakeDamage(card.damage);  // Assuming you have a method in EnemyController to take damage
                }
                break;

            case Card.cardType.Skill:
                if (card.block > 0)
                    playerStats.ApplyBlock(card.block);  // Apply block to the player
                break;
        }

        // Deduct energy after playing the card
        if (energyManager != null && card.cost > 0)
        {
            energyManager.SpendEnergy(card.cost);  // Deduct energy based on card cost
        }
    }
}
