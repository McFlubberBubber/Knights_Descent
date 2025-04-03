using UnityEngine;
using DG.Tweening; //using this package for DOTween animations

public class EnemyController : MonoBehaviour 
{
    [Header("Data")]
    public EnemyData enemyData; // Assign in Inspector
    public int currentHealth;
    public int currentBlock;

    [Header("UI")]
    [SerializeField] public EnemyUI enemyUI;
    
    [Header("References")]
    [SerializeField] public PlayerStats playerStats;

    [Header("Animations")]
    // [SerializeField] private float attackDistance = 10000f;
    // [SerializeField] private float attackDuration = 1.0f;
    // [SerializeField] private float shakeStrength = 0.1f;
    // [SerializeField] private float shakeDuration = 0.2f;
    // private Vector3 originalPosition;


    private EnemyAction selectedAction; // Store the selected action
    private int buffProgress = 0;

    private void Start() {
        InitializeEnemy();
        // originalPosition = transform.position;
    }

    private void InitializeEnemy() {
        currentHealth = enemyData.maxHealth;

        // Reset action sequence index to ensure it starts from the first action
        enemyData.currentSequenceIndex = 0;

        // Determine and display the enemy's first action
        ShowNextIntent();
    }

    public void TakeTurn() {
        if (selectedAction == null) {
            Debug.LogError($"Enemy {enemyData.enemyName} has no selected action to execute!");
            return;
        }

        // Reset block at the start of their turn
        ResetBlock();

        Debug.Log($"Enemy {enemyData.enemyName} executes action: {selectedAction.actionName}");

        // Execute the stored action
        ExecuteAction(selectedAction);
    }

    private void ExecuteAction(EnemyAction action) {
        if (action == null) {
            Debug.LogError("ExecuteAction called with a NULL action!");
            return;
        }

        switch (action.actionType) {
            case ActionType.Damage:
                // Handle damage logic
                int totalDamage = GetAdjustedDamage(action);
                Debug.Log($"Enemy deals {totalDamage} damage.");
                playerStats.TakeDamage(totalDamage);

                break;

            case ActionType.Block:
                // Handle block logic
                AddBlock(action.blockAmount);
                Debug.Log($"Enemy gains {action.blockAmount} block.");
                break;

            case ActionType.Heal:
                // Handle heal logic
                currentHealth = Mathf.Min(currentHealth + action.healAmount, enemyData.maxHealth);
                Debug.Log($"Enemy heals for {action.healAmount} HP.");
                break;

            case ActionType.Buff:
                // Handle buff logic
                // action.ApplyBuff(action.buffAmount); 
                buffProgress += selectedAction.buffAmount;
                Debug.Log($"Enemy applies buff for {action.buffAmount}.");
                break;

            default:
                Debug.LogWarning($"Unknown action type: {action.actionType}");
                break;
        }
    }

    private int GetAdjustedDamage(EnemyAction action){
        int adjustedDamagePerHit = action.damagePerHit + buffProgress;
        return action.isMultiHit ? adjustedDamagePerHit * action.hits : adjustedDamagePerHit;
    }

    public void AddBlock(int amount) {
        currentBlock += amount;
        enemyUI.UpdateStats(currentHealth, enemyData.maxHealth, currentBlock); // Update UI
    }


    public void ResetBlock() {
        currentBlock = 0;
        //Debug.Log($"Enemy {enemyData.enemyName}'s block has been reset to 0.");
        enemyUI.UpdateStats(currentHealth, enemyData.maxHealth, currentBlock); // Update UI
    }

    public void ShowNextIntent() {
        // Get the next action from the enemy's action sequence
        selectedAction = enemyData.GetNextAction();
        enemyUI.UpdateIntent(selectedAction, buffProgress); // Update the intent UI
    }

    public void TakeDamage(int amount){
        // Block reduces damage first
        int remainingDamage = amount - currentBlock;
        currentBlock = Mathf.Max(0, currentBlock - amount);

        // Optional: Add animation when the enemy takes damage
        // transform.DOPunchPosition(Vector3.one * shakeStrength, shakeDuration, 10, 1f); // Uncomment to shake the enemy

        if (remainingDamage > 0)
        {
            currentHealth -= remainingDamage;
            if (currentHealth <= 0) Die();
        }
        enemyUI.UpdateStats(currentHealth, enemyData.maxHealth, currentBlock); // Update health and block UI
    }

    // public void TriggerAttackAnimation() {
    //     transform.DOMoveX(transform.position.x - attackDistance, attackDuration)
    //         .SetEase(Ease.OutQuad)
    //         .OnComplete(() => {
    //             transform.DOMoveX(originalPosition.x, attackDuration)
    //                 .SetEase(Ease.InQuad);
    //         });
    // }

    private void Die() {
        Destroy(gameObject); // Or play death animation
    }
}