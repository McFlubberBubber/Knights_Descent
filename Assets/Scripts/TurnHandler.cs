using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class TurnHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardManager cardManager;
    [SerializeField] private Image turnProgressFill;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private List<EnemyController> enemies = new List<EnemyController>(); //In the event that we have multiple enemies


    [Header("Colors")]
    [SerializeField] private Color readyColor = Color.green;
    [SerializeField] private Color disabledColor = new Color(0.6f, 0.6f, 0.6f); // Gray

    [Header("Settings")]
    [SerializeField] private float holdDuration = .5f;

    private Color originalFillColor;
    private bool isPlayerTurn = true;
    private bool isHolding;
    private bool isProcessingTurn;
    private Coroutine holdRoutine;

    void Start(){
        originalFillColor = turnProgressFill.color;
        UpdateButtonState(); // Initialize visual state
    }

    public void SetEnemies(List<EnemyController> enemyControllers){
        enemies = enemyControllers;
    }   

    //Function that will handle the button press and release events
    public void OnPointerDown(){
        if (!CanAcceptInput()) return;
        isHolding = true;
        holdRoutine = StartCoroutine(HoldRoutine());
    }

    //If the player releases the button, stop the hold routine and reset the fill amount
    public void OnPointerUp(){ 
        ForceReleaseButton();
    }

    //Function that will update the button state based on the current game state
    private void UpdateButtonState(){
        if (isProcessingTurn || !isPlayerTurn){
            // Disabled state (gray)
            turnProgressFill.color = disabledColor;
            endTurnButton.interactable = false;
        } else {
            // Fully active state
            turnProgressFill.color = originalFillColor;
            endTurnButton.interactable = true;
        }
    }

    //Function that will check if the button can be pressed based on whether its the players turn and if the turn is not being processed
    private bool CanAcceptInput(){
        return isPlayerTurn && !isProcessingTurn;
    }

    //Function that will handle the hold routine for the button press
    private IEnumerator HoldRoutine(){
        float timer = 0;
        
        while (isHolding && timer < holdDuration)
        {
            if (!CanAcceptInput())
            {
                ForceReleaseButton();
                yield break;
            }
            timer += Time.deltaTime;
            turnProgressFill.fillAmount = timer / holdDuration;
            turnProgressFill.color = Color.Lerp(originalFillColor, readyColor, turnProgressFill.fillAmount);
            yield return null;
        }

        if (isHolding) yield return ExecuteEndTurn();
    }

    //Executing the end turn logic when the button is held down for the required duration
    private IEnumerator ExecuteEndTurn(){
        isProcessingTurn = true;
        UpdateButtonState(); // Visual feedback
        
        ForceReleaseButton(); //releasing the button
        cardManager.EndTurn();
        yield return StartCoroutine(EnemyTurnRoutine());
        
        isProcessingTurn = false;
        UpdateButtonState(); // Return to active state
    }

    //Function that will handle the enemy turn
    private IEnumerator EnemyTurnRoutine(){
        EndPlayerTurn();
        yield return new WaitForSeconds(0.5f); // optional pre-delay

        foreach (EnemyController enemy in enemies){
            if (enemy != null){
                Debug.Log($"Enemy {enemy.enemyData.enemyName} is taking a turn.");
                yield return StartCoroutine(enemy.TakeTurn()); // Wait for enemy to finish
                yield return new WaitForSeconds(0.2f);
            }
        }
        Debug.Log("Enemy Turn Ended, Switching to Player Turn.");
        StartPlayerTurn();
    }

    //Function that will start the player turn and show the next intent of the enemies
    private void StartPlayerTurn() {
        foreach (EnemyController enemy in enemies) {
            if (enemy != null) {
                //enemy.ResetBlock(); // Reset block to 0
                enemy.ShowNextIntent(); // Show the next intent
            }
        }
        isPlayerTurn = true;
        UpdateButtonState();
        cardManager.energyManager.RestoreEnergy(); //Restoring the energy
        cardManager.DrawHand(); // Draw the player's hand
        playerStats.ResetBlock();
    }

    //Function that will end the player turn and reset the button state
    private void EndPlayerTurn() {
        Debug.Log("Player Turn Ended");
        isPlayerTurn = false;
        UpdateButtonState();
    }

    //Forcefully releasing the button and resetting the fill amount
    public void ForceReleaseButton(){
        if (!isHolding) return;
        
        isHolding = false;
        if (holdRoutine != null) StopCoroutine(holdRoutine);
        turnProgressFill.fillAmount = 0;
        UpdateButtonState();
        endTurnButton.OnPointerExit(null);
    }
}