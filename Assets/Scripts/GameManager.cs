using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GameManager
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
            _ins = new GameManager();
        }
        return _ins;
    }
    #endregion

    public bool isGameOver = false;
    public delegate void ReceiveGameOver();
    public event ReceiveGameOver GameOver;


    public void TriggerGameOver()
    {
        isGameOver = true;
        GameOver.Invoke();
    }
}
