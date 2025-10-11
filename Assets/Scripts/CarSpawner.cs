using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Prefab et Spawn")]
    public GameObject carPrefab;
    public Transform spawnPoint;

    [Header("Paramètres de spawn")]
    public float spawnInterval = 2f;
    public float carLifetime = 10f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnCar();
            timer = 0f;
        }
    }

    void SpawnCar()
    {
        if (carPrefab == null) return;

        Vector3 position = spawnPoint ? spawnPoint.position : transform.position;
        Quaternion rotation = spawnPoint ? spawnPoint.rotation : Quaternion.identity;

        GameObject newCar = Instantiate(carPrefab, position, rotation);

        // Définir la direction dans le controller
        CarController3D carController = newCar.GetComponent<CarController3D>();
        if (carController != null)
        {
            carController.direction = spawnPoint ? spawnPoint.forward : Vector3.forward;
        }

        // Durée de vie gérée par le spawner
        Destroy(newCar, carLifetime);
    }
}
