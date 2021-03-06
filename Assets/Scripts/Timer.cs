using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Timer: MonoBehaviour
{
    public bool hasStart;
    public float interval;
    [SerializeField] private float count;

    bool pause;

    public void StartCount()
    {
        count = 0;
        hasStart = true;
    }

    public void Update()
    {
        if (GameManager.Ins().isPause) return;
        if (pause) return;
        if (hasStart)
        {
            count += Time.deltaTime;
        }
    }

    public bool Ready()
    {
        return count > interval;
    }

    public void Tick()
    {
        count = 0;
    }

    public void Pause()
    {
        pause = true;
    }

    public void UnPause()
    {
        pause = false;
    }
}
