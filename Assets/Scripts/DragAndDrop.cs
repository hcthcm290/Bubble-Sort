using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    PlayerMove player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !GameManager.Ins().isPause)
        {
            Catch();
        }

        if(Input.GetMouseButtonUp(0))
        {
            Release();
        }
    }

    void Catch()
    {
        RaycastHit2D target;

        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = -10;

        Vector3 direction = new Vector3(0, 0, 1);

        target = Physics2D.Raycast(position, direction, 50, LayerMask.GetMask("Player"));

        if(target.collider != null)
        {
            PlayerMove player = target.collider.GetComponent<PlayerMove>();
            if (player.isInside == true)
                return;

            if(player != null)
            {
                player.BeingDrag = true;
                this.player = player;
            }
        }
    }

    void Release()
    {
        if(player != null)
        {
            player.BeingDrag = false;
        }
        player = null;
    }
}
