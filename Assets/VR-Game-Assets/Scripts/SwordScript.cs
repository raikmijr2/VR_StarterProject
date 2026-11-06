using System.Collections;
using UnityEngine;
using EzySlice;

public class SwordScript : MonoBehaviour
{
    public Transform bladeBase;
    public Transform bladeTip;
    public Material crossSectionMaterial;
    public float cutForce = 1.5f;
    public float pieceLifetime = 2f;
    public OVRInput.Controller controller = OVRInput.Controller.LTouch;
    public float vibrationAmplitude = 1f;
    public float vibrationDuration = 0.15f;
    public GameObject slashVFX;
    public float vfxLifetime = 2f;
    public AudioClip sliceSound;

    private Vector3 lastTipPosition;
    private Vector3 tipVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastTipPosition = bladeTip.position;
    }

    void FixedUpdate()
    {
        tipVelocity = (bladeTip.position - lastTipPosition) / Time.fixedDeltaTime;
        lastTipPosition = bladeTip.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("cubeSword"))
        {
            Slice(other.gameObject);
        }
        else if (other.CompareTag("SwapCube"))
        {
            other.GetComponent<cubeLife>().Break();
            StartCoroutine(Vibrate());
        }
        else if (other.CompareTag("cubeGun"))
        {
            // Cubo de pistola cortado con la espada: penalizacion
            Destroy(other.gameObject);
            if (UIManager.instance != null) UIManager.instance.RemoveScore();
        }
    }

    void Slice(GameObject target)
    {
        Vector3 bladeDirection = bladeTip.position - bladeBase.position;
        Vector3 cutDirection = tipVelocity - target.GetComponent<Rigidbody>().linearVelocity;
        Vector3 planeNormal = Vector3.Cross(bladeDirection, cutDirection).normalized;

        SlicedHull hull = target.Slice(bladeTip.position, planeNormal, crossSectionMaterial);
        if (hull == null) return;

        SetupPiece(hull.CreateUpperHull(target, crossSectionMaterial), planeNormal);
        SetupPiece(hull.CreateLowerHull(target, crossSectionMaterial), -planeNormal);

        GameObject vfx = Instantiate(slashVFX, target.transform.position, Quaternion.LookRotation(cutDirection));
        Destroy(vfx, vfxLifetime);

        AudioSource.PlayClipAtPoint(sliceSound, target.transform.position);

        Destroy(target);
        StartCoroutine(Vibrate());
        if (UIManager.instance != null) UIManager.instance.AddScore();
    }

    void SetupPiece(GameObject piece, Vector3 pushDirection)
    {
        piece.AddComponent<MeshCollider>().convex = true;
        Rigidbody rb = piece.AddComponent<Rigidbody>();
        rb.AddForce(pushDirection * cutForce, ForceMode.Impulse);
        Destroy(piece, pieceLifetime);
    }

    IEnumerator Vibrate()
    {
        OVRInput.SetControllerVibration(1f, vibrationAmplitude, controller);
        yield return new WaitForSeconds(vibrationDuration);
        OVRInput.SetControllerVibration(0f, 0f, controller);
    }
}
