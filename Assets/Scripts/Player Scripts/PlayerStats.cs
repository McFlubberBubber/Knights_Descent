using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Stats")]
    public int maxHealth = 75;
    public int currentHealth;

    [Header("UI Elements")]
    [SerializeField] private Image HPFill;
    [SerializeField] private TMP_Text HPText;
    [SerializeField] private TMP_Text TopHPText;

    [Header("Block Stats")]
    private int blockAmount;
    [SerializeField] private GameObject blockObject;
    [SerializeField] private TMP_Text blockValueText;

    //Referring to the game manager for game over screens
    [SerializeField] private GameManager gameManager;
    private Vector3 originalPosition; // Store the original position for shake animation
    private DamageDisplay damageDisplay; // Reference to the DamageDisplay script

    private void Awake(){
        originalPosition = transform.localPosition; // Store the original position of the player
    }

    //Intitialize the health and block values
    void Start(){
        currentHealth = maxHealth;
        Debug.Log($"Player initialized with {currentHealth} HP");
        // currentHealth = 5; //Setting a value for testing purposes
        UpdateHealthUI();
        UpdateBlockUI();
        blockObject.SetActive(false); //Only showing the block value when the player has block value
        blockAmount = 0;

        damageDisplay = GetComponent<DamageDisplay>(); // Get the DamageDisplay component
    }

    //Setting the game manager
    public void SetGameManager(GameManager manager){
        gameManager = manager;
    }

    public void TakeDamage(int damageAmount) {
        Debug.Log($"Player takes {damageAmount} damage. Current HP: {currentHealth}, Current Block: {blockAmount}");
        int remainingDamage = damageAmount;

        if (blockAmount > 0) {
            // If block exists, reduce it first
            remainingDamage -= blockAmount;
            blockAmount = Mathf.Max(0, blockAmount - damageAmount);  // Decrease block

            if (remainingDamage > 0) { // Apply remaining damage to health
                currentHealth = Mathf.Max(0, currentHealth - remainingDamage);
                StartCoroutine(ShakeAnimation(0.25f, 7.5f)); // Start the shake animation

                // Show remaining damage after block
                damageDisplay.ShowDamageNumber(remainingDamage, false); // Passing false as it's not blocked
            } else {
                // If the damage is fully blocked
                damageDisplay.ShowDamageNumber(damageAmount, true); // Show "Blocked!" text
            }
        } else {
            // If no block, just apply damage to health
            currentHealth = Mathf.Max(0, currentHealth - damageAmount);
            StartCoroutine(ShakeAnimation(0.25f, 7.5f)); // Start the shake animation
            damageDisplay.ShowDamageNumber(damageAmount, false); // Show full damage if no block
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

    public void Heal(int amount){
        if (amount <= 0) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Player healed for {amount}. Current HP: {currentHealth}");
        UpdateHealthUI();
    }

    public void ApplySelfDamage(int selfDmg){
        Debug.Log($"Player takes {selfDmg} self-damage.");

        currentHealth = Mathf.Max(0, currentHealth - selfDmg);
        StartCoroutine(ShakeAnimation(0.25f, 7.5f)); // Start the shake animation
        damageDisplay.ShowDamageNumber(selfDmg, false); 

        UpdateHealthUI();
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
        HPFill.fillAmount = (float)currentHealth / maxHealth; 
        HPText.text = $"{currentHealth}/{maxHealth}"; 
        TopHPText.text = $"{currentHealth}/{maxHealth}"; 
    }

    //Function that will be called when the player dies  
    private void TriggerGameOver() {
        Debug.Log("Game Over!"); 
        if (gameManager != null){
            gameManager.DisplayGameOver(); // Call Game Over function
        } else {
            Debug.LogError("GameManager reference is missing in PlayerStats!");
        }
    }

    //Function that will handle the shake animation when the player takes damage
    private IEnumerator ShakeAnimation(float shakeDuration, float shakeStrength){
        float elapsedTime = 0f;
         float shakeInterval = 0.05f; // Time between shakes

        while (elapsedTime < shakeDuration){
            float x = Random.Range(-1f, 1f) * shakeStrength;
            float y = Random.Range(-1f, 1f) * shakeStrength;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);
            yield return new WaitForSeconds(shakeInterval); // Wait for the shake interval

            elapsedTime += shakeInterval;
        }
        transform.localPosition = originalPosition; // Reset to original position
    }
}
