using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using SceneNode = Unity.VectorGraphics.SceneNode;

namespace Softwyx.CareerLog.Map.Svg;

/// <summary>Progressively coarser SVG tessellation until mesh fits Unity’s 16-bit index limit.</summary>
internal static class SvgQualityLadder{
    /// <summary>Unity mesh indices are ushort; stay below 65535 vertices.</summary>
    private const int VertexLimit = 65535;

    private const float PixelsPerUnit      = 1f;
    private const int   GradientResolution = 32;
    private const bool  FlipVerticalAxis   = true;

    private readonly struct Tier{
        private readonly float _stepDistance;
        private readonly float _maxCordDeviation;
        private readonly float _maxTanAngleDeviation;
        private readonly float _samplingStepSize;

        internal Tier(float step, float cord, float angle, float sample){
            _stepDistance         = step;
            _maxCordDeviation     = cord;
            _maxTanAngleDeviation = angle;
            _samplingStepSize     = sample;
        }

        internal VectorUtils.TessellationOptions ToOptions(){
            return new VectorUtils.TessellationOptions{
                                                          StepDistance         = _stepDistance,
                                                          MaxCordDeviation     = _maxCordDeviation,
                                                          MaxTanAngleDeviation = _maxTanAngleDeviation,
                                                          SamplingStepSize     = _samplingStepSize
                                                      };
        }
    }

    // Fine → coarse; tuned for debrief ground-layer SVGs (not shared with other mods).
    private static readonly Tier[] Tiers =[
                                              // UHQ
                                              new(1.5f, 0.20f, 0.20f, 0.04f),
                                              // HQ
                                              new(3f, 0.40f, 0.35f, 0.08f),
                                              // FQ
                                              new(6f, 0.80f, 0.60f, 0.16f),
                                              // LQ
                                              new(12f, 1.5f, 1.0f, 0.32f),
                                              // Just go play solitaire already!
                                              new(24f, 3.0f, 1.5f, 0.64f)
                                          ];

    public static Sprite Rasterize(
        Scene scene, Dictionary<SceneNode, float> nodeOpacity, Rect viewBox, string sourcePathForLogs
    ){
        if(scene == null) return null;

        for(var tierIndex = 0; tierIndex < Tiers.Length; tierIndex++){
            var meshes = VectorUtils.TessellateScene(
                                                     scene,
                                                     Tiers[tierIndex].
                                                         ToOptions(),
                                                     nodeOpacity
                                                    );
            var vertexCount = CountVertices(meshes);

            if(vertexCount <= VertexLimit)
                return VectorUtils.BuildSprite(
                                               meshes,
                                               viewBox,
                                               PixelsPerUnit,
                                               VectorUtils.Alignment.Center,
                                               Vector2.zero,
                                               GradientResolution,
                                               FlipVerticalAxis
                                              );

            CareerLogPlugin.Log?.LogDebug(
                                          PluginInfo.Format(
                                                            $"Map SVG tier {tierIndex + 1}/{Tiers.Length} over vertex limit "
                                                          + $"({vertexCount}) for {sourcePathForLogs}; coarsening."
                                                           )
                                         );
        }

        CareerLogPlugin.Log?.LogWarning(
                                        PluginInfo.Format(
                                                          $"Map SVG could not be rasterized within vertex limit: {sourcePathForLogs}"
                                                         )
                                       );

        return null;
    }

    private static int CountVertices(IReadOnlyList<VectorUtils.Geometry> meshes){
        var total = 0;

        if(meshes == null) return total;

        foreach(var mesh in meshes){
            total += mesh.Vertices?.Length ?? 0;

            if(total > VertexLimit) return total;
        }

        return total;
    }
}
