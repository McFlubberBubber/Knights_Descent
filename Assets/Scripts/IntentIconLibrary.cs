using UnityEngine;

[CreateAssetMenu(fileName = "IntentIconLibrary", menuName = "Scriptable Objects/IntentIconLibrary")]
public class IntentIconLibrary : ScriptableObject 
{
    public Sprite attackIcon;
    public Sprite blockIcon;
    public Sprite healIcon;
    public Sprite buffIcon;

    public Sprite GetDefaultIcon(EnemyAction action) 
    {
        switch (action.actionType)
        {
            case ActionType.Damage:
                return attackIcon;
            case ActionType.Block:
                return blockIcon;
            case ActionType.Heal:
                return healIcon;
            case ActionType.Buff:
                return buffIcon;
            default:
                return null; // Return null if no matching action type is found
        }
    }
}