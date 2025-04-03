using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class Card : ScriptableObject
{
    public string cardName; //Name of the card
    public cardType type; //Category of the card
    public int cost; //The energy cost of the card
    public int damage; //Damage value
    public int block; //Block value
    public int heal; //Heal value
    public int selfDamage; //Extra value for the card (e.g. multi hits)

    public enum cardType {Attack, Skill}; //Creating a custom data type 
    public string descriptionTemplate; //Template for the card description

    public string GetDescription()
    {   
        string desc = descriptionTemplate;

        // Replace placeholders with actual values
        desc = desc.Replace("{damage}", damage.ToString());
        desc = desc.Replace("{block}", block.ToString());
        desc = desc.Replace("{heal}", heal.ToString());
        desc = desc.Replace("{selfDamage}", selfDamage.ToString());

        return desc;
    }
}
