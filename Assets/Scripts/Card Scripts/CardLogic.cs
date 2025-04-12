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
        if (cardDisplay?.card == null) return; // Using null-conditional operator

        Card card = cardDisplay.card; // Retrieve card data

        // Check if the player has enough energy to play the card
        if (!energyManager.SpendEnergy(card.cost)){
            Debug.Log("Not enough energy to play this card!");
            cardManager.ReturnCardToHand(this.gameObject);
            return;
        }

        // Apply effects (only execute if the value is greater than 0)
        ApplyCardEffects(card);

        // Discard the played card after applying its effects
        cardManager.DiscardPlayedCard(this.gameObject, card);
    }

    //Function that applies the necessary card effects based on if the effects have a value greater than 0
    private void ApplyCardEffects(Card card) {
        if (card.heal > 0) playerStats.Heal(card.heal);
        if (card.selfDamage > 0) playerStats.ApplySelfDamage(card.selfDamage);
        if (card.cardDraw > 0) cardManager.DrawCards(card.cardDraw);
        if (card.overcharge > 0) energyManager.GainEnergy(card.overcharge);
        if (card.damage > 0) enemyController.TakeDamage(card.damage);
        if (card.block > 0) playerStats.ApplyBlock(card.block);
    }

}
