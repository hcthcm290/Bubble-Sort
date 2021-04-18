using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GameManager: MonoBehaviour
{
    #region SingleTon
    private static GameManager _ins;
    private GameManager()
    {

    }

    public static GameManager Ins()
    {
        if(_ins == null)
        {
            GameObject gameObject = new GameObject("Game Manager");
            _ins = gameObject.AddComponent<GameManager>();
        }
        return _ins;
    }
    #endregion

    public bool isGameOver = false;
    public delegate void ReceiveGameOver();
    public event ReceiveGameOver GameOver;
    public delegate void ReceiveGamePause();
    public event ReceiveGamePause GamePause;
    public delegate void ReceiveGameContinue();
    public event ReceiveGameContinue GameContinue;


    public void TriggerGameOver(float delay)
    {
        GamePause.Invoke();
        StartCoroutine(TriggerRealGameOver(delay));
    }

    private IEnumerator TriggerRealGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        isGameOver = true;
        GameOver.Invoke();

    }
}
