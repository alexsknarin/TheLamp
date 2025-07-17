using System.Collections.Generic;
using _GAME.Scripts.Enemies.Generic.Presentation;
using _GAME.Scripts.Enemies.Ladybug;
using UnityEngine;
using UnityEngine.VFX;

namespace _GAME.Scripts.Enemies.Megabeetle
{
    public class MegabeetleDamageFlash : DamageIndication
    {
        private static readonly int DamageFade = Shader.PropertyToID("_DamageFade");
        private static readonly int Phase = Shader.PropertyToID("_Phase");
        [SerializeField] private List<MeshRenderer> _meshRenderer;
        [SerializeField] private MegabeetleMovement _movement;
        [SerializeField] private GameObject _damageEnergy;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private VisualEffect _damageParticles;
        [SerializeField] private MegabeetleDamageFXRotationHandler _damageFXRotationHandler;
        private List<Material> _bodyMaterials = new ();
        private Material _damageEnergyMaterial;
        private float _localTime;
        private bool _isStick;

        public override void Initialize()
        {
            _damageFXRotationHandler.Initialize();
            foreach (var meshRenderer in _meshRenderer)
            {
                if (meshRenderer == null) continue;
                var material = meshRenderer.material;
                _bodyMaterials.Add(material);
                material.SetFloat(DamageFade, 0f);
            }

            _movement.StickStarted += OnStickStarted;
            _movement.StickEnded += OnStickEnded;
            
            enabled = false;
            _damageEnergyMaterial = _damageEnergy.GetComponent<MeshRenderer>().material;
            _damageEnergy.SetActive(false);
        }

        private void OnDestroy()
        {
            _movement.StickStarted -= OnStickStarted;
            _movement.StickEnded -= OnStickEnded;
        }

        public override void Play()
        {
            _damageFXRotationHandler.Play();
            _damageEnergy.SetActive(true);
            enabled = true;
            _localTime = 0;
            _isStick = false;
        
            Vector3 direction = transform.position.normalized;
            _damageParticles.SetVector3("Direction", direction);
            _damageParticles.SendEvent("OnDamage");
        }

        private void Update()
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                enabled = false;
                SetDamageMaterialPhase(0);
                _damageFXRotationHandler.Stop();
                _damageEnergy.SetActive(false);   
                return;
            }
            SetDamageMaterialPhase(1 - phase);
            _damageEnergyMaterial.SetFloat(Phase, Mathf.Clamp01(phase*1.45f));
            _localTime += Time.deltaTime;
        }

        private void SetDamageMaterialPhase(float phase)
        {
            for (int i=0; i < _bodyMaterials.Count; i++)
            {
                _bodyMaterials[i].SetFloat(DamageFade, phase);
            }
        }

        private void OnStickEnded()
        {
            _isStick = false;
        }

        private void OnStickStarted()
        {
            _isStick = true;
        }
        
    }
}
