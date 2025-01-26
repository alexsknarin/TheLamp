using UnityEngine;

public class MothlingMovementStateFactory
{
    private Transform _cameraTransform;
    private IPosition2DProvider _position2DProvider;
    
    public MothlingMovementStateFactory(Transform cameraTransform)
    {
        _cameraTransform = cameraTransform;
    }
    
    public void SetEnemyDependencies(IPosition2DProvider position2DProvider)
    {
        _position2DProvider = position2DProvider;
    }
    
    
    public FMothlingMovementStateBase CreateEnterState(FMothlingMovementStateBase state)
    {
        return new FMothlingMovementEnterState(_position2DProvider);
    }
    
    
    
}
