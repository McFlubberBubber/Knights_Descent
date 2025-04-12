using UnityEngine;
using DG.Tweening; //using this package for DOTween animations
using System.Collections;
using UnityEngine.UI; // For UI elements

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
    [SerializeField] private GameManager gameManager;
    private static readonly AnimationCurve lungeCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.5f, 1f),
        new Keyframe(1f, 0f)
    );
    private Vector3 originalPosition;
    private EnemyAction selectedAction; // Store the selected action
    private int buffProgress = 0;

    public DamageDisplay damageDisplay;
    public Transform damageSpawnPoint;

    private void Start() {
        InitializeEnemy();
        // originalPosition = transform.position;
    }

    //Awake function to set the original position of the enemy for shake animation
    private void Awake(){
        originalPosition = transform.localPosition; // Store the original position of the enemy
    }

    //Setter for the game manager
    public void SetGameManager(GameManager gManager) {
        gameManager = gManager;
    } 

    //Initializes the enemy stats and sets the first action
    private void InitializeEnemy() {
        currentHealth = enemyData.maxHealth;
        // currentHealth = 10; // For testing purposes, set a fixed health value

        // Reset action sequence index to ensure it starts from the first action
        enemyData.currentSequenceIndex = 0;

        // Determine and display the enemy's first action
        ShowNextIntent();
    }

    //Function that will allow the enemy to take its turn
    public IEnumerator TakeTurn(){
        if (selectedAction == null)
        {
            Debug.LogError($"Enemy {enemyData.enemyName} has no selected action to execute!");
            yield break;
        }

        ResetBlock(); // At start of turn
        Debug.Log($"Enemy {enemyData.enemyName} executes action: {selectedAction.actionName}");

        yield return StartCoroutine(ExecuteAction(selectedAction)); // Wait for action to finish
    }

    //Function that will execute the action of the enemy
    private IEnumerator ExecuteAction(EnemyAction action)
    {
        switch (action.actionType)
        {
            case ActionType.Damage:
                int damagePerHit = GetAdjustedDamage(action);  // Get adjusted damage using the function
                Debug.Log($"Enemy deals {damagePerHit} total damage.");

                // Execute multiple hits if it's a multi-hit action
                for (int i = 0; i < action.hits; i++)
                {
                    if (playerStats.currentHealth <= 0)  // Check if the player's health is 0 or less
                    {
                        Debug.Log("Player has died. Ending attack sequence.");
                        break;  // Stop the attack sequence
                    }

                    // Wait for the full animation before continuing to the next hit
                    yield return StartCoroutine(LungeForwardAnimation(100f, 0.5f, () => {
                        playerStats.TakeDamage(damagePerHit);
                    }));
                }
                break;

            case ActionType.Block:
                AddBlock(action.blockAmount);
                Debug.Log($"Enemy gains {action.blockAmount} block.");
                yield return new WaitForSeconds(0.3f);
                break;

            case ActionType.Heal:
                currentHealth = Mathf.Min(currentHealth + action.healAmount, enemyData.maxHealth);
                enemyUI.UpdateStats(currentHealth, enemyData.maxHealth, currentBlock);
                Debug.Log($"Enemy heals for {action.healAmount} HP.");
                yield return new WaitForSeconds(0.3f);
                break;

            case ActionType.Buff:
                buffProgress += action.buffAmount;
                Debug.Log($"Enemy applies buff for {action.buffAmount}.");
                yield return new WaitForSeconds(0.3f);
                break;

            default:
                Debug.LogWarning($"Unknown action type: {action.actionType}");
                break;
        }
    }

    // Function to get the adjusted damage based on buffs and multi-hit
    private int GetAdjustedDamage(EnemyAction action){
        // Calculate the adjusted damage per hit (including buffs)
        int adjustedDamagePerHit = action.damagePerHit + buffProgress;

        // Return the adjusted damage per hit (don't multiply by hits here)
        return adjustedDamagePerHit;
    }

    //Function to add block for the enemy
    public void AddBlock(int amount) {
        currentBlock += amount;
        enemyUI.UpdateStats(currentHealth, enemyData.maxHealth, currentBlock); // Update UI
    }

    //Function that will reset the block of the enemy
    public void ResetBlock() {
        currentBlock = 0;
        //Debug.Log($"Enemy {enemyData.enemyName}'s block has been reset to 0.");
        enemyUI.UpdateStats(currentHealth, enemyData.maxHealth, currentBlock); // Update UI
    }

    //Function that will show the next intent of the enemy
    public void ShowNextIntent() {
        // Get the next action from the enemy's action sequence
        selectedAction = enemyData.GetNextAction();
        enemyUI.UpdateIntent(selectedAction, buffProgress); // Update the intent UI
    }

    public void TakeDamage(int amount){
        // Block reduces damage first
        int remainingDamage = amount - currentBlock;

        // Update currentBlock to reflect the damage taken
        currentBlock = Mathf.Max(0, currentBlock - amount);

        // If damage was blocked entirely, show "Blocked!" message
        if (remainingDamage <= 0){
            // Show the "Blocked!" message above the enemy
            if (damageDisplay != null){
                damageDisplay.ShowDamageNumber("Blocked!"); // Display "Blocked!" instead of damage number
            }
        } else {
            // If some damage remains after block, apply it
            currentHealth = Mathf.Max(0, currentHealth - remainingDamage);
            StartCoroutine(ShakeAnimation(0.25f, 7.5f)); // Start the shake animation

            // Show the remaining damage
            if (damageDisplay != null){
                damageDisplay.ShowDamageNumber(remainingDamage, false); // Show remaining damage above enemy
            }

            // Check if the enemy is dead
            if (currentHealth <= 0){
                Die();
            }
        }

        // Update the enemy UI to reflect the new health and block status
        enemyUI.UpdateStats(currentHealth, enemyData.maxHealth, currentBlock);
    }


    //Function that will handle the shake animation when the enemy takes damage
    private IEnumerator ShakeAnimation(float shakeDuration, float shakeStrength){
        float elapsedTime = 0f;
         float shakeInterval = 0.05f; // Time between shakes

        while (elapsedTime < shakeDuration){
            float x = Random.Range(-1f, 1f) * shakeStrength;
            float y = Random.Range(-1f, 1f) * shakeStrength;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);
            yield return new WaitForSeconds(shakeInterval); // Wait for the shake interval

            elapsedTime += shakeInterval;
            // yield return null;
        }
        transform.localPosition = originalPosition; // Reset to original position
    }

    public IEnumerator LungeForwardAnimation(float moveDistance, float moveDuration, System.Action onImpact)
    {
        Vector3 startPosition = transform.localPosition;
        Vector3 targetPosition = startPosition + new Vector3(-moveDistance, 0, 0); // move left toward player
        bool impactTriggered = false;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            float curvedT = lungeCurve.Evaluate(t); // use the shared curve
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, curvedT);
            elapsed += Time.deltaTime;

            if (!impactTriggered && t >= 0.5f){
                onImpact?.Invoke();
                impactTriggered = true;
            }
            yield return null;
        }
        //Return to the original position
        transform.localPosition = startPosition;
    }

    private void Die(){
        Debug.Log($"{enemyData.enemyName} has been defeated!");
        StartCoroutine(FadeOutAndDestroy());
        gameManager.IncrementEnemiesSlain(); // Increment the slain enemies count
    }

    private IEnumerator FadeOutAndDestroy(){
        float fadeDuration = 1f; // Duration of the fade
        Image enemyImage = GetComponent<Image>();  // Assuming the enemy uses an Image component
        float elapsed = 0f;

        Color startColor = enemyImage.color;
        while (elapsed < fadeDuration){
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration); // Fade out
            enemyImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure fully transparent
        enemyImage.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // After fading out, destroy the enemy
        Destroy(gameObject);
        Destroy(enemyUI.gameObject);

        if (gameManager != null)
            gameManager.DisplayVictory();
        else
            Debug.LogError("GameManager reference is missing!");
    }
}