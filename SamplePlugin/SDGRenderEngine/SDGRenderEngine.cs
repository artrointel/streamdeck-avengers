using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Timers;

namespace SDG
{
    /*
     * SDGRenderEngine is a simple graphic module to make an animated action icon.
     * It has multiple image layers to be composed per frame rendering.
     */
    public class SDGRenderEngine
    {
        public const int UPDATE_FRAME_RATE = 60;

        private double mFrameDuration;
        private Timer mRenderTimer;

        /**
         * render engine has multiple rendering images.
         */
        ArrayList mRenderers = new ArrayList();
        SDCanvas mCompositedCanvas;
        bool mNeedComposition;

        Action<SDCanvas> mOnUpdatedCanvas;

        public SDGRenderEngine(int frameRate = UPDATE_FRAME_RATE)
        {
            mFrameDuration = 1000.0 / frameRate;
            mCompositedCanvas = SDCanvas.CreateCanvas();
            mCompositedCanvas.mGraphics.CompositingMode = CompositingMode.SourceOver;

            mRenderTimer = new Timer(mFrameDuration);
            mRenderTimer.Elapsed += onTimedEvent;
            mRenderTimer.AutoReset = true;
        }

        public void run()
        {
            mRenderTimer.Start();
        }

        public void pause()
        {
            mRenderTimer.Stop();
        }

        protected void onTimedEvent(object sender, ElapsedEventArgs e)
        {
            doRender();
            if(doComposite())
            {
                mOnUpdatedCanvas(mCompositedCanvas);
            }
        }

        public void setRenderingUpdatedListener(Action<SDCanvas> listener)
        {
            mOnUpdatedCanvas = listener;
        }

        public void addRenderer(SDCanvasRendererBase renderer)
        {
            mRenderers.Add(renderer);
        }

        public bool removeLayerRenderer(SDCanvasRendererBase renderer)
        {
            if(mRenderers.Contains(renderer))
            {
                mRenderers.Remove(renderer);
                return true;
            } 
            else
            {
                return false;
            }
        }

        public void doRender()
        {
            mNeedComposition = false;
            foreach(SDCanvasRendererBase renderer in mRenderers)
            {
                if(renderer.needToUpdate())
                {
                    renderer.onRender(renderer.mDefaultCanvas.mGraphics);
                    mNeedComposition = true;
                }
            }
        }

        public bool doComposite()
        {
            if(mNeedComposition)
            {
                mCompositedCanvas.mGraphics.Clear(Color.Black);
                foreach (SDCanvasRendererBase renderer in mRenderers)
                {
                    mCompositedCanvas.mGraphics.DrawImage(renderer.mDefaultCanvas.mImage, new Point(0, 0));
                }
                return true;
            }
            return false;
        }
    }
}
