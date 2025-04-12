using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BattleManager battleManager; 
    [SerializeField] private CardRewardManager cardRewardManager; 
    [SerializeField] private PlayerStats playerStats; 
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private CanvasGroup gameOverPanel;
    [SerializeField] private CanvasGroup victoryPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioSource gameAudio;
    [SerializeField] private CardManager cardManager;
    [SerializeField] private int enemiesSlain;
    [SerializeField] private TMP_Text enemiesSlainText;
    [SerializeField] private TMP_Text gameOverHeader;
    [SerializeField] private GameObject fadeTransitionPrefab;
    [SerializeField] private Transform gameCanvas;
    private int totalEnemies = 3;
    private SceneTransition sceneTransition;

    //Calling the start combat as soon as the game starts
    private void Start(){
        enemiesSlain = 0; // Initialize enemies slain count
        StartCombat();
        settingsPanel.SetActive(false); //Ensure the settings panel is not displayed from the start

        //Adjusting the game over panel on start so it does not interfere with game
        gameOverPanel.alpha = 0;
        gameOverPanel.interactable = false;
        gameOverPanel.blocksRaycasts = false;

        //Adjusting the victory panel on start so it does not interfere with game
        victoryPanel.alpha = 0;
        victoryPanel.interactable = false;
        victoryPanel.blocksRaycasts = false;

        //Matching the volume values of the slider to the game audio music
        volumeSlider.value = gameAudio.volume;
        volumeSlider.onValueChanged.AddListener(SetVolume);

        //Setting the Game Manager to the player stats
        if (playerStats != null){
            playerStats.SetGameManager(this);
        }

        // Instantiate the fade transition prefab and set its alpha to 1
        GameObject fadeObject = Instantiate(fadeTransitionPrefab, gameCanvas);
        Image fadeImage = fadeObject.GetComponent<Image>();
        fadeObject.SetActive(true); // Ensure the fade object is active

        // Set the fade image alpha to 1 (fully visible)
        Color fadeColor = fadeImage.color;
        fadeColor.a = 1;  // Set to fully visible (alpha = 1)
        fadeImage.color = fadeColor;

        // Start the fade-out and hide transition
        sceneTransition = fadeObject.GetComponent<SceneTransition>();
        if (sceneTransition != null) {
            StartCoroutine(sceneTransition.FadeOutAndHide(fadeImage, 1.0f)); // Fade out the image over 1 second
        }
    }

    public void StartCombat(){
        // Condition to ensure the battlemanager is assigned to start the battle
        if (battleManager != null){
            battleManager.StartBattle(true);
        } else {
            Debug.LogError("BattleManager not assigned in GameManager!");
        }
    }

    //Function that will show the settings panel
    public void DisplaySettings(){
        settingsPanel.SetActive(true);
    }

    //Function that will close the settings panel
    public void CloseSettings(){
        settingsPanel.SetActive(false);
    }

    //Function that will set the volume of the game
    public void SetVolume(float volume){
        gameAudio.volume = volume;
    }

    //Function that will display the game over panel when the players HP hits 0
    public void DisplayGameOver(){
        enemiesSlainText.text = $"- Enemies Slain: {enemiesSlain}"; // Update the text with the number of enemies slain
        StartCoroutine(FadeInGameOver()); // Start the fade-in effect
    }

    //Function that will fade in the game over panel
    private IEnumerator FadeInGameOver(){
        float duration = 1.0f; // Time in seconds
        float elapsedTime = 0f;

        gameOverPanel.interactable = true;
        gameOverPanel.blocksRaycasts = true;

        while (elapsedTime < duration){
            gameOverPanel.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        gameOverPanel.alpha = 1; // Ensure it's fully visible
    }

    //Calling the end turn function in card manager then fading the in the vistory screen
    public void DisplayVictory(){
        cardManager.EndTurn(); // End the turn to reset the cards
        cardRewardManager.ResetRewardButtons();
        StartCoroutine(DelayedVictoryFadeIn(0.4f)); // Start the fade-in effect
    }

    private IEnumerator DelayedVictoryFadeIn(float delay){
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        StartCoroutine(FadeInVictory()); // Now fade in the victory screen
    }

    //While loop that will fade in the victory screen
    private IEnumerator FadeInVictory(){
        float duration = 0.25f; // Time in seconds
        float elapsedTime = 0f;

        victoryPanel.interactable = true;
        victoryPanel.blocksRaycasts = true;

        while (elapsedTime < duration){
            victoryPanel.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        victoryPanel.alpha = 1; // Ensure it's fully visible
    }

    public void IncrementEnemiesSlain() {
        enemiesSlain++;
        Debug.Log($"Enemies slain: {enemiesSlain}");

        CheckVictoryCondition(enemiesSlain);
    }

    //Function to load the next battle
    public void LoadNextBattle(){
        // Stop any ongoing fade-in animations
        StopAllCoroutines();

        // Call card manager stuff
        cardManager.ResetDeck(); // Clear the cards from the previous battle

        victoryPanel.alpha = 0; 
        victoryPanel.interactable = false;
        victoryPanel.blocksRaycasts = false; 

        battleManager.initNextBattle();
    }

    //Checking the victory condition to see if the player has slain all the enemies, to then display the victory screen
    public void CheckVictoryCondition(int totalEnemiesSlain){
        if (totalEnemiesSlain >= totalEnemies){
            DisplayGameOver(); // Show the victory screen
            gameOverHeader.text = "Demo Complete"; // Update the header text
        } else {
            Debug.Log("Not all enemies slain yet!");
        }
    }

    //Function that will send the player back to the main menu scene
    public void ReturnToMainMenu(){
        sceneTransition.beginFadeTransition("MainMenu"); // Start the fade transition to the main menu
    }
}
