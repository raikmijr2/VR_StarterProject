using System.Collections;
using UnityEngine;
using Autohand;

public class GunScript : MonoBehaviour
{
    public Transform nozzle;
    public GameObject bulletPrefab;
    public float shootForce = 40f;
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    public AudioClip shootSound;
    public float vibrationAmplitude = 1f;
    public float vibrationDuration = 0.15f;
    public float recoilDistance = 0.03f;
    public float recoilAngle = 12f;
    public float recoilDuration = 0.15f;

    private Coroutine recoilRoutine;

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        AudioSource.PlayClipAtPoint(shootSound, nozzle.position);

        Vector3 shootDirection = nozzle.localToWorldMatrix.MultiplyVector(Vector3.forward).normalized;

        GameObject bullet = Instantiate(bulletPrefab, nozzle.position, Quaternion.LookRotation(shootDirection));
        bullet.GetComponent<BulletScript>().gun = this;

        // El balin sale pegado a la mano, asi que ignoramos los colliders del jugador para que no lo desvien hacia un lado
        Collider bulletCollider = bullet.GetComponent<Collider>();
        foreach (Collider playerCollider in transform.root.GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(bulletCollider, playerCollider);
        }

        bullet.GetComponent<Rigidbody>().AddForce(shootDirection * shootForce, ForceMode.Impulse);

        if (recoilRoutine != null) StopCoroutine(recoilRoutine);
        recoilRoutine = StartCoroutine(Recoil(shootDirection));
    }

    IEnumerator Recoil(Vector3 shootDirection)
    {
        // La pistola es hija de la mano, asi que movemos la mano y la pistola va con ella
        Hand hand = GetComponentInParent<Hand>();
        if (hand == null) yield break;

        Transform pivot = GetRecoilPivot(hand);
        Transform follow = pivot.parent;

        // Hacia atras por el cañon y levantando el cañon hacia la parte de arriba de la pistola
        Vector3 gunUp = transform.localToWorldMatrix.MultiplyVector(Vector3.forward).normalized;
        Quaternion kickWorld = Quaternion.AngleAxis(recoilAngle, Vector3.Cross(shootDirection, gunUp).normalized);

        Vector3 kickPosition = follow.InverseTransformVector(-shootDirection * recoilDistance);
        Quaternion kickRotation = Quaternion.Inverse(follow.rotation) * kickWorld * follow.rotation;

        float time = 0f;
        while (time < recoilDuration)
        {
            time += Time.deltaTime;
            float t = time / recoilDuration;
            pivot.localPosition = Vector3.Lerp(kickPosition, Vector3.zero, t);
            pivot.localRotation = Quaternion.Slerp(kickRotation, Quaternion.identity, t);
            yield return null;
        }

        pivot.localPosition = Vector3.zero;
        pivot.localRotation = Quaternion.identity;
    }

    Transform GetRecoilPivot(Hand hand)
    {
        // La mano de AutoHand sigue el punto "follow" del mando. Le ponemos un hijo
        // que es el que movemos con el recoil, y hacemos que la mano siga a ese hijo.
        if (hand.follow.name == "RecoilPivot") return hand.follow;

        Transform pivot = new GameObject("RecoilPivot").transform;
        pivot.SetParent(hand.follow, false);
        hand.follow = pivot;
        return pivot;
    }

    public void OnBulletHit()
    {
        StartCoroutine(Vibrate());
    }

    IEnumerator Vibrate()
    {
        OVRInput.SetControllerVibration(1f, vibrationAmplitude, controller);
        yield return new WaitForSeconds(vibrationDuration);
        OVRInput.SetControllerVibration(0f, 0f, controller);
    }
}
