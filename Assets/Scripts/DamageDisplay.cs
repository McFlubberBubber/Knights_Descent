using UnityEngine;
using TMPro;
using System.Collections;

public class DamageDisplay : MonoBehaviour
{
    [Header("Reference to Damage Number Prefab")]
    public GameObject damageTextPrefab;

    [Header("Optional Target")]
    public Transform spawnTarget;

    //Function that will be called to show the damage number or "Blocked!"
    public void ShowDamageNumber(int damageAmount, bool isBlocked){
        // Ensure spawnTarget is assigned
        if (spawnTarget == null){
            Debug.LogWarning("spawnTarget is not assigned! Please assign it.");
            return;
        }

        // Instantiate the damage text prefab at the spawnTarget's position
        GameObject damageTextObj = Instantiate(damageTextPrefab, spawnTarget.position, Quaternion.identity);

        // Make sure the text appears in world space by setting it under the spawnTarget
        damageTextObj.transform.SetParent(spawnTarget, worldPositionStays: true);
        TextMeshProUGUI damageText = damageTextObj.GetComponent<TextMeshProUGUI>();

        // Check if the damage is blocked completely
        if (isBlocked){
            damageText.text = "Blocked!"; // Display "Blocked!" if the damage is fully blocked
        } else {
            // Otherwise, display the damage amount
            damageText.text = damageAmount.ToString();
        }

        // Start the animation
        StartCoroutine(AnimateDamageText(damageTextObj));
    }

    //Version of the function that will only show the "Blocked!" message when there is no damage overflow
    public void ShowDamageNumber(string message){
        // Instantiate the damage text prefab at the spawnTarget's position
        GameObject damageTextObj = Instantiate(damageTextPrefab, spawnTarget.position, Quaternion.identity);

        // Make sure the text appears in world space by setting it under the spawnTarget
        damageTextObj.transform.SetParent(spawnTarget, worldPositionStays: true);
        TextMeshProUGUI damageText = damageTextObj.GetComponent<TextMeshProUGUI>();

        // Set the text to display the "Blocked!" message
        damageText.text = message;

        // Start the animation
        StartCoroutine(AnimateDamageText(damageTextObj));
    }

    private IEnumerator AnimateDamageText(GameObject damageTextObj){
        float elapsedTime = 0f;
        float floatDuration = 1f;
        float fadeDuration = 0.5f;

        Vector3 startPos = damageTextObj.transform.position;
        TextMeshProUGUI tmp = damageTextObj.GetComponent<TextMeshProUGUI>();
        Color startColor = tmp.color;

        while (elapsedTime < floatDuration){
            // Make the text float upwards
            damageTextObj.transform.position = startPos + new Vector3(0, Mathf.Lerp(0, 50f, elapsedTime / floatDuration), 0);
            
            // Make the text fade out
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            tmp.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destroy the damage text after the animation completes
        Destroy(damageTextObj);
    }
}
