using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollBubbleScore : MonoBehaviour
{
    private static RollBubbleScore _ins;
    public static RollBubbleScore Ins
    {
        get { return _ins; }
    }

    public delegate void Finish();
    public event Finish OnRollingFinish;

    public List<List<GameObject>> bubblePaths;
    bool started = false;
    [SerializeField] List<Transform> listStartPosition;
    [SerializeField] ScoreBoard scoreBoard;

    [SerializeField] List<Animator> listConveyor;
    [SerializeField] AudioSource movingSound;

    private void Start()
    {
        _ins = this;
        bubblePaths = new List<List<GameObject>>();
        bubblePaths.Add(new List<GameObject>());
        bubblePaths.Add(new List<GameObject>());
        bubblePaths.Add(new List<GameObject>());
        bubblePaths.Add(new List<GameObject>());
    }

    private void Update()
    {
        if (!started) return;

        int count = 0;

        foreach(var path in bubblePaths)
        {
            path.RemoveAll(x => x == null);

            if (path.Count == 0)
                count++;

            foreach(var bubble in path)
            {
                Vector3 position = bubble.transform.position;
                position.y += 2.75f * Time.deltaTime;
                bubble.transform.position = position;
            }
        }

        if(count == 4 && started) // all path clear its bubble
        {
            movingSound.Stop();
            OnRollingFinish?.Invoke();
            started = false;

            if(GameManager.Ins().isGameOver)
            {
                scoreBoard.Activate(Score._ins.realScore);
            }
            for (int i = 0; i < listConveyor.Count; i++)
            {
                listConveyor[i].enabled = false;
            }
        }
    }

    public void SetBubbleOnPath(List<List<GameObject>> bubblesOnPath)
    {
        if (bubblesOnPath.Count == 2)
        {
            bubblePaths[1] = bubblesOnPath[0];
            bubblePaths[2] = bubblesOnPath[1];

            bubblePaths[0].Clear();
            bubblePaths[3].Clear();
        }
        else if (bubblesOnPath.Count == 4)
        {
            for (int i = 0; i < 4; i++)
            {
                if(bubblesOnPath[i] != null)
                {
                    bubblePaths[i] = bubblesOnPath[i];
                }
            }
        }
        else
        {
            throw new Exception("Bubble on paths only accept 2 kind, 2 paths and 4 paths");
        }

        InitializeBubblePosition();
    }

    public void StartRolling()
    {
        if(!started)
        {
            movingSound.Play();
        }

        started = true;

        for(int i=0; i<bubblePaths.Count; i++)
        {
            if (i >= listConveyor.Count) continue;
            if (bubblePaths[i].Count > 0)
            {
                listConveyor[i].enabled = true;
            }
        }
    }

    private void InitializeBubblePosition()
    {
        for(int iPath = 0; iPath < 4; iPath++)
        {
            for(int ibb = 0; ibb < bubblePaths[iPath].Count; ibb++)
            {
                var bubble = bubblePaths[iPath][ibb];
                bubble.transform.position = listStartPosition[iPath].position - new Vector3(0, 1f, 0) * ibb;
                bubble.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
                bubble.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                bubble.GetComponent<SpriteRenderer>().sortingOrder = 2;

                Vector3 scale = bubble.transform.localScale;
                scale.x = 0.5f;
                scale.y = 0.5f;

                bubble.transform.localScale = scale;
            }
        }
    }
}
