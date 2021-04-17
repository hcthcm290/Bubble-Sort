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
    private float count;

    public void StartCount()
    {
        count = 0;
        hasStart = true;
    }

    public void Update()
    {
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
        count -= interval;
    }

}
