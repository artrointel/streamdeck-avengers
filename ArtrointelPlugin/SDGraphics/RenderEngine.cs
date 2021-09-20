using System;
using System.Drawing;
using System.Collections.Generic;
using System.Timers;
using ArtrointelPlugin.SDGraphics.Renderer;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDGraphics
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
    public class RenderEngine : IControllable
    {
        // Aims to be 60fps as default, but it may not reach to the 60 fps
        // due to the system performance and due to the timer library.
        public const int FRAME_RATE_HINT = 60;

        private double mFrameDuration;
        private TimerControl mRenderTimer;

        private List<CanvasRendererBase> mRenderers = new List<CanvasRendererBase>();
        private SDCanvas mCompositedCanvas;
        private bool mNeedComposition;

        private Action<Image> mOnUpdatedCanvas;

        /// <summary>
        /// It renders and composites per input frameRate.
        /// </summary>
        /// <param name="frameRate"></param>
        public RenderEngine(int frameRate = FRAME_RATE_HINT)
        {
            mFrameDuration = 1000.0 / frameRate;
            mCompositedCanvas = SDCanvas.CreateCanvas();
            mRenderTimer = new TimerControl(mFrameDuration, onTimedEvent);
        }

        #region Interfaces
        public bool animateRendererAt(int index)
        {
            if(mRenderers == null || mRenderers[index] == null)
            {
                return false;
            }
            if (mRenderers[index] is CanvasRendererAnimatable)
            {
                CanvasRendererAnimatable renderer = (CanvasRendererAnimatable)mRenderers[index];
                renderer.start();
                return true;
            }

            return false;
        }

        /// <summary>
        /// listener is called whenever the rendering result is updated.
        /// </summary>
        /// <param name="listener"></param>
        public void setRenderingUpdatedListener(Action<Image> listener)
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

        public void addRendererAt(int index, CanvasRendererBase renderer)
        {
            mRenderers.Insert(index, renderer);
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

        public List<CanvasRendererBase> getRenderers()
        {
            return mRenderers;
        }

        public double getTotalDurationAt(int index)
        {
            return mRenderers[index].getTotalDuration();
        }

        /// <summary>
        /// stop the rendering preserving internal threaded tasks
        /// </summary>
        public void preserve()
        {
            mRenderTimer.stop();
        }

        /// <summary>
        /// Destroys all resources including attached renderers.
        /// </summary>
        public void destroyAll()
        {
            mRenderTimer.stop();
            foreach (CanvasRendererBase renderer in mRenderers)
            {
                renderer.onDestroy();
            }
            mRenderers.Clear();
            lock (mCompositedCanvas)
            {
                mCompositedCanvas.Dispose();
            }
        }
        #endregion

        #region Internal logics
        private void onTimedEvent(object sender, ElapsedEventArgs e)
        {
            doRender();
            if (doComposite())
            {
                updateCanvasLocked();
            }
        }

        private void doRender()
        {
            mNeedComposition = false;
            foreach(CanvasRendererBase renderer in mRenderers)
            {
                if(renderer.needToUpdate())
                {
                    renderer.onRender(renderer.mOffscreenCanvas.getGraphics());
                    mNeedComposition = true;
                }
            }
        }

        private bool doComposite()
        {
            if(mNeedComposition)
            {
                lock (mCompositedCanvas)
                {
                    mCompositedCanvas.getGraphics().Clear(Color.Empty);
                    foreach (CanvasRendererBase renderer in mRenderers)
                    {
                        if (renderer.isVisible())
                            mCompositedCanvas.getGraphics().DrawImage(renderer.mOffscreenCanvas.getImage(), new Point(0, 0));
                    }
                }
                return true;
            }
            return false;
        }

        private void updateCanvasLocked()
        {
            lock (mCompositedCanvas)
            {
                mOnUpdatedCanvas?.Invoke(mCompositedCanvas.getImage());
            }
        }
        #endregion

        #region Implements IControllable
        public void start()
        {
            mRenderTimer.start();
        }

        public void pause()
        {
            foreach (CanvasRendererBase renderer in mRenderers)
            {
                if(renderer is CanvasRendererAnimatable)
                {
                    ((CanvasRendererAnimatable)renderer).pause();
                }
            }
            mRenderTimer.pause();
        }

        public void resume()
        {
            foreach (CanvasRendererBase renderer in mRenderers)
            {
                if (renderer is CanvasRendererAnimatable)
                {
                    ((CanvasRendererAnimatable)renderer).resume();
                }
            }
            mRenderTimer.resume();
        }

        public void stop()
        {
            foreach (CanvasRendererBase renderer in mRenderers)
            {
                if (renderer is CanvasRendererAnimatable)
                {
                    ((CanvasRendererAnimatable)renderer).stop();
                }
            }
            mRenderTimer.stop();
        }
        #endregion
    }
}
