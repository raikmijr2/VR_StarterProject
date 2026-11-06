using UnityEngine;

public enum CubeEffect { None, Slow, Fast, Swap }

public class cubeLife : MonoBehaviour
{
    public GameObject shatterPrefab;
    public float shatterSpeed = 2f;
    public float shatterLifetime = 1f;
    public CubeEffect effect = CubeEffect.None;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CubeKiller"))
        {
            Destroy(gameObject);
        }
    }

    public void Break()
    {
        GameObject shatter = Instantiate(shatterPrefab, transform.position, transform.rotation);
        foreach (AnimationState state in shatter.GetComponent<Animation>())
        {
            state.speed = shatterSpeed;
        }
        Destroy(shatter, shatterLifetime);
        Destroy(gameObject);
        if (UIManager.instance != null) UIManager.instance.AddScore();

        if (effect != CubeEffect.None && EffectManager.instance != null)
        {
            EffectManager.instance.Apply(effect);
        }
    }
}
