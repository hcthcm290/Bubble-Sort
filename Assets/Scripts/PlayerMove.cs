using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerState
{
    freeMove,
    beingDrag,
    limitedMove,
    Explode,
}

public class PlayerMove : MonoBehaviour
{
    #region Moving Property
    [SerializeField] public Vector3 direction;
    [SerializeField] public float speed;
    [SerializeField] public Rigidbody2D body;
    #endregion

    #region Draging Property
    [SerializeField]
    bool beingDrag = false;
    public bool BeingDrag
    {
        get
        {
            return beingDrag;
        }
        set
        {
            if(value == true)
            {
                body.bodyType = RigidbodyType2D.Kinematic;
                state = PlayerState.beingDrag;
            }
            else
            {
                body.bodyType = RigidbodyType2D.Dynamic;
                if(state == PlayerState.beingDrag)
                {
                    state = PlayerState.freeMove;
                }
            }
            beingDrag = value;
        }
    }
    [SerializeField]
    Vector2 offset;
    Vector3 prevMousePosition;
    #endregion

    [SerializeField] int type;

    #region Limited Move Property
    public bool isInside = false;
    private Basket basket;
    #endregion

    #region Blinking Property
    [SerializeField]
    SpriteRenderer sprite;

    private Timer lifeTime;

    private Timer delayBlink;
    private Timer blink;
    #endregion

    PlayerState state;

    // Start is called before the first frame update
    void Start()
    {
        body.velocity = direction.normalized * speed;
        lifeTime = gameObject.AddComponent<Timer>();
        lifeTime.interval = 8;

        delayBlink = gameObject.AddComponent<Timer>();
        delayBlink.interval = 8;
        delayBlink.StartCount();

        blink = gameObject.AddComponent<Timer>();
        blink.interval = 0.3f;

        GameManager.Ins().GameOver += HandleGameOver;
        state = PlayerState.freeMove;
    }

    // Update is called once per frame
    void Update()
    {
        if(state == PlayerState.freeMove)
        {
            UpdateFreeMove();
            UpdateBlinking();
        }
        else if(state == PlayerState.beingDrag)
        {
            UpdateBeingDrag();
            UpdateBlinking();
        }
        else if(state == PlayerState.limitedMove)
        {
            UpdateLimitedMove();
        }
        else if(state == PlayerState.Explode)
        {
            UpdateExplode();
        }

    }

    #region Update
    void UpdateFreeMove()
    {
        body.velocity = body.velocity.normalized * speed;
    }

    void UpdateBeingDrag()
    {
        transform.position = offset + (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.mousePosition != prevMousePosition)
        {
            direction = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)Camera.main.ScreenToWorldPoint(prevMousePosition);
            prevMousePosition = Input.mousePosition;
        }
    }

    void UpdateLimitedMove()
    {
        // snap to basket
        var position = transform.position;

        Rect limit = basket.limit;

        position.x = Mathf.Clamp(position.x, limit.x - limit.width / 2, limit.x + limit.width / 2);
        position.y = Mathf.Clamp(position.y, limit.y - limit.height / 2, limit.y + limit.height / 2);

        transform.position = position;

        body.velocity = body.velocity.normalized * speed;
    }

    void UpdateBlinking()
    {
        if (delayBlink.Ready())
        {
            //start blinking
            DoBlink();

            if (lifeTime.hasStart == false)
            {
                lifeTime.StartCount();
            }
        }

        /// Game Over ///
        /// This is when you don't drop the bubble to the right basket in time
        if (lifeTime.Ready())
        {
            GameManager.Ins().TriggerGameOver();
        }
    }

    void UpdateExplode()
    {

    }
    #endregion

    void DoBlink()
    {
        if(blink.Ready())
        {
            blink.interval *= 0.9f;
            sprite.enabled = !sprite.enabled;
            blink.Tick();
        }
    }

    #region Stuff
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "basket")
        {
            basket = collision.GetComponent<Basket>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "basket" && !isInside)
        {
            basket = null;
        }
    }

    private void OnMouseUp()
    {
        if(basket != null)
        {
            isInside = true;

            /// Game Over ///
            // this is when you drop bubble to the wrong basket
            if(type != basket.type)
            {
                GameManager.Ins().TriggerGameOver();
                return;
            }

            // make it fly
            direction.x = Random.Range(-1.0f, 1.0f);
            direction.y = Random.Range(-1.0f, 1.0f);
            prevMousePosition = Input.mousePosition;

            body.velocity = direction.normalized * speed * 0.5f;

            state = PlayerState.limitedMove;
            sprite.enabled = true;

            basket.playersInside.Add(this);

            Score._ins.score += 1;
            Debug.Log("Scored");
        }
    }
    #endregion

    public void Explode()
    {
        state = PlayerState.Explode;
        Destroy(gameObject);
    }

    public void HandleGameOver()
    {
        if(!isInside)
        {
            Explode();
        }
    }
}
