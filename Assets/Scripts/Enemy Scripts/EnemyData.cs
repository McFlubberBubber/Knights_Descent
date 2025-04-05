using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject 
{
    [Header("Core Stats")]
    public string enemyName;
    public int maxHealth;
    public bool resetHealthOnSpawn = true;

    [Header("Prefabs")]
    public GameObject enemyUIPrefab; 
    public GameObject enemyPrefab; // Prefab for the enemy model
    public GameObject damageTextPrefab; // Add this line to reference the damage text prefab

    [Header("Behavior")]
    public List<EnemyAction> actionSequence;  // Fixed order of actions
    public bool loopSequence = true;          // If false, stops after last action
    public bool randomizeSequence = false;    // Overrides order if true

    // For sequence-based enemies (e.g., "Turns 1-3: Attack, Block, Heal")
    public int currentSequenceIndex = 0;      // Tracks progress (non-serialized)
    private EnemyAction lastAction = null;           // Last action (non-serialized)

    public EnemyAction GetNextAction() {
        if (randomizeSequence){
            if(actionSequence.Count == 1){
                return actionSequence[0]; // Only one action, no need to randomize
            }

            EnemyAction newAction;
            do {
                newAction = actionSequence[Random.Range(0, actionSequence.Count)];
            } while (newAction == lastAction); // Ensure it's not the same as the last action

            lastAction = newAction;
            return newAction;

        } else {
            
            EnemyAction nextAction = actionSequence[currentSequenceIndex];

            if(loopSequence) {
                currentSequenceIndex =  (currentSequenceIndex + 1) % actionSequence.Count; // Loop back to start
            } else if (currentSequenceIndex < actionSequence.Count - 1) {
                currentSequenceIndex++; // Move to next action
            } else {
                currentSequenceIndex = actionSequence.Count - 1; // Stay on last action
            }

            return nextAction; // Ensure return value
        }
    }

}
