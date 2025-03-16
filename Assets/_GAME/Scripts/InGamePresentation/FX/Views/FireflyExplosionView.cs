using UnityEngine;

public class FireflyExplosionView: IInitializable, IDisposable
{
    private FXFactory _fxFactory;
    private FireflyExplosionViewModel _fireflyExplosionViewModel;
    private FireflyExplosion _fireflyExplosion = null;
    
    public FireflyExplosionView(FXFactory fxFactory, FireflyExplosionViewModel fireflyExplosionViewModel)
    {
        _fxFactory = fxFactory;
        _fireflyExplosionViewModel = fireflyExplosionViewModel;
    }
    
    public void Initialize()
    {
        _fireflyExplosionViewModel.FireflyExplosionLoadRequested += OnFireflyExplosionLoadRequested;
        _fireflyExplosionViewModel.FireflyExplosionStarted += OnFireflyExplosionStarted;
    }

    public void Dispose()
    {
        _fireflyExplosionViewModel.FireflyExplosionLoadRequested += OnFireflyExplosionLoadRequested;
        _fireflyExplosionViewModel.FireflyExplosionStarted += OnFireflyExplosionStarted;
    }

    private void OnFireflyExplosionLoadRequested()
    {
        _fxFactory.Load(typeof(FireflyExplosion));
    }

    private void OnFireflyExplosionStarted(Vector2 position)
    {
        if (_fireflyExplosion ==null)
        {
            _fireflyExplosion = _fxFactory.GetFireflyExplosion();
        }
        _fireflyExplosion.Play(position);
    }
}
