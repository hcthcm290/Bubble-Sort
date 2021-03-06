using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class GameManager: MonoBehaviour
{
    #region SingleTon
    private static GameManager _ins;
    private GameManager()
    {

    }

    public static GameManager Ins()
    {
        return _ins;
    }

    private void Start()
    {
        _ins = this;
    }
    #endregion

    public bool isGameOver = false;
    public delegate void ReceiveGameOver();
    public event ReceiveGameOver GameOver;
    public delegate void ReceiveGamePause();
    public event ReceiveGamePause GamePause;
    public delegate void ReceiveGameContinue();
    public event ReceiveGameContinue GameContinue;
    private bool _isPause;
    public bool isPause { get { return _isPause; } }

    [SerializeField] AudioSource backgroundSound;

    private void Update()
    {
        if(isPause || isGameOver)
        {
            backgroundSound.volume = 0.2f;
        }
        else
        {
            backgroundSound.volume = 0.4f;
        }
    }

    public void TriggerGameOver(float delay)
    {
        Pause();
        StartCoroutine(TriggerRealGameOver(delay));
    }

    private IEnumerator TriggerRealGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        isGameOver = true;
        if (GameOver != null) GameOver.Invoke();
    }

    public void Pause()
    {
        _isPause = true;
        if (GamePause != null) GamePause.Invoke();
    }

    public void Unpause()
    {
        _isPause = false;
        if (GameContinue != null) GameContinue.Invoke();
    }

}
