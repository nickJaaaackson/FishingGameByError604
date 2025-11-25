using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    public static Player Instance;
    [Header("References")]
    public Animator animator;
    public Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;
    public FishingRod fishingRod;
    public float moveSpeed = 3f;

    [Header("State Flags")]
    public bool isWalking = false;
    public bool isCasting = false;
    public bool isReeling = false;
    public bool isCatching = false;
    public bool isFrozen = false;

    [Header("Fishing System")]
    public FishingSystem fishingSystem;
    [SerializeField] private BaitData emtyBait;
    public BaitData currentBait;
   

    [SerializeField] private LayerMask interlactLayer;
    [SerializeField] private float interactRange;

    public float money = 500;

    private bool canFish = false;
    private bool canMove = true;

    private Vector2 input;

    // ===============================
    // 🔹 Update
    // ===============================

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentBait = emtyBait;
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if (isFrozen)
        {
            // ห้ามขยับ/ห้ามกดตอนตกปลา
            rb.linearVelocity = Vector2.zero;
            UpdateAnimator();
            return;
        }
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        HandleMovement();
        HandleFishingInput();
        UpdateAnimator();
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryInterract();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            FishData data = Resources.Load<FishData>("Fish/Mackerel");
            float weight = Random.Range(1f, 10f);

            Inventory.Instance.AddItem(new FishItem(data, weight));

            Debug.Log($"🧪 Added Random Fish: {data.fishName} {weight}kg");
        }
    }

    // ===============================
    // 🧭 Movement
    // ===============================

    public void SetMovementEnable(bool enable)
    {
        canMove = enable;
        rb.linearVelocity = Vector2.zero;

    }
    void HandleMovement()
    {
        input.x = Input.GetAxisRaw("Horizontal");


        bool isMoving = input.magnitude > 0;
        isWalking = isMoving;

        if (isMoving)
        {
            rb.linearVelocity = input.normalized * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
        if(input.x > 0f)
        {
            spriteRenderer.flipX = false;
          
        }
        if (input.x < 0f)
        {
            spriteRenderer.flipX = true;
            
        }
    }

    void TryInterract()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, interlactLayer);
          
        IInteractable interactable = hit.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact(this);
        }
    }

    #region Money

    public bool SpendMoney(int amount)
    {
        if (money < amount) return false;

        money -= amount;
        return true;
    }
    public void AddMoney(int amount)
    {
        money += amount;
    }
       


    #endregion

    // ===============================
    //  เริ่มตกปลา (กด E)
    // ===============================
    void HandleFishingInput()
    {
        if (canFish && Input.GetKeyDown(KeyCode.E) && currentBait != emtyBait)
        {
            if (fishingSystem != null && !isCasting && !isFrozen)
            {
                Debug.Log("🎣 Start Fishing!");
                StartCasting();

                
                fishingSystem.BeginFishingMiniGame(this, currentBait);
            }
        }
    }
    public void SetCurrentBait(BaitData baitdata)
    {
        if(baitdata == null)
        {
             currentBait =emtyBait;
        }
        else
        {
            currentBait = baitdata;
            Debug.Log("เหยื่อปัจจุบัน = " + currentBait.baitName);
        }
       
    }

    public void SetCanFish(bool value)
    {
        canFish = value;
    }


    // ===============================
    //  Animator Sync
    // ===============================
    void UpdateAnimator()
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isCasting", isCasting);
        animator.SetBool("isReeling", isReeling);
        animator.SetBool("isCatching", isCatching);
    }

    // ===============================
    //  ฟังก์ชันเปลี่ยนสถานะอนิเมชัน
    // ===============================
    public void StartCasting()
    {
        isFrozen = true;
        isWalking = false;
        isCasting = true;
        isReeling = false;
        isCatching = false;
    }

    public void StartReeling()
    {
        isCasting = false;
        isReeling = true;
    }

    public void StartCatching()
    {
        isReeling = false;
        isCatching = true;
    }

    public void UnfreezeAfterFishing()
    {
        isFrozen = false;
        isCasting = false;
        isReeling = false;
        isCatching = false;
    }
}
