using System.Collections.Generic;
using UnityEngine;

public class CubePhisics : MonoBehaviour
{
    public static string[] cubeTags = { "cubeGun", "cubeSword", "SwapCube" };

    public GameObject cubePrefab;
    public GameObject swordCubePrefab;
    public GameObject[] specialCubePrefabs;
    public float specialCubeChance = 0f;
    public float spawnInterval = 1f;
    public float speed = 2f;
    public float speedMultiplier = 1f;
    public int maxCubesAtOnce = 1;
    public bool spawnSwordCubes = true;
    public Transform spawnPoint;
    public float randomRangeX = 1f;
    public float randomRangeY = 0.5f;
    public float minDistanceBetweenCubes = 0.6f;

    public void StartSpawning()
    {
        CancelInvoke("SpawnCube");
        InvokeRepeating("SpawnCube", 0f, spawnInterval);
    }

    public void StopSpawning()
    {
        CancelInvoke("SpawnCube");
    }

    public void DestroyAllCubes()
    {
        foreach (string cubeTag in cubeTags)
        {
            foreach (GameObject cube in GameObject.FindGameObjectsWithTag(cubeTag))
            {
                Destroy(cube);
            }
        }
    }

    void SpawnCube()
    {
        int count = Random.Range(1, maxCubesAtOnce + 1);
        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < count; i++)
        {
            Vector3 position = RandomSpawnPosition(usedPositions);
            usedPositions.Add(position);
            Vector3 direction = spawnPoint.forward;

            GameObject prefab = cubePrefab;
            if (spawnSwordCubes && Random.value < 0.5f)
            {
                prefab = swordCubePrefab;
            }
            if (specialCubePrefabs.Length > 0 && Random.value < specialCubeChance)
            {
                prefab = specialCubePrefabs[Random.Range(0, specialCubePrefabs.Length)];
            }

            GameObject cube = Instantiate(prefab, position, spawnPoint.rotation);
            cube.GetComponent<Rigidbody>().linearVelocity = direction * speed * speedMultiplier;
        }
    }

    Vector3 RandomSpawnPosition(List<Vector3> usedPositions)
    {
        Vector3 position = spawnPoint.position;

        // Probamos varias posiciones para que dos cubos no salgan uno encima del otro
        for (int attempt = 0; attempt < 10; attempt++)
        {
            float x = Random.Range(-randomRangeX, randomRangeX);
            float y = Random.Range(-randomRangeY, randomRangeY);
            position = spawnPoint.position + spawnPoint.right * x + spawnPoint.up * y;

            bool farEnough = true;
            foreach (Vector3 used in usedPositions)
            {
                if (Vector3.Distance(position, used) < minDistanceBetweenCubes) farEnough = false;
            }
            if (farEnough) break;
        }

        return position;
    }
}
