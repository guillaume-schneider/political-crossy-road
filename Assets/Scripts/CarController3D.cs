using UnityEngine;

public class CarController3D : MonoBehaviour
{
    [Header("Paramètres du mouvement")]
    public float speed = 5f;

    [Header("Direction de déplacement (définie par le spawner)")]
    public Vector3 direction = Vector3.forward;

    void Update()
    {
        // Avance dans la direction définie
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collision avec le joueur !");
            Destroy(gameObject);
        }
    }
}
