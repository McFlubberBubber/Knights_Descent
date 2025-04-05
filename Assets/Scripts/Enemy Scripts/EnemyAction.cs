using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAction", menuName = "Scriptable Objects/EnemyAction")]
public class EnemyAction : ScriptableObject
{
    public string actionName;

    [Header("Action Type")]
    public ActionType actionType;

    [Header("Dynamic Damage")]
    public bool isMultiHit = false;
    public int hits = 1;
    public int damagePerHit;

    [Header("Effect Values")]
    public int blockAmount;  // For Block actions
    public int healAmount;   // For Heal actions
    public int buffAmount;   // For Buff actions

    // Default to null (use library)
    public Sprite intentIconOverride; 

    public Sprite GetIntentIcon(IntentIconLibrary library) {
        return intentIconOverride ?? library.GetDefaultIcon(this);
    }

    // Method to calculate total damage with buff applied
    public int GetTotalDamage(){
        return isMultiHit ? (damagePerHit + buffAmount) * hits : damagePerHit + buffAmount;
    }
}

// Enum to define the type of action
public enum ActionType
{
    Damage,
    Block,
    Heal,
    Buff
}