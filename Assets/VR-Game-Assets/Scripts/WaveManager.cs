using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public CubePhisics cubeSpawner;
    public float[] waveDurations = { 20f, 40f, 90f };
    [HideInInspector] public float waveDuration;
    public float timeBetweenWaves = 3f;
    public int totalWaves = 3;
    public float lastWaveStartSpeed = 5f;
    public float lastWaveEndSpeed = 9f;

    public int currentWave = 0;
    public float timeLeft;
    public bool waveFinished = true;
    public bool gameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartNextWave();
    }

    // Update is called once per frame
    void Update()
    {
        if (waveFinished) return;

        timeLeft -= Time.deltaTime;

        if (currentWave == totalWaves)
        {
            cubeSpawner.speed = Mathf.Lerp(lastWaveStartSpeed, lastWaveEndSpeed, 1f - timeLeft / waveDuration);
        }

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndWave();
        }
    }

    void StartNextWave()
    {
        currentWave++;
        waveDuration = waveDurations[currentWave - 1];
        timeLeft = waveDuration;
        waveFinished = false;

        switch (currentWave)
        {
            case 1:
                cubeSpawner.spawnInterval = 3.5f;
                cubeSpawner.speed = 3f;
                cubeSpawner.maxCubesAtOnce = 1;
                cubeSpawner.spawnSwordCubes = false;
                cubeSpawner.specialCubeChance = 0f;
                break;
            case 2:
                cubeSpawner.spawnInterval = 2.5f;
                cubeSpawner.speed = 3.5f;
                cubeSpawner.maxCubesAtOnce = 2;
                cubeSpawner.spawnSwordCubes = true;
                cubeSpawner.specialCubeChance = 0f;
                break;
            case 3:
                cubeSpawner.spawnInterval = 1.5f;
                cubeSpawner.speed = lastWaveStartSpeed;
                cubeSpawner.maxCubesAtOnce = 2;
                cubeSpawner.spawnSwordCubes = true;
                cubeSpawner.specialCubeChance = 0.2f;
                break;
        }

        cubeSpawner.StartSpawning();
    }

    void EndWave()
    {
        waveFinished = true;
        cubeSpawner.StopSpawning();
        cubeSpawner.DestroyAllCubes();

        if (currentWave < totalWaves)
        {
            Invoke("StartNextWave", timeBetweenWaves);
        }
        else
        {
            gameOver = true;
            UIManager.instance.EndGame();
        }
    }
}
