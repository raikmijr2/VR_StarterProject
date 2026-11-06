using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class BulletScript : MonoBehaviour
{
    public float lifetime = 5f;
    public float gravityScale = 0.3f;
    [HideInInspector] public GunScript gun;

    private Rigidbody rb;
    private Vector3 lastPosition;
    private bool hasHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        lastPosition = rb.position;
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        // Gravedad propia para que el balin caiga menos que el resto de objetos
        rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);

        // Los cubos son triggers y el balin va rapido, asi que miramos el tramo recorrido para que no los atraviese sin detectarlos
        Vector3 travel = rb.position - lastPosition;
        foreach (RaycastHit hit in Physics.RaycastAll(lastPosition, travel.normalized, travel.magnitude, ~0, QueryTriggerInteraction.Collide))
        {
            TryHit(hit.collider);
        }
        lastPosition = rb.position;
    }

    void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    void TryHit(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("cubeGun") || other.CompareTag("SwapCube"))
        {
            hasHit = true;
            other.GetComponent<cubeLife>().Break();
            if (gun != null) gun.OnBulletHit();
            Destroy(gameObject);
        }
        else if (other.CompareTag("cubeSword"))
        {
            // Cubo de espada disparado con la pistola: penalizacion
            hasHit = true;
            Destroy(other.gameObject);
            if (UIManager.instance != null) UIManager.instance.RemoveScore();
            Destroy(gameObject);
        }
        else if (other.TryGetComponent(out Button button) && button.interactable)
        {
            // Disparar a un boton de UI (con collider) es como pulsarlo, p.ej. el de Back to Lobby
            hasHit = true;
            button.onClick.Invoke();
            Destroy(gameObject);
        }
    }
}
