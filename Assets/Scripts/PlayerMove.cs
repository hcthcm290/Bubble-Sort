using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] public Vector3 direction;
    [SerializeField] public float speed;
    [SerializeField] public Rigidbody2D body;

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
            }
            else
            {
                body.bodyType = RigidbodyType2D.Dynamic;
            }
            beingDrag = value;

        }
    }
    [SerializeField]
    Vector2 offset;
    Vector3 prevMousePosition;

    [SerializeField] int type;

    public bool isInside = false;
    private Basket basket;

    [SerializeField]
    SpriteRenderer sprite;

    private Timer lifeTime;

    private Timer delayBlink;
    private Timer blink;

    // Start is called before the first frame update
    void Start()
    {
        body.velocity = direction.normalized * speed;
        lifeTime = gameObject.AddComponent<Timer>();
        lifeTime.interval = 5;

        delayBlink = gameObject.AddComponent<Timer>();
        delayBlink.interval = 5;
        delayBlink.StartCount();


        blink = gameObject.AddComponent<Timer>();
        blink.interval = 0.3f;

    }

    // Update is called once per frame
    void Update()
    {
        if(isInside)
        {
            // snap to basket
            var position = transform.position;

            Rect limit = basket.limit;

            position.x = Mathf.Clamp(position.x, limit.x - limit.width / 2, limit.x + limit.width / 2);
            position.y = Mathf.Clamp(position.y, limit.y - limit.height / 2, limit.y + limit.height / 2);

            transform.position = position;

            return;
        }

        if(beingDrag)
        {
            transform.position = offset + (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(!Input.GetMouseButton(0))
            {
                beingDrag = false;
                body.velocity = direction.normalized * speed;
            }

            if (Input.mousePosition != prevMousePosition)
            {
                direction = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)Camera.main.ScreenToWorldPoint(prevMousePosition);
                prevMousePosition = Input.mousePosition;
            }
        }
        else
        {
            body.velocity = body.velocity.normalized * speed;
        }
       
        if(delayBlink.Ready())
        {
            //start blinking
            DoBlink();

            if(lifeTime.hasStart == false)
            {
                lifeTime.StartCount();
            }
        }

        if(lifeTime.Ready())
        {
            Debug.Log("You Losed");
        }
    }

    void DoBlink()
    {
        if(blink.Ready())
        {
            blink.interval *= 0.9f;
            sprite.enabled = !sprite.enabled;
            blink.Tick();
        }
    }

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

            if(type != basket.type)
            {
                Debug.Log("You Lose");
                return;
            }

            // make it fly
            direction.x = Random.Range(-1.0f, 1.0f);
            direction.y = Random.Range(-1.0f, 1.0f);
            prevMousePosition = Input.mousePosition;

            body.velocity = direction.normalized * speed * 0.5f;

            basket.playersInside.Add(this);

            Score._ins.score += 1;
            Debug.Log("Scored");
        }
    }
}
