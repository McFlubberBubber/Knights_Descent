using UnityEngine;
using TMPro;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private int maxEnergy = 3; // Max energy available per turn
    private int currentEnergy;

    [Header("UI References")]
    [SerializeField] private TMP_Text currentEnergyText; // Display the energy
    [SerializeField] private TMP_Text maxEnergyText; // Display the energy

    private void Start(){
        currentEnergy = maxEnergy;
        UpdateEnergyUI();
    }

    public bool SpendEnergy(int amount)
    {
        if (amount <= currentEnergy){
            currentEnergy -= amount;
            UpdateEnergyUI();
            return true;
        }
        return false;
    }

    public void RestoreEnergy(){
        currentEnergy = maxEnergy;
        UpdateEnergyUI();
    }

    private void UpdateEnergyUI(){
        currentEnergyText.text = currentEnergy.ToString();
        maxEnergyText.text = maxEnergy.ToString();
    }
}
