using UnityEngine;

public class CardLogic : MonoBehaviour
{
    private CardDisplay cardDisplay;
    [SerializeField] private PlayerStats playerStats;  // Reference to PlayerStats
    [SerializeField] private EnergyManager energyManager;  // Reference to EnergyManager
    [SerializeField] private EnemyController enemyController;  // Reference to EnemyController
    [SerializeField] private CardManager cardManager;  // Reference to Card Manager

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

    // Method to set CardManager from CardManager itself
    public void SetCardManager(CardManager manager){
        cardManager = manager;
    }

    public void PlayCard(){
        if (cardDisplay == null || cardDisplay.card == null) return;
        Card card = cardDisplay.card; // Retrieve card data

        if (!energyManager.SpendEnergy(card.cost)){
            Debug.Log("Not enough energy to play this card!");
            cardManager.ReturnCardToHand(this.gameObject);
            return;
        }

        switch (card.type)
        {
            case Card.cardType.Attack:
                // Apply damage to the enemy
                if (enemyController != null){
                    enemyController.TakeDamage(card.damage);  
                }
                break;

            case Card.cardType.Skill:
                if (card.block > 0)
                    playerStats.ApplyBlock(card.block);  
                break;
        }

        // Discard the played card after applying its effects
        cardManager.DiscardPlayedCard(this.gameObject, card);
    }
}
