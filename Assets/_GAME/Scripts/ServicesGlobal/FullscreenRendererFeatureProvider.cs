using System.Collections.Generic;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _GAME.Scripts.ServicesGlobal
{
    public class FullscreenRendererFeatureProvider : IInitializable
    {
        private FullScreenPassRendererFeature _fullScreenRendererFeature;
        
        public void Initialize()
        {
            var scriptableRenderer = 
                (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset)?.GetRenderer(0);
            var property = typeof(ScriptableRenderer).GetProperty(
                "rendererFeatures",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (property != null)
            {
                List<ScriptableRendererFeature> rendererFeatures = 
                    property.GetValue(scriptableRenderer, null) as List<ScriptableRendererFeature>;
            
                // Find the feature we want to enable:
                if (rendererFeatures != null)
                {
                    foreach (var feature in rendererFeatures)
                    {
                        if (feature.GetType() == typeof(FullScreenPassRendererFeature))
                        {
                            _fullScreenRendererFeature = feature as FullScreenPassRendererFeature;
                        }
                    }
                }
            }
        }
        
        public FullScreenPassRendererFeature Get()
        {
            return _fullScreenRendererFeature;
        }
    }
}
