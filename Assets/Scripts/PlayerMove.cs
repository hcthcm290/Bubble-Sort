using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerState
{
    freeMove,
    beingDrag,
    limitedMove,
    Explode,
    Freeze,
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

                beforeDragZValue = transform.position.z;
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
    public Vector2 offset;
    public int touchID;
    Vector3 prevMousePosition;
    float beforeDragZValue;

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

    float blinkShow = 0.15f;
    #endregion

    PlayerState state;
    PlayerState prevState;

    Vector3 prevVelocity;

    [SerializeField] Animator splodeAnimator;
    [SerializeField] float delayGameOver;

    bool notRegisterToGM = true;

    // Start is called before the first frame update
    void Start()
    {
        body.velocity = direction.normalized * speed;
        lifeTime = gameObject.AddComponent<Timer>();
        lifeTime.interval = 2;

        delayBlink = gameObject.AddComponent<Timer>();
        delayBlink.interval = 8;
        delayBlink.StartCount();

        blink = gameObject.AddComponent<Timer>();
        blink.interval = blinkShow;

        if(GameManager.Ins() != null)
        {
            GameManager.Ins().GameOver += HandleGameOver;
            GameManager.Ins().GamePause += HandleGamePause;
            GameManager.Ins().GameContinue += HandleGameUnpause;

            notRegisterToGM = false;
        }
        
        state = PlayerState.freeMove;
    }

    // Update is called once per frame
    void Update()
    {
        if(notRegisterToGM)
        {
            GameManager.Ins().GameOver += HandleGameOver;
            GameManager.Ins().GamePause += HandleGamePause;
            GameManager.Ins().GameContinue += HandleGameUnpause;

            notRegisterToGM = true;
        }

        if(GameManager.Ins().isPause && state != PlayerState.Freeze)
        {
            HandleGamePause();
        }

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
        else if(state == PlayerState.Freeze)
        {
            return;
        }

    }

    #region Update
    void UpdateFreeMove()
    {
        body.velocity = body.velocity.normalized * speed;
    }

    void UpdateBeingDrag()
    {
        Vector3 positon = offset + (Vector2)Camera.main.ScreenToWorldPoint(Input.GetTouch(touchID).position);
        positon.z = -6;
        transform.position = positon;

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
            if (!blink.hasStart)
                blink.StartCount();

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
            StartCoroutine(HandleTimeout());
        }
    }

    void UpdateExplode()
    {
        body.velocity = Vector3.zero;
    }
    #endregion

    void DoBlink()
    {
        if (blink.Ready())
        {
            blink.interval *= 0.9f;
            sprite.enabled = !sprite.enabled;
            blink.Tick();
        }
    }

    #region Stuff
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state != PlayerState.beingDrag) return;
        if(collision.gameObject.tag == "basket")
        {
            basket = collision.GetComponent<Basket>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (state != PlayerState.beingDrag) return;
        if (collision.gameObject.tag == "basket" && !isInside)
        {
            basket = null;
        }
    }

    private void OnMouseUp()
    {
        if(basket != null)
        {
            /// Game Over ///
            // this is when you drop bubble to the wrong basket
            if(type != basket.type)
            {
                StartCoroutine(HandleWrongBasket());
                return;
            }

            Vector3 position = transform.position;
            position.z += 20;
            transform.position = position;

            isInside = true;

            // make it fly
            direction.x = Random.Range(-1.0f, 1.0f);
            direction.y = Random.Range(-1.0f, 1.0f);
            prevMousePosition = Input.mousePosition;

            body.velocity = direction.normalized * speed * 0.5f;

            state = PlayerState.limitedMove;
            sprite.enabled = true;

            basket.playersInside.Add(this);

            Score._ins.score = Score._ins.score + 1;
        }
        else
        {
            Vector3 position = transform.position;
            position.z = beforeDragZValue;
            transform.position = position;
        }
    }
    #endregion

    public void Explode()
    {
        state = PlayerState.Explode;
        if(sprite != null) sprite.enabled = true;
        if(splodeAnimator != null) splodeAnimator.enabled = true;
    }

    public void HandleGamePause()
    {
        if (state == PlayerState.Explode) return;

        prevState = state;
        state = PlayerState.Freeze;

        prevVelocity = body.velocity;
        body.velocity = Vector3.zero;
    }

    public void HandleGameUnpause()
    {
        prevState = state;
        state = prevState;

        body.velocity = prevVelocity;
    }

    public void Destroy()
    {
        GameManager.Ins().GameOver -= HandleGameOver;
        GameManager.Ins().GamePause -= HandleGamePause;
        GameManager.Ins().GameContinue -= HandleGameUnpause;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if(GameManager.Ins() != null)
        {
            GameManager.Ins().GameOver -= HandleGameOver;
            GameManager.Ins().GamePause -= HandleGamePause;
            GameManager.Ins().GameContinue -= HandleGameUnpause;
        }
    }

    public void HandleGameOver()
    {
        if(!isInside)
        {
            Explode();
        }
    }

    private IEnumerator HandleTimeout()
    {
        sprite.enabled = true;

        yield return new WaitForSeconds(0.5f);
        GameManager.Ins().TriggerGameOver(delayGameOver);

        Explode();
    }

    private IEnumerator HandleWrongBasket()
    {
        sprite.enabled = true;
        GameManager.Ins().Pause();
        yield return new WaitForSeconds(0.5f);

        GameManager.Ins().TriggerGameOver(delayGameOver);
        Explode();
        basket.Explode(delayGameOver - 0.5f);

    }
}
