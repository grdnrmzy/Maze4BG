using System;
using System.Collections;
using Maze;
using UnityEngine;

public class Logic : MonoBehaviour
{
    public static Logic Instance;
    public CanvasGroup loadScreen;
    public Action OnLogicStart;
    public Action OnLogicPause;
    public Action OnLogicContinue;

    private void Awake()
    {
        Instance = this;
        MazeGenerator.Instance.OnMazeGenerated += OnMazeGenerated;
    }

    private void OnDestroy()
    {
        MazeGenerator.Instance.OnMazeGenerated -= OnMazeGenerated;
    }

    private void OnMazeGenerated()
    {
        StartCoroutine(FadeCoroutine(1f, 0f, loadScreen, callback: StartLogic));
    }

    private void StartLogic()
    {
        OnLogicStart?.Invoke();
    }

    public void PauseLogic()
    {
        OnLogicPause?.Invoke();
    }

    public void ContinueLogic()
    {
        OnLogicContinue?.Invoke();
    }

    public void Finish()
    {
        Player.Instance.Finish();
        StartCoroutine(FadeCoroutine(0f, 1f, loadScreen, 1.5f, ResetLogic));
    }

    private void ResetLogic()
    {
        Player.Instance.ResetPlayer();
        MazeGenerator.Instance.GenerateMaze();
    }

    public IEnumerator FadeCoroutine(float from, float to, CanvasGroup screen, float cooldown = 0f, Action callback = null)
    {
        yield return new WaitForSeconds(cooldown);

        float time = 0;

        while (time < 1)
        {
            yield return new WaitForSeconds(0.008f);

            time += 0.05f;
            if (screen) screen.alpha = Mathf.Lerp(from, to, time);
        }

        callback?.Invoke();
    }
}
