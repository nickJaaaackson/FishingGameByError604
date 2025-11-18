using UnityEngine;

public class FishMiniGame : MonoBehaviour
{
    #region Inspector References
    [Header("References")]
    [SerializeField] Transform topPivot;
    [SerializeField] Transform bottomPivot;
    [SerializeField] Transform fish;
    [SerializeField] Transform hook;
    [SerializeField] Transform progressBarContainer;
    [SerializeField] SpriteRenderer hookSpriteRender;

    [Header("Base Hook Settings")]
    [SerializeField] float baseHookSize = 0.1f;
    [SerializeField] float baseHookPullPower = 0.04f;
    [SerializeField] float baseHookPower = 0.7f;
    [SerializeField] float baseGravityPower = 0.01f;
    [SerializeField] float baseDegradePower = 0.05f;

    [Header("Fish Movement Settings")]
    [SerializeField] float baseSmoothMotion = 1f;
    [SerializeField] float baseTimerMultiplier = 2f;
    [SerializeField] float minStayTime = 0.3f;
    [SerializeField] float maxStayTime = 2f;
    #endregion

    #region MiniGame Variables
    float hookPosition;
    float hookPullVelocity;
    float hookProgress;

    float fishPosition;
    float fishDestination;
    float fishTimer;
    float fishSpeed;

    float hookPullPower;
    float hookPower;
    float gravityPower;
    float degradePower;
    float smoothMotion;
    float timerMultiplier;

    bool isPlaying = false; 
    bool pause = false;

    FishData currentFish;
    Player player;
    FishingSystem system;
    float difficulty = 1f;
    float failTimer = 10f;
    private float caughtWeight;
    public float CaughtWeight => caughtWeight;
    #endregion


    #region Start MiniGame
    public void StartMiniGame(FishData fishData, Player playerRef, FishingSystem systemRef, float weight)
    {
        currentFish = fishData;
        player = playerRef;
        system = systemRef;
        caughtWeight = weight;

        ApplyDifficulty(fishData);
        ResizeHook();
        ResetState();

        isPlaying = true;
        pause = false;

        player.StartReeling();
    }
    #endregion


    #region Difficulty
    void ApplyDifficulty(FishData fishData)
    {
        float rarityMultiplier = fishData.rarity switch
        {
            Rarity.Common => 1.0f,
            Rarity.Uncommon => 1.2f,
            Rarity.Rare => 1.5f,
            Rarity.Epic => 1.8f,
            Rarity.Legendary => 2.2f,
            _ => 1.0f
        };

        float normalizedWeight = Mathf.InverseLerp(1f, 50f, caughtWeight);
        difficulty = rarityMultiplier * (1 + normalizedWeight);

        smoothMotion = baseSmoothMotion / difficulty;
        timerMultiplier = baseTimerMultiplier / difficulty;

        hookPullPower = baseHookPullPower;
        hookPower = baseHookPower;
        gravityPower = baseGravityPower;
        degradePower = baseDegradePower * difficulty;
    }
    #endregion

    #region FishMovement
    void UpdateFish()
    {
        fishTimer -= Time.deltaTime;

        if (fishTimer <= 0f)
        {
            fishTimer = Random.Range(minStayTime, maxStayTime) * timerMultiplier;
            fishDestination = Random.value;
        }

        fishPosition = Mathf.SmoothDamp(fishPosition, fishDestination, ref fishSpeed, smoothMotion);
        fish.position = Vector3.Lerp(bottomPivot.position, topPivot.position, fishPosition);
    }


    #endregion

    #region HookMovement
    void UpdateHook()
    {
        float maxPullVelocity = 0.02f;

        if (Input.GetMouseButton(0))
        {
            hookPullVelocity += hookPullPower * Time.deltaTime;
            hookPullVelocity = Mathf.Min(hookPullVelocity, maxPullVelocity);
        }

        hookPullVelocity -= gravityPower * Time.deltaTime;

        if (hookPosition - baseHookSize / 2 <= 0f && hookPullVelocity < 0f)
            hookPullVelocity = 0f;

        if (hookPosition + baseHookSize / 2 >= 1f && hookPullVelocity > 0f)
            hookPullVelocity = 0f;

        hookPosition += hookPullVelocity;
        hookPosition = Mathf.Clamp(hookPosition, baseHookSize / 2, 1 - baseHookSize / 2);

        hook.position = Vector3.Lerp(bottomPivot.position, topPivot.position, hookPosition);
    }
    #endregion

    #region Update Loop
    void Update()
    {
        if (!isPlaying) return;
        if (pause) return;

        UpdateFish();
        UpdateHook();
        UpdateProgress();
    }
    #endregion


    #region Progress & Win/Lose
    void UpdateProgress()
    {
        Vector3 ls = progressBarContainer.localScale;
        ls.y = hookProgress;
        progressBarContainer.localScale = ls;

        float min = hookPosition - baseHookSize / 2;
        float max = hookPosition + baseHookSize / 2;

        if (min < fishPosition && fishPosition < max)
            hookProgress += hookPower * Time.deltaTime;
        else
            hookProgress -= degradePower * Time.deltaTime;

        // Fail
        if (hookProgress <= 0f)
        {
            failTimer -= Time.deltaTime;
            if (failTimer <= 0f)
            {
                Lose();
                return; 
            }
        }
        else failTimer = 10f;

        // Success
        if (hookProgress >= 1f)
        {
            Win();
            return; 
        }

        hookProgress = Mathf.Clamp(hookProgress, 0f, 1f);
    }


    void Win()
    {
        if (!isPlaying) return; // 

        isPlaying = false;
        pause = true;

        player.StartCatching();
        player.UnfreezeAfterFishing();

        system.SpawnFishIcon(player.transform.position,currentFish.icon);
        system.OnMiniGameResult(true);
    }

    void Lose()
    {
        if (!isPlaying) return; 

        isPlaying = false;
        pause = true;

        player.UnfreezeAfterFishing();
        system.OnMiniGameResult(false);
    }
    #endregion


    #region Helpers
    void ResizeHook()
    {
        if (hookSpriteRender == null) return;

        Bounds b = hookSpriteRender.bounds;
        float ySize = b.size.y;
        float distance = Vector3.Distance(topPivot.position, bottomPivot.position);

        Vector3 ls = hook.localScale;
        ls.y = (distance / ySize * baseHookSize);
        hook.localScale = ls;
    }

    void ResetState()
    {
        hookPosition = 0.5f;
        fishPosition = 0.5f;
        hookProgress = 0f;
        hookPullVelocity = 0f;
        fishSpeed = 0f;
        fishDestination = 0.5f;
        failTimer = 10f;
        pause = false;
    }
    #endregion
}