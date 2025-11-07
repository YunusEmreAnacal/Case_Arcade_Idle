using UnityEngine;
using UnityEngine.Events; 

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Zaten bir tane varsa, yenisini yok et
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField]
    private int startingMoney = 100;

    public int CurrentMoney { get; private set; }

    public UnityEvent<int> OnMoneyChanged;

    private void Start()
    {
        CurrentMoney = startingMoney;
        OnMoneyChanged?.Invoke(CurrentMoney);
    }


    public void AddMoney(int amount)
    {
        if (amount <= 0) return;

        CurrentMoney += amount;
        OnMoneyChanged?.Invoke(CurrentMoney); // UI'a haber ver
    }

    public bool SpendMoney(int amount)
    {
        if (CurrentMoney >= amount)
        {
            // Para yeterli, harcamayý yap
            CurrentMoney -= amount;
            OnMoneyChanged?.Invoke(CurrentMoney); // UI'a haber ver
            return true;
        }
        else
        {
            return false;
        }
    }
}