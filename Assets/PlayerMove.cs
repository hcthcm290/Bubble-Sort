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
    [SerializeField]
    Vector2 offset;
    Vector3 prevMousePosition;

    [SerializeField] int type;

    public bool isInside = false;
    private Basket basket;

    // Start is called before the first frame update
    void Start()
    {
        body.velocity = direction.normalized * speed;

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

    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButton(0) && !beingDrag && !isInside)
        {
            beingDrag = true;

            offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            body.velocity = Vector3.zero;
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

            Debug.Log("Scored");
        }
    }
}
