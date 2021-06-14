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
        public static CanvasRendererBase generateRenderer(EffectConfig cfg)
        {
            CanvasRendererBase renderer = null;
            // creates a renderer
            if(cfg.mType.Equals(EffectConfig.EType.Flash.ToString()))
            {
                renderer = new SDGraphics.FlashRenderer();
            } else if (cfg.mType.Equals(EffectConfig.EType.CircleSpread.ToString()))
            {

            } else if (cfg.mType.Equals(EffectConfig.EType.Pie.ToString()))
            {

            }

            return renderer;
        }
    }
}
