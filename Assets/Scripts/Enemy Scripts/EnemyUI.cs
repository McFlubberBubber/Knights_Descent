using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Image hpFill; 
    [SerializeField] private TMP_Text hpValue;

    [Header("Block UI")]
    [SerializeField] private GameObject blockObject; // Parent object of BlockIcon + BlockValue
    [SerializeField] private TMP_Text blockValue; 

    [Header("Intent UI")]
    [SerializeField] private CanvasGroup intentGroup;
    [SerializeField] private Image intentIcon;  
    [SerializeField] private TMP_Text intentValue;  
    [SerializeField] private IntentIconLibrary iconLibrary; 

    private void Awake()
    {
        // intentGroup.alpha = 0; // Hide intent UI at the start
    }

    // **UPDATE INTENT ICON**
    public void UpdateIntent(EnemyAction nextAction, int buffProgress) {
        if (nextAction == null) {
            Debug.LogWarning("UpdateIntent called with NULL action!");
            return;
        }

        //Debug.Log($"Enemy intent update: {nextAction.name}");

        if (iconLibrary != null) {
            Sprite newIcon = iconLibrary.GetDefaultIcon(nextAction);

            if (newIcon != null) {
                intentIcon.sprite = newIcon;
                intentIcon.gameObject.SetActive(true); // Make sure it's visible
                //Debug.Log($"Intent Icon Set: {newIcon.name}");
            } else {
                Debug.LogWarning($"No icon found for action: {nextAction.name}");
            }
        } else {
            Debug.LogError("IntentIconLibrary is NULL! Assign it in the Enemy UI.");
        }

        intentIcon.enabled = (intentIcon.sprite != null);  

        // Update intentValue based on action type
        switch (nextAction.actionType) {
            case ActionType.Damage:

                if (nextAction.isMultiHit) {
                    // Show damage as "damagePerHit x hits" (e.g., "2x3")
                    // int adjustedDamagePerHit = nextAction.damagePerHit + nextAction.buffAmount;
                    // intentValue.text = $"{adjustedDamagePerHit}x{nextAction.hits}";

                    int adjustedDamagePerHit = nextAction.damagePerHit + buffProgress;
                    intentValue.text = $"{adjustedDamagePerHit}x{nextAction.hits}";

                } else {
                    // Show single damage value
                    // intentValue.text = (nextAction.GetTotalDamage() > 0) ? nextAction.GetTotalDamage().ToString() : "";

                    int adjustedDamage = nextAction.damagePerHit + buffProgress;
                    intentValue.text = (adjustedDamage > 0) ? adjustedDamage.ToString() : "";
                }
                break;

            case ActionType.Block:
                intentValue.text = (nextAction.blockAmount > 0) ? nextAction.blockAmount.ToString() : "";
                break;

            case ActionType.Heal:
                intentValue.text = (nextAction.healAmount > 0) ? nextAction.healAmount.ToString() : "";
                break;

            case ActionType.Buff:
                intentValue.text = (nextAction.buffAmount > 0) ? $"+{nextAction.buffAmount}" : "";
                break;

            default:
                intentValue.text = ""; // Clear text if no matching action type
                break;
        }
    }

    public void ShowIntent(){
        StartCoroutine(FadeInIntent());
    }

    private IEnumerator FadeInIntent(){
        float duration = 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            intentGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        intentGroup.alpha = 1;
    }


    // **UPDATE ENEMY HP & BLOCK**
    public void UpdateStats(int currentHP, int maxHP, int block) 
    {
        hpValue.text = $"{currentHP}/{maxHP}";
        hpFill.fillAmount = (float)currentHP / maxHP;

        blockObject.SetActive(block > 0); 
        blockValue.text = (block > 0) ? block.ToString() : "";
    }
}
