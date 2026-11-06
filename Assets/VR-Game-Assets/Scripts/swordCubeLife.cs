using UnityEngine;

public class swordCubeLife : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CubeKiller"))
        {
            Destroy(gameObject);
        }
    }
}
