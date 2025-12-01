using Unity.Mathematics;
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


    [Header("Fish Movement Settings")]
    [SerializeField] float baseSmoothMotion = 1;
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
    float failTimer = 2f;
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
        FishingRod rod = Player.Instance.fishingRod;


        float rarityMoveMultiplier = fishData.rarity switch
        {
            Rarity.Common => 1.0f,
            Rarity.Uncommon => 1.1f,
            Rarity.Rare => 1.25f,
            Rarity.Epic => 1.4f,
            Rarity.Legendary => 1.6f,
            _ => 1.0f
        };


        float w = Mathf.InverseLerp(1f, 50f, caughtWeight);


        float weightDifficulty = Mathf.Lerp(0.15f, 0.85f, w);

        float weightPullFactor = Mathf.Lerp(1f, 0.4f, w);
        hookPower = baseHookPower * weightPullFactor;

        degradePower = weightDifficulty * rod.tensionSpeed;
        degradePower = Mathf.Clamp(degradePower, 0.05f, 0.20f);

        minStayTime = Mathf.Lerp(0.5f, 0.15f, rarityMoveMultiplier - 1f);
        maxStayTime = Mathf.Lerp(1.2f, 0.45f, rarityMoveMultiplier - 1f);

        float weatherDebuffMultiplier = 1f;
        if (GameManager.Instance.isStorm)
        {
            weatherDebuffMultiplier = 1.12f;
        }
        smoothMotion = (baseSmoothMotion / rarityMoveMultiplier) / weatherDebuffMultiplier;
        timerMultiplier = (baseSmoothMotion / rarityMoveMultiplier) / weatherDebuffMultiplier;


        baseHookSize = rod.hookArea;
        hookPullPower = baseHookPullPower;
        hookPower = baseHookPower;
        gravityPower = baseGravityPower;

        Debug.Log("w=" + w + "  D=" + degradePower);
    }


    #endregion

    #region FishMovement
    void UpdateFish()
    {
        fishTimer -= Time.deltaTime;

        if (fishTimer <= 0f)
        {
            fishTimer = UnityEngine.Random.Range(minStayTime, maxStayTime) * timerMultiplier;
            fishDestination = UnityEngine.Random.value;
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


        float bottom = baseHookSize / 2f;
        if (hookPosition <= bottom)
        {
            hookPosition = bottom;
            if (hookPullVelocity < 0f)
            {
                hookPullVelocity = 0f;
            }
        }


        float top = 1f - baseHookSize / 2f;
        if (hookPosition >= top)
        {
            hookPosition = top;
            if (hookPullVelocity > 0f)
            {
                hookPullVelocity = 0f;
            }
        }

        hookPullVelocity = Mathf .Clamp(hookPullVelocity, -0.015f, 0.015f);
        
        hookPosition += hookPullVelocity ;
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


        if (hookProgress <= 0f)
        {
            failTimer -= Time.deltaTime;
            if (failTimer <= 0f)
            {
                Lose();
                return;
            }
        }
        else failTimer = 2f;


        if (hookProgress >= 1f)
        {
            Win();
            return;
        }

        hookProgress = Mathf.Clamp(hookProgress, 0f, 1f);
    }


    void Win()
    {
        if (!isPlaying) return;

        isPlaying = false;
        pause = true;

        player.StartCatching();
        player.UnfreezeAfterFishing();

        system.SpawnFishIcon(player.transform.position, currentFish.icon);
        HUDManager.Instance.RefreshQuests();
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
