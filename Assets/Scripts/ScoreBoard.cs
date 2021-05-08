using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] List<Text> listTxtScore;
    [SerializeField] Text lowTxtScore;
    [SerializeField] Text txtHighScore;
    bool activated;
    int highlightIndex;

    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerPrefs.HasKey("ScoreCount"))
        {
            PlayerPrefs.SetInt("ScoreCount", listTxtScore.Count);
        }
        else
        {
            if (listTxtScore.Count > PlayerPrefs.GetInt("ScoreCount"))
            {
                PlayerPrefs.SetInt("ScoreCount", listTxtScore.Count);
            }
        }

        for(int i=0; i< PlayerPrefs.GetInt("ScoreCount"); i++)
        {
            if(PlayerPrefs.HasKey("Score" + i))
            {
                listTxtScore[i].text = PlayerPrefs.GetInt("Score" + i).ToString();
            }
            else
            {
                PlayerPrefs.SetInt("Score" + i, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate(int currentScore)
    {
        List<int> scores = new List<int>();

        for(int i=0; i< PlayerPrefs.GetInt("ScoreCount"); i++)
        {
            scores.Add(PlayerPrefs.GetInt("Score" + i));
        }

        int index = scores.Count;

        for (int i = scores.Count - 1; i >= 0; i--)
        {
            if (currentScore > scores[i])
                index = i;
        }
        
        if(index == scores.Count) // if this score is the lowest in board, add it to list
        {
            scores.Add(currentScore);
        }
        else // else, insert it to the right place
        {
            scores.Insert(index, currentScore);
        }

        // update the score on UI
        highlightIndex = index;

        for (int i = 0; i < listTxtScore.Count; i++)
        {
            listTxtScore[i].text = scores[i].ToString();
        }

        if(highlightIndex < listTxtScore.Count)
        {
            listTxtScore[highlightIndex].color = Color.yellow;
            listTxtScore[highlightIndex].fontSize = 50;
        }
        else
        {
            lowTxtScore.text = currentScore.ToString();
        }

        // Update the scores in player prefabs
        for (int i = 0; i < PlayerPrefs.GetInt("ScoreCount"); i++)
        {
            PlayerPrefs.SetInt("Score" + i, scores[i]);
        }

        StartCoroutine(ShowTheScore());

        activated = true;
    }

    private IEnumerator ShowTheScore()
    {
        txtHighScore.enabled = true;

        for(int i = 0; i < listTxtScore.Count; i++)
        {
            yield return new WaitForSeconds(0.5f);
            listTxtScore[i].gameObject.SetActive(true);
            listTxtScore[i].enabled = true;
        }

        if(highlightIndex >= listTxtScore.Count)
        {
            yield return new WaitForSeconds(0.5f);
            lowTxtScore.enabled = true;

            // blinking effect
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.6f);
                lowTxtScore.enabled = false;
                yield return new WaitForSeconds(0.6f);
                lowTxtScore.enabled = true;
            }

            yield return new WaitForSeconds(1.0f);
            SceneManager.LoadScene("TitleScene");
        }
        else
        {
            // blinking effect
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.6f);
                listTxtScore[highlightIndex].enabled = false;

                yield return new WaitForSeconds(0.6f);
                listTxtScore[highlightIndex].enabled = true;
            }

            yield return new WaitForSeconds(1.0f);
            SceneManager.LoadScene("TitleScene");
        }


    }
}
