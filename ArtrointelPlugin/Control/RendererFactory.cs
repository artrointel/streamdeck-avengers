using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDGraphics;

namespace ArtrointelPlugin.Control.Model
{
    public class RendererFactory
    {
        public static CanvasRendererBase CreateRenderer(EffectConfig cfg)
        {
            CanvasRendererBase renderer = null;
            // creates a renderer
            if(cfg.mType.Equals(EffectConfig.EType.Flash.ToString()))
            {
                renderer = new FlashRenderer(cfg.mColor, cfg.mDuration);
            } else if (cfg.mType.Equals(EffectConfig.EType.CircleSpread.ToString()))
            {
                renderer = new CircleSpreadRenderer(cfg.mColor, cfg.mDuration);
            } else if (cfg.mType.Equals(EffectConfig.EType.Pie.ToString()))
            {
                // TODO Add more cfg data
                renderer = new PieRenderer(cfg.mColor, cfg.mDuration);
            } else if (cfg.mType.Equals(EffectConfig.EType.ImageBlending.ToString()))
            {
                // TODO
            }
            return renderer;
        }
    }
}
