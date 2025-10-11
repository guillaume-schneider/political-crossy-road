using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private Transform _myTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _myTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
           
    }
}
