using UnityEngine;

public class WorldUIBillboard : MonoBehaviour
{
    private Camera _camera;
    private Transform _target;
    private Vector3 _offset;

    private void Awake()
    {
        _camera = Camera.main;
        _target = transform.parent;
        _offset = transform.localPosition;
    }

    private void LateUpdate()
    {
        if (_camera == null)
            return;

        transform.forward = _camera.transform.forward;
        transform.position = _target.position + _offset;
    }
}