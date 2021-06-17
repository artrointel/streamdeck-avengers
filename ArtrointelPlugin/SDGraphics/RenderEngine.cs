using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Timers;

namespace SDGraphics
{
    /// <summary>
    /// RenderEngine is a simple graphic module to make dynamically-animated rendered action icon for the stream deck.
    /// Multiple renderers can be attached to the render engine for the rendering.
    /// 
    /// RenderEngine checks every attached renderer whether it should be rendered again or not first, 
    /// and renders on each offscreen canvas.
    /// SDGRenderEngine composites each canvas in the renderer, ordered by array list.
    /// Result of the composition will be drawn at mCompositedCanvas.
    /// </summary>
    public class RenderEngine
    {
        // Aimes to be 60fps as default, but it may not reach to the 60 fps
        // due to the system performance or due to the timer library.
        public const int FRAME_RATE_HINT = 60;

        private double mFrameDuration;
        private Timer mRenderTimer;

        ArrayList mRenderers = new ArrayList();
        SDCanvas mCompositedCanvas;
        bool mNeedComposition;

        Action<SDCanvas> mOnUpdatedCanvas;

        /// <summary>
        /// It renders and composites per input frameRate.
        /// </summary>
        /// <param name="frameRate"></param>
        public RenderEngine(int frameRate = FRAME_RATE_HINT)
        {
            mFrameDuration = 1000.0 / frameRate;
            mCompositedCanvas = SDCanvas.CreateCanvas();
            mCompositedCanvas.mGraphics.CompositingMode = CompositingMode.SourceOver;

            mRenderTimer = new Timer(mFrameDuration);
            mRenderTimer.Elapsed += onTimedEvent;
            mRenderTimer.AutoReset = true;
        }

        /// <summary>
        /// starts the rendering engine.
        /// </summary>
        public void run()
        {
            mRenderTimer.Start();
        }

        public bool animateRendererAt(int index, double delayInSecond, bool restart = true)
        {
            if (mRenderers[index] is IAnimatableRenderer)
            {
                ((IAnimatableRenderer)mRenderers[index]).animate(delayInSecond, restart);
                return true;
            }
            return false;
        }

        /// <summary>
        /// pause the internal rendering loop. it can be restarted by calling run().
        /// </summary>
        public void pause()
        {
            mRenderTimer.Stop();
        }

        /// <summary>
        /// Destroys all resources including attached renderers.
        /// </summary>
        public void destroyAll()
        {
            mOnUpdatedCanvas = null;
            mRenderTimer.Stop();
            mRenderTimer.Dispose();
            foreach (CanvasRendererBase renderer in mRenderers)
            {
                renderer.onDestroy();
            }
            mRenderers.Clear();
            mCompositedCanvas.mImage.Dispose();
        }

        /// <summary>
        /// listener is called whenever the rendering result is updated.
        /// </summary>
        /// <param name="listener"></param>
        public void setRenderingUpdatedListener(Action<SDCanvas> listener)
        {
            mOnUpdatedCanvas = listener;
        }

        /// <summary>
        /// Renderer will be rendered and also composited by the engine.
        /// </summary>
        /// <param name="renderer"></param>
        public void addRenderer(CanvasRendererBase renderer)
        {
            mRenderers.Add(renderer);
        }

        /// <summary>
        /// Removed renderer will not be rendered and composed to the result.
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns></returns>
        public bool removeRenderer(CanvasRendererBase renderer)
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

        public ArrayList getRenderers()
        {
            return mRenderers;
        }

        private void onTimedEvent(object sender, ElapsedEventArgs e)
        {
            doRender();
            if (doComposite() && (mOnUpdatedCanvas != null))
            {
                mOnUpdatedCanvas(mCompositedCanvas);
            }
        }

        private void doRender()
        {
            mNeedComposition = false;
            foreach(CanvasRendererBase renderer in mRenderers)
            {
                if(renderer.needToUpdate())
                {
                    renderer.onRender(renderer.mOffscreenCanvas.mGraphics);
                    mNeedComposition = true;
                }
            }
        }

        private bool doComposite()
        {
            if(mNeedComposition)
            {
                mCompositedCanvas.mGraphics.Clear(Color.Black);
                foreach (CanvasRendererBase renderer in mRenderers)
                {
                    mCompositedCanvas.mGraphics.DrawImage(renderer.mOffscreenCanvas.mImage, new Point(0, 0));
                }
                return true;
            }
            return false;
        }
    }
}
