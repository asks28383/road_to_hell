using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("场景名称")]
    public string mainScene = "MainBattleScene";
    public string dreamScene = "DreamScene";

    [Header("过渡设置")]
    public float fadeDuration = 1.0f;
    public Animator fadeAnimator;

    private GameState savedState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 保存游戏状态
    public void SaveGameState(GameObject player, GameObject boss)
    {
        savedState = new GameState
        {
            playerPosition = player.transform.position,
            playerHealth = player.GetComponent<PlayerHealth>().currentHealth,
            bossPosition = boss.transform.position,
            bossHealth = boss.GetComponent<EnemyHealth>().currentHealth,
            bossPhase = boss.GetComponent<BossPhaseController>().currentPhase.Value
        };
    }

    // 加载游戏状态
    public void LoadGameState(GameObject player, GameObject boss)
    {
        if (savedState == null) return;

        player.transform.position = savedState.playerPosition;
        player.GetComponent<PlayerHealth>().currentHealth = savedState.playerHealth;
        boss.transform.position = savedState.bossPosition;
        boss.GetComponent<EnemyHealth>().currentHealth = savedState.bossHealth;
        boss.GetComponent<BossPhaseController>().currentPhase.Value = savedState.bossPhase;
    }

    // 切换到梦境场景
    public void EnterDreamWorld(GameObject player, GameObject boss)
    {
        SaveGameState(player, boss);
        StartCoroutine(TransitionCoroutine(dreamScene));
    }

    // 返回主场景
    public void ExitDreamWorld()
    {
        StartCoroutine(TransitionCoroutine(mainScene));
    }

    private IEnumerator TransitionCoroutine(string sceneName)
    {
        //// 淡出效果
        //if (fadeAnimator != null)
        //    fadeAnimator.SetTrigger("FadeOut");

        yield return new WaitForSeconds(fadeDuration);

        // 加载新场景
        SceneManager.LoadScene(sceneName);

        //// 淡入效果
        //if (fadeAnimator != null)
        //    fadeAnimator.SetTrigger("FadeIn");
    }
}

[System.Serializable]
public class GameState
{
    public Vector3 playerPosition;
    public float playerHealth;
    public Vector3 bossPosition;
    public float bossHealth;
    public int bossPhase;
}