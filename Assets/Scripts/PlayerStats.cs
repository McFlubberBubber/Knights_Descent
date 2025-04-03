using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Stats")]
    public int maxHealth = 75;
    private int currentHealth;

    [Header("UI Elements")]
    [SerializeField] private Image HPFill;
    [SerializeField] private TMP_Text HPText;

    [Header("Block Stats")]
    private int blockAmount;
    [SerializeField] private GameObject blockObject;
    [SerializeField] private TMP_Text blockValueText;

    //Intitialize the health and block values
    void Start(){
        currentHealth = maxHealth;
        Debug.Log($"Player initialized with {currentHealth} HP");
        // currentHealth = 50; // Set initial health to custom value for testing purposes
        UpdateHealthUI();
        UpdateBlockUI();
        blockObject.SetActive(false); //Only showing the block value when the player has block value
        blockAmount = 0;
    }
    
    //Function that will handle the damage taken by the player
    public void TakeDamage(int damageAmount) {
        Debug.Log($"Player takes {damageAmount} damage. Current HP: {currentHealth}, Current Block: {blockAmount}");

        if (blockAmount > 0) {
            // If block exists, reduce it first
            int remainingDamage = damageAmount - blockAmount;
            blockAmount = Mathf.Max(0, blockAmount - damageAmount);

            if (remainingDamage > 0) {
                currentHealth = Mathf.Max(0, currentHealth - remainingDamage);
            }
        } else {
            currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        }

        Debug.Log($"After damage: HP = {currentHealth}, Block = {blockAmount}");

        UpdateHealthUI();
        UpdateBlockUI();

        if (currentHealth <= 0) {
            Debug.Log("Player HP reached 0! Game Over triggered.");
            TriggerGameOver();
        }
    }


    //Function that will apply the block amount to the player stats
    public void ApplyBlock (int blockValue) {
        blockAmount += blockValue; //block value is the parameter that is passed to the function
        UpdateBlockUI();
    }

    //Function that will reset the block value to 0 when the player's turn starts
    public void ResetBlock() {
        blockAmount = 0;
        UpdateBlockUI();
    }

    //Updating the block UI
    private void UpdateBlockUI() {
        if (blockAmount > 0) {
            blockObject.SetActive(true); //Showing the block value when the player has block value
            blockValueText.text = blockAmount.ToString(); //Updating the text to show the current block value
        } else {
            blockObject.SetActive(false); //Hiding the block value when the player has no block value
        }
    }

    //Updating the health UI
    private void UpdateHealthUI() {
        HPFill.fillAmount = (float)currentHealth / maxHealth; //Updating the health bar fill amount
        HPText.text = $"{currentHealth}/{maxHealth}"; //Updating the text to show the current health value
    }

    //Function that will be called when the player dies  
    private void TriggerGameOver() {
        // Implement game over logic here, such as showing a game over screen or restarting the level.
        Debug.Log("Game Over!"); // Placeholder for game over logic
    }
}
