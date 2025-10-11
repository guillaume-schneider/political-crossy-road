using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class IsometricCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                  // Le joueur à suivre

    [Header("Isometric Setup")]
    [Tooltip("Active l’orthographique pour un rendu isométrique ‘pur’.")]
    public bool useOrthographic = true;
    [Tooltip("Taille de la caméra orthographique (zoom).")]
    public float orthoSize = 8f;

    [Space(6)]
    [Tooltip("Utiliser un offset fixe au lieu d’angles isométriques.")]
    public bool useFixedOffset = false;

    [Tooltip("Offset monde (utilisé si useFixedOffset = true).")]
    public Vector3 worldOffset = new Vector3(-10f, 10f, -10f);

    [Tooltip("Distance caméra → cible (utilisée si useFixedOffset = false).")]
    public float distance = 14f;
    [Tooltip("Inclinaison X (typiquement 30–45°).")]
    [Range(0f, 89f)] public float tiltX = 35f;
    [Tooltip("Rotation autour de l’axe Y (typiquement 45° pour l’iso classique).")]
    public float yawY = 45f;

    [Header("Follow & Look")]
    [Tooltip("Vitesse de lissage du suivi (plus grand = plus réactif).")]
    public float followDamping = 10f;
    [Tooltip("Faire regarder la caméra vers la cible.")]
    public bool lookAtTarget = true;
    [Tooltip("Décalage vertical du point visé (ex: centre du torse).")]
    public float lookHeightOffset = 1.0f;

    private Camera cam;

    void OnValidate()
    {
        if (!cam) cam = GetComponent<Camera>();
        ApplyCameraMode();
    }

    void Awake()
    {
        cam = GetComponent<Camera>();
        ApplyCameraMode();
    }

    void LateUpdate()
    {
        if (!target) return;

        // Calcule la position désirée (iso par angles ou via offset fixe)
        Vector3 desiredPos;
        if (useFixedOffset)
        {
            desiredPos = target.position + worldOffset;
        }
        else
        {
            // Rotation isométrique : d’abord yaw (Y), puis tilt (X)
            Quaternion rot = Quaternion.Euler(tiltX, yawY, 0f);
            // On part d’un vecteur « derrière » et on applique la rotation
            Vector3 dir = rot * new Vector3(0f, 0f, -1f);
            desiredPos = target.position + dir.normalized * distance;
        }

        // Lissage exponentiel (indépendant du framerate)
        float t = 1f - Mathf.Exp(-followDamping * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPos, t);

        // Orientation
        if (lookAtTarget)
        {
            Vector3 lookPoint = target.position + Vector3.up * lookHeightOffset;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation((lookPoint - transform.position).normalized, Vector3.up),
                t
            );
        }
        else if (!useFixedOffset)
        {
            // Si on n’utilise pas LookAt mais qu’on est en mode angles,
            // on garde l’orientation isométrique définie par tilt/yaw :
            transform.rotation = Quaternion.Euler(tiltX, yawY, 0f);
        }

        // Applique le mode cam (utile en mode Éditeur)
        ApplyCameraMode();
    }

    private void ApplyCameraMode()
    {
        if (!cam) return;
        cam.orthographic = useOrthographic;
        if (useOrthographic) cam.orthographicSize = Mathf.Max(0.01f, orthoSize);
    }
}
