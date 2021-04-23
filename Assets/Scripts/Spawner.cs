using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class SpawnStructure
{
    public int score;
    public int bubbleCount;
    public float interval;
}

public class Spawner : MonoBehaviour
{
    private static int SpawnCount;

    [SerializeField]
    List<PlayerMove> playersPrefab;
    Timer spawnTimer;
    Timer delay;

    [SerializeField]
    int y_dir;

    int bubbleSpawnCount;
    bool started;

    bool notRegisteredToScore;

    [SerializeField] List<SpawnStructure> spawnStructure;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = gameObject.AddComponent<Timer>();

        delay = gameObject.AddComponent<Timer>();
        delay.interval = 1;
        delay.StartCount();

        GameManager.Ins().GamePause += HandleGamePause;
        GameManager.Ins().GameContinue += HandleGameUnpause;

        if (Score._ins == null)
        {
            notRegisteredToScore = true;
        }
        else
        {
            Score._ins.OnScoreChanged += OnScoreChanged;
        }

        OnScoreChanged(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(notRegisteredToScore)
        {
            Score._ins.OnScoreChanged += OnScoreChanged;
            notRegisteredToScore = false;
        }

        if (GameManager.Ins().isGameOver) return;
        if(!delay.Ready())
        {
            spawnTimer.Pause();
        }
        else
        {
            if (!started)
            {
                spawnTimer.UnPause();
                spawnTimer.StartCount();
                for (int i = 0; i < bubbleSpawnCount; i++)
                {
                    Spawn();
                }
                started = true;
            }
        }

        if(spawnTimer.Ready())
        {
            spawnTimer.Tick();

            if(!GameManager.Ins().isGameOver)
            {
                for(int i=0; i<bubbleSpawnCount; i++)
                {
                    Spawn();
                }
            }
        }
    }

    void Spawn()
    {
        if (playersPrefab.Count == 0) return;


        int random = Random.Range(0, playersPrefab.Count);
        PlayerMove newP = Instantiate(playersPrefab[random]);

        Vector3 position = transform.position;
        position.z = -5 + 0.02f * SpawnCount;

        newP.transform.position = position;
        newP.direction.x = Random.Range(-1.0f, 1.0f);
        newP.direction.y = y_dir*Random.Range(0.3f, 1.0f);
        
        SpawnCount++;
    }

    public void HandleGamePause()
    {
        spawnTimer.Pause();
    }

    public void HandleGameUnpause()
    {
        spawnTimer.UnPause();
    }

    public void OnScoreChanged(int score)
    {
        var structure = spawnStructure.Find(x => x.score == score);

        if (structure == null) return;

        spawnTimer.interval = structure.interval;
        bubbleSpawnCount = structure.bubbleCount;
    }
}
