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
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && !GameManager.Ins().isPause)
            {
                Debug.Log("Pressed");
                Catch(0);
            }

            if(touch.phase == TouchPhase.Ended)
            {
                Release(0);
            }
        }

        //if (Input.GetMouseButtonDown(0) && !GameManager.Ins().isPause)
        //{
        //    Catch(0);
        //}

        //if (Input.GetMouseButtonUp(0))
        //{
        //    Release(0);
        //}

    }

    void Catch(int touchIndex)
    {
        RaycastHit2D target;

        Vector3 position = Camera.main.ScreenToWorldPoint(Input.GetTouch(touchIndex).position);
        position.z = -10;

        Vector3 direction = new Vector3(0, 0, 1);

        target = Physics2D.Raycast(position, direction, 50, LayerMask.GetMask("Player"));

        if(target.collider != null)
        {
            PlayerMove player = target.collider.GetComponent<PlayerMove>();

            if(player != null)
            {
                if (player.isInside == true)
                    return;

                player.offset = player.transform.position - position;
                player.touchID = touchIndex;
                player.BeingDrag = true;
                this.player = player;
            }
        }
    }

    void Release(int touchIndex)
    {
        if(player != null)
        {
            player.BeingDrag = false;
        }
        player = null;
    }
}
