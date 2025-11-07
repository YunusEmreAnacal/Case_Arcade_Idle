using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerStack))]
[RequireComponent(typeof(Animator))]
public class AIWorker : MonoBehaviour
{

    private Transform spawnerDepotLocation;
    private Transform transformerInputLocation;
    private CharacterAudio myAudio;
    private enum AiState { GettingItem, DeliveringItem }
    private AiState currentState = AiState.GettingItem;

    // Gerekli Bileþenler
    private NavMeshAgent agent;
    private PlayerStack stack;
    private Animator animator;

    private float transferTimer = 0f;
    private const float TRANSFER_INTERVAL = 0.1f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stack = GetComponent<PlayerStack>();
        animator = GetComponent<Animator>();
        myAudio = GetComponent<CharacterAudio>();
    }

    private void Start()
    {
        try
        {
            spawnerDepotLocation = GameObject.FindGameObjectWithTag("SpawnerDepot").transform;

            transformerInputLocation = GameObject.FindGameObjectWithTag("InputDepot").transform;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"AIWorker BAÞARISIZ: 'SpawnerDepot' veya 'InputDepot' etiketli objeler sahnede bulunamadý! Lütfen etiketleri kontrol et. Hata: {ex.Message}");
            this.enabled = false; // AI'ý devre dýþý býrak
            return;
        }

        GoToSpawner();
    }

    private void Update()
    {
        if (currentState == AiState.GettingItem && stack.IsFull)
        {
            GoToTransformer();
        }
        else if (currentState == AiState.DeliveringItem && stack.IsEmpty)
        {
            GoToSpawner();
        }

        UpdateAnimations();

        if (transferTimer < TRANSFER_INTERVAL)
        {
            transferTimer += Time.deltaTime;
        }
    }


    private void GoToSpawner()
    {
        currentState = AiState.GettingItem;
        agent.SetDestination(spawnerDepotLocation.position);
    }

    private void GoToTransformer()
    {
        currentState = AiState.DeliveringItem;
        agent.SetDestination(transformerInputLocation.position);
    }

    private void UpdateAnimations()
    {
        bool isRunning = agent.velocity.magnitude > 0.1f;
        animator.SetBool("IsRunning", isRunning);

        bool isCarrying = !stack.IsEmpty;
        animator.SetBool("IsCarrying", isCarrying);
    }

    private void OnTriggerStay(Collider other)
    {
        if (transferTimer < TRANSFER_INTERVAL) return;

        string tag = other.tag;

        // 1. GÖREV: ITEM TOPLAMA
        if (tag == "SpawnerDepot" && currentState == AiState.GettingItem && !stack.IsFull)
        {
            if (other.TryGetComponent<GridDepot>(out GridDepot spawnerDepot))
            {
                if (!spawnerDepot.IsEmpty)
                {
                    GameObject item = spawnerDepot.RemoveItem();
                    myAudio.PlayPickupSound();
                    if (item != null)
                    {
                        myAudio.PlayPickupSound();
                        stack.AddItem(item);
                        transferTimer = 0f;
                    }
                }
            }
        }
        // 2. GÖREV: ITEM BIRAKMA
        else if (tag == "InputDepot" && currentState == AiState.DeliveringItem && !stack.IsEmpty)
        {
            if (other.TryGetComponent<GridDepot>(out GridDepot inputDepot))
            {
                if (!inputDepot.IsFull)
                {
                    GameObject item = stack.RemoveItem();
                    myAudio.PlayPickupSound();
                    if (item != null)
                    {
                        myAudio.PlayPickupSound();
                        inputDepot.AddItem(item);
                        transferTimer = 0f;
                    }
                }
            }
        }
    }
}