using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    public Rect limit;
    public int type;

    public List<PlayerMove> playersInside;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
