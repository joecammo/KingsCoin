using UnityEngine;
using TMPro;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;
    public float fadeDuration = 1f;
    private bool isShowing = false;

    void Start()
    {
        if (gameOverText == null)
        {
            Debug.LogError("Game Over Text component not assigned!");
        }
        
        // Hide the text initially
        gameOverText.color = new Color(gameOverText.color.r, 
                                     gameOverText.color.g, 
                                     gameOverText.color.b, 
                                     0f);
    }

    public void ShowGameOver()
    {
        if (!isShowing)
        {
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        isShowing = true;
        float elapsedTime = 0f;
        Color originalColor = gameOverText.color;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / fadeDuration;
            gameOverText.color = new Color(originalColor.r, 
                                        originalColor.g, 
                                        originalColor.b, 
                                        alpha);
            yield return null;
        }
        
        gameOverText.color = originalColor;
    }

    public void HideGameOver()
    {
        if (isShowing)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        isShowing = false;
        float elapsedTime = 0f;
        Color originalColor = gameOverText.color;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1f - (elapsedTime / fadeDuration);
            gameOverText.color = new Color(originalColor.r, 
                                        originalColor.g, 
                                        originalColor.b, 
                                        alpha);
            yield return null;
        }
        
        gameOverText.color = new Color(originalColor.r, 
                                     originalColor.g, 
                                     originalColor.b, 
                                     0f);
    }
}
