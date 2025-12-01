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

    public float money = 2000;

    private bool canFish = false;
    private bool canMove = true;

    private Vector2 input;

    // ===============================
    // 🔹 Update
    // ===============================

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        fishingRod = GetComponent<FishingRod>();
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
            HUDManager.Instance.RefreshQuests();
            Debug.Log($"🧪 Added Random Fish: {data.fishName} {weight:F1}kg");
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
            rb.linearVelocity = new Vector2(input.x * moveSpeed, rb.linearVelocity.y);
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
            HUDManager.Instance.ShowInteract(false);
            interactable.Interact(this);
        }
    }

    #region Money

    public bool SpendMoney(int amount)
    {
        if (money < amount) return false;

        money -= amount;
        HUDManager.Instance.RefreshMoney();
        return true;
    }
    public void AddMoney(int amount)
    {
        money += amount;
        HUDManager.Instance.RefreshMoney();
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
        if(!isWalking && !isCasting && !isReeling &&!isCatching)
        { animator.Play("Idle"); }
    }

    // ===============================
    //  ฟังก์ชันเปลี่ยนสถานะอนิเมชัน
    // ===============================
    public void StartCasting()
    {
        isFrozen = true;
       ClearAnimFlags();
        isCasting = true;
        Debug.Log("Cast");

    }

    public void StartReeling()
    {
        ClearAnimFlags();
        isReeling = true;
        Debug.Log("Reel");
    }

    public void StartCatching()
    {
        ClearAnimFlags();
        isCatching = true;
        Debug.Log("Catch");
    }

    public void UnfreezeAfterFishing()
    {
        isFrozen = false;
        ClearAnimFlags();
    }
    public void ClearAnimFlags()
    {
        isCasting = false;
        isReeling = false;
        isCatching = false;
        isWalking =false;
    }
}
