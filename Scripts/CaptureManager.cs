using UnityEngine;
using TMPro;

public class CaptureManager : MonoBehaviour
{
    [Header("Pokeball")]
    public GameObject pokeballPrefab;

    public Transform throwPoint;

    [Header("Pokemon")]
    public Transform pokemon;

    [Header("Trainer Motion")]
    public TrainerMotion trainerMotion;

    [Header("UI")]
    public TextMeshProUGUI resultText;

    private bool canThrow = true;
    private bool captureFinished = false;

    void Start()
    {
        if (resultText != null)
        {
            resultText.text = "";
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) &&
            canThrow &&
            !captureFinished)
        {
            ThrowPokeball();
        }
    }

    void ThrowPokeball()
    {
        if (pokeballPrefab == null ||
            throwPoint == null ||
            pokemon == null)
        {
            Debug.LogError("請設定必要物件！");
            return;
        }

        // 訓練家跳起，模擬拋球動作
        if (trainerMotion != null)
        {
            trainerMotion.ThrowMotion();
        }

        // 生成精靈球
        GameObject ball = Instantiate(
            pokeballPrefab,
            throwPoint.position,
            Quaternion.identity
        );

        // 啟動精靈球飛行
        PokeballController controller =
            ball.GetComponent<PokeballController>();

        if (controller != null)
        {
            controller.Launch(pokemon, this);
        }

        canThrow = false;
    }

    public void CaptureSucceeded()
    {
        captureFinished = true;

        if (resultText != null)
        {
            resultText.text = "捕捉成功！";
        }

        Debug.Log("成功收服寶可夢！");
    }
}