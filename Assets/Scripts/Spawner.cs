using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    List<PlayerMove> playersPrefab;
    Timer spawnTimer;

    [SerializeField]
    int y_dir;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = gameObject.AddComponent<Timer>();
        spawnTimer.interval = 3;
        spawnTimer.StartCount();

        GameManager.Ins().GamePause += HandleGamePause;
        GameManager.Ins().GameContinue += HandleGameUnpause;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Ins().isGameOver) return;


        if(spawnTimer.Ready())
        {
            spawnTimer.Tick();

            if(!GameManager.Ins().isGameOver)
            {
                Spawn();
            }
        }
    }

    void Spawn()
    {
        if (playersPrefab.Count == 0) return;
        int random = Random.Range(0, playersPrefab.Count);
        PlayerMove newP = Instantiate(playersPrefab[random]);
        newP.transform.position = transform.position;
        newP.direction.x = Random.Range(-1.0f, 1.0f);
        newP.direction.y = y_dir*Random.Range(0.5f, 1.0f);

    }

    public void HandleGamePause()
    {
        spawnTimer.Pause();
    }

    public void HandleGameUnpause()
    {
        spawnTimer.UnPause();
    }
}
