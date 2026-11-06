using UnityEngine;
using Autohand;

public class FallReset : MonoBehaviour
{
    private AutoHandPlayer player;
    private Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<AutoHandPlayer>();
        startPosition = player.transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        // Si el jugador cae de la plataforma lo devolvemos a donde empezo
        if (other.GetComponentInParent<AutoHandPlayer>() == player)
        {
            player.SetPosition(startPosition);
            player.body.linearVelocity = Vector3.zero;
        }
    }
}
