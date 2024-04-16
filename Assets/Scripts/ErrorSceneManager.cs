using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Globalization;
using UnityEngine.SceneManagement;
using TMPro;

public class ErrorSceneManager : MonoBehaviour
{
    public TMP_Text rebootingText;
    private static bool isJumpscareActive = false;

    void Start()
    {
        if (!isJumpscareActive)
        {
            // Set the flag to true
            isJumpscareActive = true;

            // Sound first
            GetComponent<AudioSource>().Play();

            // Blinking coroutine
            StartCoroutine(BlinkText(rebootingText, 0.5f));

            // Return to main game after a delay
            Invoke("CloseErrorScene", 5.0f);
        }
    }

    IEnumerator BlinkText(TMP_Text text, float interval)
    {
        bool isVisible = true;
        while (true)
        {
            isVisible = !isVisible; // Toggle visibility state
            text.alpha = isVisible ? 1.0f : 0.0f; // Set alpha to 1 or 0
            yield return new WaitForSeconds(interval);
        }
    }

    void CloseErrorScene()
    {
        SceneManager.UnloadSceneAsync("JumpscareScene_(WindowError)");

        isJumpscareActive = false;

        // stop blinking when closing the scene
        StopAllCoroutines();

        Time.timeScale = 1;
    }

    public static void TriggerJumpscare()
    {
        if(!isJumpscareActive)
        {
            SceneManager.LoadSceneAsync("JumpscareScene_(WindowError)", LoadSceneMode.Additive);
        }
    }
}
