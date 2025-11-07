using UnityEngine;
using TMPro; 

[RequireComponent(typeof(TextMeshProUGUI))]
public class MoneyDisplayUI : MonoBehaviour
{
    [SerializeField]
    private string prefix = "$: ";

    private TextMeshProUGUI moneyText;

    private void Awake()
    {
        moneyText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnMoneyChanged.AddListener(UpdateMoneyText);
            UpdateMoneyText(MoneyManager.Instance.CurrentMoney);
        }
        else
        {
            Debug.LogError("MoneyDisplayUI, MoneyManager.Instance'ý bulamadý! 'Managers' objesinin sahnede olduðundan emin ol.");
        }
    }

    private void OnDestroy()
    {
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnMoneyChanged.RemoveListener(UpdateMoneyText);
        }
    }

    public void UpdateMoneyText(int newAmount)
    {
        if (moneyText != null)
        {
            moneyText.text = prefix + newAmount.ToString();
        }
    }
}