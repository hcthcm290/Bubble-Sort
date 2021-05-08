using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum BasketState
{
    Chilling,
    TempScore,
    FinalScore,
}

public class Basket : MonoBehaviour
{
    public Rect limit;
    public int type;

    public List<PlayerMove> playersInside;
    private List<List<GameObject>> bubbleOnPath;

    BasketState state = BasketState.Chilling;

    bool notRegisterToGM = true;

    [SerializeField] Basket anotherBasket;
    [SerializeField] AudioSource whistle;
    [SerializeField] Text score;

    // Start is called before the first frame update
    void Start()
    {
        var collider = GetComponent<BoxCollider2D>();
        if(collider != null)
        {
            limit.center = transform.position;
            limit.width = collider.bounds.size.x;
            limit.height = collider.bounds.size.y;
        }

        bubbleOnPath = new List<List<GameObject>>();
        bubbleOnPath.Add(new List<GameObject>());
        bubbleOnPath.Add(new List<GameObject>());

        if(GameManager.Ins() != null)
        {
            GameManager.Ins().GameOver += OnGameOver;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(state == BasketState.Chilling)
        {
            if (score != null)
            {
                score.text = playersInside.Count.ToString() + "/20";
            }

            if (playersInside.Count == 20)
            {
                GameManager.Ins().Pause();

                foreach (var bubble in playersInside)
                {
                    bubble.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

                }

                state = BasketState.TempScore;
                whistle.Play();
            }
            return;
        }
        else if(state == BasketState.TempScore)
        {
            List<PlayerMove> removeBubbles = new List<PlayerMove>();

            foreach(var bubble in playersInside)
            {
                Vector3 position = bubble.transform.position;
                if (type == 1) // red basket, move bubble to the left
                {

                    position.x -= 1.5f * Time.deltaTime;
                }
                else if(type == 0)  // blue basket, move bubble to the right
                {
                    position.x += 1.2f * Time.deltaTime;
                }
                bubble.transform.position = position;

                if(position.x <= -3.2 || position.x >= 3.2)
                {

                    removeBubbles.Add(bubble);
                }
            }

            foreach(var bubble in removeBubbles)
            {
                playersInside.Remove(bubble);

                if(bubbleOnPath[0].Count < 10)
                {
                    bubbleOnPath[0].Add(bubble.gameObject);
                    Destroy(bubble); // This function don't destroy game object, it just destroy the PlayerMove script
                }
                else
                {
                    bubbleOnPath[1].Add(bubble.gameObject);
                    Destroy(bubble); // This function don't destroy game object, it just destroy the PlayerMove script
                }
            }

            if(playersInside.Count == 0)
            {
                StartCoroutine(StartCountPoint());
            }

            return;
        }
        else if(state == BasketState.FinalScore)
        {
            playersInside.RemoveAll(x => x == null);
            List<PlayerMove> removeBubbles = new List<PlayerMove>();

            foreach (var bubble in playersInside)
            {
                if(bubble == null)
                {
                    Debug.Log("csacsacsacsaca");
                }
                Vector3 position = bubble.transform.position;
                if (type == 1) // red basket, move bubble to the left
                {

                    position.x -= 1.5f * Time.deltaTime;
                }
                else if (type == 0)  // blue basket, move bubble to the right
                {
                    position.x += 1.5f * Time.deltaTime;
                }
                bubble.transform.position = position;

                if (position.x <= -3.2 || position.x >= 3.2)
                {

                    removeBubbles.Add(bubble);
                }
            }

            foreach (var bubble in removeBubbles)
            {
                playersInside.Remove(bubble);

                if (playersInside.Count % 2 == 0)
                {
                    if(type == 1)
                    {
                        bubbleOnPath[0].Add(bubble.gameObject);
                    }
                    else
                    {
                        bubbleOnPath[2].Add(bubble.gameObject);
                    }
                    
                }
                else
                {
                    if (type == 1)
                    {
                        bubbleOnPath[1].Add(bubble.gameObject);
                    }
                    else
                    {
                        bubbleOnPath[3].Add(bubble.gameObject);
                    }
                }
                Destroy(bubble); // This function don't destroy game object, it just destroy the PlayerMove script
            }

            if (playersInside.Count == 0 && type == 1)
            {
                RollBubbleScore.Ins.SetBubbleOnPath(bubbleOnPath);
                state = BasketState.Chilling;

            }
            else if(playersInside.Count == 0 && type == 0 && anotherBasket.playersInside.Count == 0)
            {
                RollBubbleScore.Ins.SetBubbleOnPath(bubbleOnPath);
                RollBubbleScore.Ins.StartRolling();
                state = BasketState.Chilling;
            }

            return;
        }
    }

    IEnumerator StartCountPoint()
    {
        state = BasketState.Chilling;
        RollBubbleScore.Ins.SetBubbleOnPath(bubbleOnPath);
        RollBubbleScore.Ins.StartRolling();

        yield return new WaitForSeconds(7);
        GameManager.Ins().Unpause();
    }

    public void Explode(float delay)
    {
        StartCoroutine(RealExplode(delay));
    }

    private IEnumerator RealExplode(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var player in playersInside)
        {
            player.Explode();
        }
    }

    void OnGameOver()
    {
        StartCoroutine(OnGameOverDelay());
    }

    IEnumerator OnGameOverDelay()
    {
        yield return new WaitForSeconds(0.6f);

        state = BasketState.FinalScore;
        bubbleOnPath.Clear();
        for (int i = 0; i < 4; i++)
        {
            bubbleOnPath.Add(null);
        }

        if (type == 0)
        {
            bubbleOnPath[2] = new List<GameObject>();
            bubbleOnPath[3] = new List<GameObject>();
        }
        else if (type == 1)
        {
            bubbleOnPath[0] = new List<GameObject>();
            bubbleOnPath[1] = new List<GameObject>();
        }

        playersInside.RemoveAll(x => x == null);

        foreach (var bubble in playersInside)
        {
            bubble.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
    }
}
