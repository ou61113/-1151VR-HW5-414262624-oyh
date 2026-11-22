using UnityEngine;
using System.Collections;

public class PokemonCapture : MonoBehaviour
{
    private Vector3 originalScale;
    private bool isCaptured = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public IEnumerator CaptureAnimation()
    {
        if (isCaptured)
            yield break;

        isCaptured = true;

        // 寶可夢縮小
        float duration = 0.8f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            transform.localScale = Vector3.Lerp(
                originalScale,
                Vector3.zero,
                t
            );

            yield return null;
        }

        transform.localScale = Vector3.zero;

        // 隱藏寶可夢
        gameObject.SetActive(false);
    }

    public void ResetPokemon()
    {
        isCaptured = false;

        transform.localScale = originalScale;

        gameObject.SetActive(true);
    }
}
