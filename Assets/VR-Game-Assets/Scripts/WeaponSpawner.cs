using System.Collections;
using UnityEngine;
using Autohand;

public class WeaponSpawner : MonoBehaviour
{
    public GameObject gunPrefab;
    public GameObject swordPrefab;

    // Misma posicion que tienen las armas en las manos de GameScene
    public Vector3 gunPosition = new Vector3(0.0305f, 0.0616f, -0.0047f);
    public Vector3 gunRotation = new Vector3(-90f, 90f, 0f);
    public Vector3 gunScale = new Vector3(-0.024238776f, 0.024238776f, 0.024238776f);
    public Vector3 swordPosition = new Vector3(0.038f, 0.316f, 0.122f);
    public Vector3 swordRotation = new Vector3(-63.33f, 0f, 0f);
    public Vector3 swordScale = Vector3.one;

    [Header("Texto del boton")]
    public string buttonLabel = "PROVA";
    public float labelHeight = 0.35f;
    public float labelFontSize = 1f;

    [Header("Cubos de prueba")]
    public GameObject gunCubePrefab;
    public GameObject swordCubePrefab;
    public GameObject swapCubePrefab;
    // Empties de la escena donde salen los cubos (si estan vacios se buscan por nombre)
    public Transform gunCubePoint;
    public Transform swordCubePoint;
    public Transform swapCubePoint;
    public float respawnDelay = 2.5f;

    private bool spawned = false;
    private GameObject[] testCubePrefabs;
    private Vector3[] testCubePositions;
    private GameObject[] testCubes;
    private bool[] respawning;
    private Quaternion testCubeRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ButtonLabel.Create(transform, buttonLabel, labelHeight, labelFontSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (testCubes == null) return;

        // Cuando rompes un cubo de prueba sale otro igual en el mismo sitio al cabo de unos segundos
        for (int i = 0; i < testCubes.Length; i++)
        {
            if (testCubePrefabs[i] != null && testCubes[i] == null && !respawning[i])
            {
                StartCoroutine(Respawn(i));
            }
        }
    }

    public void SpawnWeapons()
    {
        if (spawned) return;
        spawned = true;

        bool gunPlaced = false;
        bool swordPlaced = false;

        foreach (Hand hand in FindObjectsByType<Hand>(FindObjectsSortMode.None))
        {
            if (!hand.left && !gunPlaced)
            {
                PlaceInHand(gunPrefab, hand.transform, gunPosition, gunRotation, gunScale);
                gunPlaced = true;
            }
            else if (hand.left && !swordPlaced)
            {
                PlaceInHand(swordPrefab, hand.transform, swordPosition, swordRotation, swordScale);
                swordPlaced = true;
            }
        }

        // El SwapCube necesita un EffectManager para cambiar las armas de mano
        if (EffectManager.instance == null) gameObject.AddComponent<EffectManager>();

        SpawnTestCubes();
    }

    void PlaceInHand(GameObject prefab, Transform hand, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        GameObject weapon = Instantiate(prefab, hand);
        weapon.transform.localPosition = position;
        weapon.transform.localEulerAngles = rotation;
        weapon.transform.localScale = scale;
    }

    void SpawnTestCubes()
    {
        if (gunCubePoint == null) gunCubePoint = FindByName("pistola");
        if (swordCubePoint == null) swordCubePoint = FindByName("espasa");
        if (swapCubePoint == null) swapCubePoint = FindByName("canvi");

        testCubePrefabs = new GameObject[] { gunCubePrefab, swordCubePrefab, swapCubePrefab };
        Transform[] points = { gunCubePoint, swordCubePoint, swapCubePoint };
        testCubePositions = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null)
            {
                Debug.LogWarning("WeaponSpawner: falta el empty para " + testCubePrefabs[i].name);
                testCubePrefabs[i] = null;
                continue;
            }
            testCubePositions[i] = points[i].position;
        }

        // Los cubos miran hacia el jugador, como los de la partida
        Transform head = Camera.main != null ? Camera.main.transform : transform;
        Vector3 forward = head.forward;
        forward.y = 0f;
        testCubeRotation = forward != Vector3.zero ? Quaternion.LookRotation(-forward) : Quaternion.identity;

        testCubes = new GameObject[testCubePrefabs.Length];
        respawning = new bool[testCubePrefabs.Length];
        for (int i = 0; i < testCubes.Length; i++)
        {
            SpawnTestCube(i);
        }
    }

    void SpawnTestCube(int i)
    {
        if (testCubePrefabs[i] == null) return;
        testCubes[i] = Instantiate(testCubePrefabs[i], testCubePositions[i], testCubeRotation);
    }

    Transform FindByName(string objectName)
    {
        foreach (Transform t in FindObjectsByType<Transform>(FindObjectsSortMode.None))
        {
            if (t.name.ToLower() == objectName) return t;
        }
        return null;
    }

    IEnumerator Respawn(int i)
    {
        respawning[i] = true;
        yield return new WaitForSeconds(respawnDelay);
        SpawnTestCube(i);
        respawning[i] = false;
    }
}
