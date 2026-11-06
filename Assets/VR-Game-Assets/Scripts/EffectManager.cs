using System.Collections;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    public CubePhisics cubeSpawner;
    public GunScript gun;
    public SwordScript sword;
    public float slowMultiplier = 0.5f;
    public float fastMultiplier = 1.5f;
    public float effectDuration = 5f;

    private float currentMultiplier = 1f;
    private Coroutine speedRoutine;

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gun == null) gun = FindFirstObjectByType<GunScript>();
        if (sword == null) sword = FindFirstObjectByType<SwordScript>();
    }

    public void Apply(CubeEffect effect)
    {
        switch (effect)
        {
            case CubeEffect.Slow:
                StartSpeedEffect(slowMultiplier);
                break;
            case CubeEffect.Fast:
                StartSpeedEffect(fastMultiplier);
                break;
            case CubeEffect.Swap:
                // El cambio se queda hasta que rompas otro SwapCube, no vuelve solo
                SwapWeapons();
                break;
        }
    }

    void StartSpeedEffect(float multiplier)
    {
        if (speedRoutine != null) StopCoroutine(speedRoutine);
        speedRoutine = StartCoroutine(SpeedEffect(multiplier));
    }

    IEnumerator SpeedEffect(float multiplier)
    {
        SetSpeedMultiplier(multiplier);
        yield return new WaitForSeconds(effectDuration);
        SetSpeedMultiplier(1f);
    }

    void SetSpeedMultiplier(float multiplier)
    {
        float factor = multiplier / currentMultiplier;

        foreach (string cubeTag in CubePhisics.cubeTags)
        {
            foreach (GameObject cube in GameObject.FindGameObjectsWithTag(cubeTag))
            {
                cube.GetComponent<Rigidbody>().linearVelocity *= factor;
            }
        }

        if (cubeSpawner != null) cubeSpawner.speedMultiplier = multiplier;
        currentMultiplier = multiplier;
    }

    void SwapWeapons()
    {
        Transform gunRoot = gun.transform;
        Transform swordRoot = sword.transform.parent;

        Transform gunHand = gunRoot.parent;
        gunRoot.SetParent(swordRoot.parent, false);
        swordRoot.SetParent(gunHand, false);

        // Las manos son espejo una de la otra, asi que invertimos el eje lateral de cada arma
        gunRoot.localScale = Vector3.Scale(gunRoot.localScale, new Vector3(1f, -1f, 1f));
        swordRoot.localScale = Vector3.Scale(swordRoot.localScale, new Vector3(-1f, 1f, 1f));

        OVRInput.Controller gunController = gun.controller;
        gun.controller = sword.controller;
        sword.controller = gunController;
    }
}
