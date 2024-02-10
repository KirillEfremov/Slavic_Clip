using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Rigidbody _rb;
    private Vector3 _velocity;
    private Vector3 _rotation;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Move (Vector3 velocity)
    {
        _velocity = velocity;
    }

    public void Rotate(Vector3 rotation)
    {
        _rotation = rotation;
    }
    private void FixedUpdate()
    {
        PerfrormMove();
    }

    void PerfrormMove()
    {
        if (_velocity != Vector3.zero)
            _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }
}
