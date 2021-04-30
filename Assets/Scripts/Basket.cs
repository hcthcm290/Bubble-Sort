using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BasketState
{
    Chilling,
    MoveBubblesOut,
    MoveBubbleUp,
}

public class Basket : MonoBehaviour
{
    public Rect limit;
    public int type;

    public List<PlayerMove> playersInside;
    private List<List<GameObject>> bubbleOnPath;

    BasketState state = BasketState.Chilling;

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
    }

    // Update is called once per frame
    void Update()
    {
        

        if(state == BasketState.Chilling)
        {
            if (playersInside.Count == 10)
            {
                GameManager.Ins().Pause();

                foreach (var bubble in playersInside)
                {
                    bubble.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

                }

                state = BasketState.MoveBubblesOut;
            }
            return;
        }
        else if(state == BasketState.MoveBubblesOut)
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
                    position.x += 1.5f * Time.deltaTime;
                }
                bubble.transform.position = position;

                if(position.x <= -3.2 || position.x >= 3.2)
                {

                    removeBubbles.Add(bubble);
                    // TODO
                    // Add this bubble to result screen to display score
                }
            }

            foreach(var bubble in removeBubbles)
            {
                playersInside.Remove(bubble);

                if(bubbleOnPath[0].Count < 5)
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
}
