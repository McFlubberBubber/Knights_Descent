using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public BattleManager battleManager; // Assign BattleManager in Inspector

    //Calling the start combat as soon as the game starts
    private void Start(){
        StartCombat();
    }

    public void StartCombat(){

        // Condition to ensure the battlemanager is assigned to start the battle
        if (battleManager != null){
            battleManager.StartBattle();
        } else {
            Debug.LogError("BattleManager not assigned in GameManager!");
        }
    }
}
