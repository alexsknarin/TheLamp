using UnityEngine;

public class LampTestMover : MonoBehaviour
{
    [SerializeField] private float _speed;


    void Update()
    {
        Vector3 pos = transform.position;
        pos.x += _speed * Time.deltaTime;
        transform.position = pos;
    }
}
