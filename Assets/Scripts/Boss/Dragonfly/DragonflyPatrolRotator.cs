using UnityEngine;

public class DragonflyPatrolRotator : MonoBehaviour
{
    [SerializeField] private Transform _tiltTransform;
    [SerializeField] private Transform _spinTransform;
    [SerializeField] private Transform _distanceTransform;
    [SerializeField] private bool _isPlaying = false;
    [SerializeField] private int _direction = 1;
    [SerializeField] private float _spinSpeed;
    [SerializeField] private float _ditanceFront;
    [SerializeField] private float _distanceBack;
    [SerializeField] private float _tiltFront;
    [SerializeField] private float _tiltBack;
    [SerializeField] private float _speedFrontMultipler;
    [SerializeField] private float _speedBackMultipler;
    [Header("--- Debug ---")]
    [SerializeField] private float _fronBackMask = 0;
    
    public void Play()
    {
        _isPlaying = true;
    }
    
    public void Stop()
    {
        _isPlaying = false;
    }
    
    public void SetRotationPhase(Vector3 position)
    {
        Vector3 pos2d = position;
        pos2d.y = 0;
        pos2d.Normalize();
        float angle = Vector3.Angle(Vector3.back, pos2d);
        if (pos2d.x > 0)
        {
            angle = -angle;
        }
        
        _spinTransform.localEulerAngles = new Vector3(0, angle, 0);
        AdjustPosition();
    }
    

    private void Update()
    {
        if (_isPlaying)
        {
            Spin(_fronBackMask);
            AdjustPosition();
        }
    }
    
    private void Spin(float mask)
    {
        float speedMultiplier = Mathf.Lerp(_speedBackMultipler, _speedFrontMultipler, mask);
        _spinTransform.Rotate(Vector3.up, _spinSpeed * _direction * speedMultiplier * Time.deltaTime);
    }
    
    private void AdjustPosition()
    {
        _fronBackMask = Mathf.Abs((_spinTransform.localEulerAngles.y - 180)/180f);
        float maskSmoothPower = Mathf.Lerp(2f, 0.2f,_fronBackMask);
        _fronBackMask = Mathf.Pow(_fronBackMask, maskSmoothPower);
        Tilt(_fronBackMask);
        KeepDistance(_fronBackMask);  
    }

    private void Tilt(float mask)
    {
        float tiltAngle = Mathf.Lerp(_tiltBack, _tiltFront, mask);
        Vector3 tilt = _tiltTransform.localEulerAngles;
        tilt.x = tiltAngle;
        _tiltTransform.localEulerAngles = tilt;
    }
    
    private void KeepDistance(float mask)
    {
        float distance = Mathf.Lerp(_distanceBack, _ditanceFront, mask);
        Vector3 position = _distanceTransform.localPosition;
        position.z = distance;
        _distanceTransform.localPosition = position;
    }
}
