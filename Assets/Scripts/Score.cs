using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static Score _ins;

    private int _score;
    public int score
    {
        get { return _score; }
        set
        {
            _score = value;
            OnScoreChanged.Invoke(score);
        }
    }
    public Text textScore;

    public delegate void ScoreChange(int currentScore);
    public event ScoreChange OnScoreChanged;
        

    // Start is called before the first frame update
    void Start()
    {
        _ins = this;
    }

    // Update is called once per frame
    void Update()
    {
        textScore.text = score.ToString();
    }
}
