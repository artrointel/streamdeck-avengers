using System;
using ArtrointelPlugin.SDGraphics.Renderer;
using ArtrointelPlugin.SDGraphics.Renderer.AnimatedEffects;

namespace ArtrointelPlugin.Control.Model
{
    public class RendererFactory
    {
        public static bool IsSupported(EffectConfig cfg)
        {
            if(cfg.mType == null)
                return false;

            foreach (EffectConfig.EType t in Enum.GetValues(typeof(EffectConfig.EType)))
            {
                if (cfg.mType.Equals(t.ToString())) return true;
            }
            return false;
        }

        public static CanvasRendererBase CreateRenderer(EffectConfig cfg)
        {
            if (cfg.mType == null)
                return null;

            CanvasRendererBase renderer = null;

            // Creates a renderer
            if(cfg.mType.Equals(EffectConfig.EType.Flash.ToString()))
            {
                renderer = new FlashRenderer(cfg.mDelay, cfg.mDuration, cfg.getColor());
            } 
            else if (cfg.mType.Equals(EffectConfig.EType.CircleSpread.ToString()))
            {
                renderer = new CircleSpreadRenderer(cfg.mDelay, cfg.mDuration, cfg.getColor());
            } 
            else if (cfg.mType.Equals(EffectConfig.EType.Pie.ToString()))
            {
                bool grow = false;
                bool clockwise = false;
                try
                {
                    string[] meta = cfg.mMetadata.Split(' '); // grow and clockwise options
                    grow = bool.Parse(meta[0]);
                    clockwise = bool.Parse(meta[1]);
                } catch { }
                
                renderer = new PieRenderer(cfg.mDelay, cfg.mDuration, cfg.getColor(), grow, clockwise);
            }
            else if (cfg.mType.Equals(EffectConfig.EType.BorderWave.ToString()))
            {
                renderer = new BorderWaveRenderer(cfg.mDelay, cfg.mDuration, cfg.getColor());
            }
            else if (cfg.mType.Equals(EffectConfig.EType.ColorOverlay.ToString()))
            {
                renderer = new ColorOverlayRenderer(cfg.mDelay, cfg.mDuration, cfg.getColor());
            }
            else if (cfg.mType.Equals(EffectConfig.EType.BlendGrayscaleFiltering.ToString()))
            {
                renderer = new AlphaBlendRenderer(cfg.mDelay, cfg.mDuration);
            }

            return renderer;
        }
    }
}
