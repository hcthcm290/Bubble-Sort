using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static Score _ins;

    public int score;
    public Text textScore;

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
