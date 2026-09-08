using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PowerStruggleController : MonoBehaviour
{
    private enum StrugglePhase
    {
        PlayerStruggle,
        CousinCounterattack,
        Finished
    }


    [Header("Visual References")]

    // 上方：表哥的能量球
    public Transform cousinEnergyBall;

    // 下方：主角的能量球
    public Transform playerEnergyBall;

    // 中间会向上、向下移动的横向分界线
    public Transform boundaryLine;

    // 玩家占优时，分界线应到达的上方位置
    public Transform playerAdvancePoint;

    // 主角失败时，分界线应到达的下方位置
    public Transform playerDefeatPoint;


    [Header("Click Settings")]

    [Range(40, 60)]
    public int counterattackTriggerClicks = 50;

    // 每次点击带来的瞬间膨胀幅度
    [Range(0.01f, 0.20f)]
    public float clickPulseAmount = 0.06f;

    // 点击膨胀恢复到基础大小的速度
    public float pulseReturnSpeed = 0.8f;

    // 页面刚出现时短暂屏蔽输入，避免翻页点击被误计数
    public float inputDelay = 0.25f;


    [Header("Scale Settings")]

    // 达到反击触发点时，主角能量球相对初始大小
    public float playerPeakScaleMultiplier = 1.8f;

    // 玩家反抗阶段，表哥能量球可轻微缩小
    public float cousinResistanceScaleMultiplier = 0.9f;

    // 最终失败时，主角能量球相对初始大小
    public float playerDefeatScaleMultiplier = 0.45f;

    // 最终胜利时，表哥能量球相对初始大小
    public float cousinVictoryScaleMultiplier = 2.0f;


    [Header("Counterattack Settings")]

    // 表哥把力量线压到底部所需时间
    public float counterattackDuration = 3.5f;

    // 失败音效后，等待多久进入下一页
    public float finishDelay = 1.0f;


    [Header("Audio")]

    // 播放点击、增长、压制和失败等单次音效
    public AudioSource oneShotAudioSource;

    // 专门播放双方力量碰撞循环音效
    public AudioSource collisionLoopAudioSource;

    public AudioClip continuousClickSound;
    public AudioClip energyGrowthSound;
    public AudioClip collisionLoopSound;
    public AudioClip suppressionSound;
    public AudioClip failureSound;

    // 每隔多少次点击播放一次能量增长音效
    [Min(1)]
    public int growthSoundInterval = 6;


    [Header("Finish Event")]

    // 在 Inspector 中连接下一页的显示方法或按钮逻辑
    public UnityEvent onInteractionFinished;


    [Header("Runtime Debug")]

    [SerializeField]
    private int totalClicks;

    [SerializeField]
    private StrugglePhase currentPhase;


    private Vector3 playerInitialScale;
    private Vector3 cousinInitialScale;
    private Vector3 boundaryInitialLocalPosition;

    private Vector3 playerBaseScale;
    private Vector3 cousinBaseScale;
    private Vector3 boundaryBaseLocalPosition;

    private float playerPulse;
    public float cousinPulseVice;

    private float inputEnableTime;
    private bool referencesReady;


    private void Awake()
    {
        referencesReady = CheckReferences();

        if (!referencesReady)
        {
            enabled = false;
            return;
        }

        playerInitialScale = playerEnergyBall.localScale;
        cousinInitialScale = cousinEnergyBall.localScale;
        boundaryInitialLocalPosition = boundaryLine.localPosition;
    }


    private void OnEnable()
    {
        if (!referencesReady)
        {
            return;
        }

        ResetInteraction();
    }


    private void Update()
    {
        if (!referencesReady)
        {
            return;
        }

        // 旧版 Input Manager 下，鼠标左键和手机触屏都会进入这里。
        if (
            currentPhase != StrugglePhase.Finished
            && Time.time >= inputEnableTime
            && Input.GetMouseButtonDown(0)
        )
        {
            RegisterClick();
        }

        playerPulse = Mathf.MoveTowards(
            playerPulse,
            0f,
            pulseReturnSpeed * Time.deltaTime
        );

        cousinPulseVice = Mathf.MoveTowards(
            cousinPulseVice,
            0f,
            pulseReturnSpeed * Time.deltaTime
        );

        ApplyVisuals();
    }


    private bool CheckReferences()
    {
        bool ready = true;

        if (cousinEnergyBall == null)
        {
            Debug.LogError("PowerStruggleController 没有连接 Cousin Energy Ball");
            ready = false;
        }

        if (playerEnergyBall == null)
        {
            Debug.LogError("PowerStruggleController 没有连接 Player Energy Ball");
            ready = false;
        }

        if (boundaryLine == null)
        {
            Debug.LogError("PowerStruggleController 没有连接 Boundary Line");
            ready = false;
        }

        if (playerAdvancePoint == null)
        {
            Debug.LogError("PowerStruggleController 没有连接 Player Advance Point");
            ready = false;
        }

        if (playerDefeatPoint == null)
        {
            Debug.LogError("PowerStruggleController 没有连接 Player Defeat Point");
            ready = false;
        }

        return ready;
    }


    private void ResetInteraction()
    {
        StopAllCoroutines();
        StopCollisionLoop();

        currentPhase = StrugglePhase.PlayerStruggle;
        totalClicks = 0;
        playerPulse = 0f;
        inputEnableTime = Time.time + inputDelay;

        playerBaseScale = playerInitialScale;
        cousinBaseScale = cousinInitialScale;
        boundaryBaseLocalPosition = boundaryInitialLocalPosition;

        ApplyVisuals();
    }


    private void RegisterClick()
    {
        totalClicks++;

        PlayOneShot(continuousClickSound);

        // 反击开始后仍保留微弱点击反馈，但不再增加主角力量。
        // 因此玩家无论点击多快，都无法改变固定失败结果。
        if (currentPhase == StrugglePhase.CousinCounterattack)
        {
            playerPulse = Mathf.Max(
                playerPulse,
                clickPulseAmount * 0.35f
            );

            return;
        }

        // pulse value set up
        playerPulse = Mathf.Max(
            playerPulse,
            clickPulseAmount
        );

        // pulse value for cousin
        cousinPulseVice = Mathf.Min(
            cousinPulseVice,
            -clickPulseAmount*1.3f
        );


        float progress = Mathf.Clamp01(
            (float)totalClicks / counterattackTriggerClicks
        );

        float easedProgress = 1f - Mathf.Pow(1f - progress, 2f);

        playerBaseScale = Vector3.Lerp(
            playerInitialScale,
            playerInitialScale * playerPeakScaleMultiplier,
            easedProgress
        );

        cousinBaseScale = Vector3.Lerp(
            cousinInitialScale,
            cousinInitialScale * cousinResistanceScaleMultiplier,
            easedProgress
        );

        boundaryBaseLocalPosition = Vector3.Lerp(
            boundaryInitialLocalPosition,
            playerAdvancePoint.localPosition,
            easedProgress
        );

        if (
            growthSoundInterval > 0
            && totalClicks % growthSoundInterval == 0
        )
        {
            PlayOneShot(energyGrowthSound);
        }

        if (totalClicks >= counterattackTriggerClicks)
        {
            StartCoroutine(CousinCounterattack());
        }
    }


    private IEnumerator CousinCounterattack()
    {
        currentPhase = StrugglePhase.CousinCounterattack;

        Vector3 counterStartPlayerScale = playerBaseScale;
        Vector3 counterStartCousinScale = cousinBaseScale;
        Vector3 counterStartBoundaryPosition = boundaryBaseLocalPosition;

        Vector3 playerDefeatScale =
            playerInitialScale * playerDefeatScaleMultiplier;

        Vector3 cousinVictoryScale =
            cousinInitialScale * cousinVictoryScaleMultiplier;

        StartCollisionLoop();

        bool suppressionSoundPlayed = false;
        float timer = 0f;

        while (timer < counterattackDuration)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(
                timer / counterattackDuration
            );

            float smoothProgress =
                progress * progress * (3f - 2f * progress);

            playerBaseScale = Vector3.Lerp(
                counterStartPlayerScale,
                playerDefeatScale,
                smoothProgress
            );

            cousinBaseScale = Vector3.Lerp(
                counterStartCousinScale,
                cousinVictoryScale,
                smoothProgress
            );

            boundaryBaseLocalPosition = Vector3.Lerp(
                counterStartBoundaryPosition,
                playerDefeatPoint.localPosition,
                smoothProgress
            );

            if (!suppressionSoundPlayed && progress >= 0.55f)
            {
                suppressionSoundPlayed = true;
                PlayOneShot(suppressionSound);
            }

            yield return null;
        }

        playerBaseScale = playerDefeatScale;
        cousinBaseScale = cousinVictoryScale;
        boundaryBaseLocalPosition = playerDefeatPoint.localPosition;

        ApplyVisuals();
        StopCollisionLoop();
        PlayOneShot(failureSound);

        currentPhase = StrugglePhase.Finished;

        yield return new WaitForSeconds(finishDelay);

        onInteractionFinished?.Invoke();
    }


    private void ApplyVisuals()
    {
        playerEnergyBall.localScale =
            playerBaseScale * (1f + playerPulse); // player size getting bigger

        
        cousinEnergyBall.localScale = cousinBaseScale * (1f + cousinPulseVice);   // cousin's size stay same

        boundaryLine.localPosition = boundaryBaseLocalPosition;

    }


    private void PlayOneShot(AudioClip clip)
    {
        if (oneShotAudioSource != null && clip != null)
        {
            oneShotAudioSource.PlayOneShot(clip);
        }
    }


    private void StartCollisionLoop()
    {
        if (
            collisionLoopAudioSource == null
            || collisionLoopSound == null
        )
        {
            return;
        }

        collisionLoopAudioSource.clip = collisionLoopSound;
        collisionLoopAudioSource.loop = true;
        collisionLoopAudioSource.Play();
    }


    private void StopCollisionLoop()
    {
        if (collisionLoopAudioSource != null)
        {
            collisionLoopAudioSource.Stop();
            collisionLoopAudioSource.loop = false;
        }
    }


    private void OnDisable()
    {
        StopCollisionLoop();
    }
}
