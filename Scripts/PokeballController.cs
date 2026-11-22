using UnityEngine;
using System.Collections;

public class PokeballController : MonoBehaviour
{
    public Transform target;

    [Header("Flight Settings")]
    public float flightDuration = 1.2f;
    public float arcHeight = 2.5f;
    public float rotateSpeed = 720f;

    [Header("Capture Effect")]
    public GameObject captureEffectPrefab;

    private bool isFlying = false;
    private bool hasHit = false;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private float flightTimer = 0f;

    private CaptureManager captureManager;

    public void Launch(Transform pokemon, CaptureManager manager)
    {
        target = pokemon;
        captureManager = manager;

        startPosition = transform.position;
        endPosition = target.position;

        flightTimer = 0f;
        isFlying = true;
    }

    void Update()
    {
        if (!isFlying || target == null)
            return;

        flightTimer += Time.deltaTime;

        float t = flightTimer / flightDuration;

        t = Mathf.Clamp01(t);

        // 水平方向與基本高度的移動
        Vector3 position = Vector3.Lerp(
            startPosition,
            endPosition,
            t
        );

        // 加入拋物線高度
        float height = Mathf.Sin(t * Mathf.PI) * arcHeight;

        position.y += height;

        transform.position = position;

        // 精靈球飛行時旋轉
        transform.Rotate(
            Vector3.right * rotateSpeed * Time.deltaTime,
            Space.World
        );

        // 飛行結束
        if (t >= 1f)
        {
            transform.position = endPosition;

            isFlying = false;

            if (!hasHit)
            {
                hasHit = true;

                StartCoroutine(CaptureSequence());
            }
        }
    }

    IEnumerator CaptureSequence()
    {
        Debug.Log("擊中寶可夢！");

        // 第一次播放：擊中寶可夢的粒子特效
        PlayCaptureEffect(target.position);

        // 寶可夢縮小並消失
        PokemonCapture pokemonCapture =
            target.GetComponent<PokemonCapture>();

        if (pokemonCapture != null)
        {
            yield return StartCoroutine(
                pokemonCapture.CaptureAnimation()
            );
        }
        else
        {
            target.gameObject.SetActive(false);
        }

        Debug.Log("寶可夢已被吸入精靈球！");

        // 精靈球落地
        Vector3 groundPosition = new Vector3(
            transform.position.x,
            0.35f,
            transform.position.z
        );

        Vector3 startPosition = transform.position;

        float fallDuration = 0.5f;
        float timer = 0f;

        while (timer < fallDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fallDuration;

            transform.position = Vector3.Lerp(
                startPosition,
                groundPosition,
                t
            );

            yield return null;
        }

        transform.position = groundPosition;

        // 精靈球搖晃三次
        yield return StartCoroutine(ShakeBall());

        Debug.Log("精靈球停止搖晃！");

        // 第二次播放：捕捉完成的粒子特效
        PlayCaptureEffect(transform.position);

        // 等待特效出現
        yield return new WaitForSeconds(0.5f);

        Debug.Log("收服成功！");

        // 顯示捕捉成功 UI
        if (captureManager != null)
        {
            captureManager.CaptureSucceeded();
        }
    }

    void PlayCaptureEffect(Vector3 position)
    {
        if (captureEffectPrefab == null)
            return;

        GameObject effect = Instantiate(
            captureEffectPrefab,
            position,
            Quaternion.identity
        );

        ParticleSystem[] systems =
            effect.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem ps in systems)
        {
            ps.Play();
        }

        // 特效播放完後自動刪除
        Destroy(effect, 3f);
    }

    IEnumerator ShakeBall()
    {
        // 每次晃動的時間
        float shakeDuration = 0.25f;

        // 晃動三次
        for (int i = 0; i < 3; i++)
        {
            float timer = 0f;

            Quaternion originalRotation = transform.rotation;

            while (timer < shakeDuration)
            {
                timer += Time.deltaTime;

                float angle = Mathf.Sin(timer * 30f) * 20f;

                transform.rotation = originalRotation *
                    Quaternion.Euler(0f, 0f, angle);

                yield return null;
            }

            transform.rotation = originalRotation;

            // 每次晃動後停頓
            yield return new WaitForSeconds(0.2f);
        }
    }
}