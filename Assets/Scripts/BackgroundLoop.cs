using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    [SerializeField]
    List<GameObject> listBackground;

    public float speed;

    void Start()
    {
        
    }

    void Update()
    {
        MoveBackground();
        LoopBackground();
    }

    void LateUpdate()
    {
        
    }

    void MoveBackground()
    {
        foreach(var bg in listBackground)
        {
            Vector3 position = bg.transform.position;

            position.y -= speed * Time.deltaTime;

            bg.transform.position = position;
        }
    }

    void LoopBackground()
    {
        RectTransform bottomBgTransform = (RectTransform)listBackground[0].transform;

        Vector3 bottomBgPosition = bottomBgTransform.position;
        bottomBgPosition.y += bottomBgTransform.rect.height / 2;

        if (Camera.main.WorldToScreenPoint(bottomBgPosition).y < 0)
        {
            GameObject temp = listBackground[0];

            listBackground.RemoveAt(0);

            listBackground.Add(temp);
        }

        for (int i = 1; i < listBackground.Count; i++)
        {
            RectTransform prevBg = (RectTransform)listBackground[i - 1].transform;
            RectTransform bg = (RectTransform)listBackground[i].transform;

            Vector3 position = bg.position;

            position.y = prevBg.position.y + prevBg.rect.height / 2 + bg.rect.height / 2;

            bg.transform.position = position;
        }
    }
}
