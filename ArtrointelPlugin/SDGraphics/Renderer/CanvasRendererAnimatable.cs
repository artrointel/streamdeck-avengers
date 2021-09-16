using ArtrointelPlugin.Utils;
using System.Collections.Generic;

namespace ArtrointelPlugin.SDGraphics.Renderer
{
    public abstract class CanvasRendererAnimatable : CanvasRendererBase, IControllable
    {
        public CanvasRendererAnimatable(
               int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
               int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
               : this(SDCanvas.CreateInfo.DEFAULT, canvasWidth, canvasHeight)
        { }

        public CanvasRendererAnimatable(SDCanvas.CreateInfo info,
            int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
            int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
            : base(info, canvasWidth, canvasHeight)
        { }
        /*
        public enum State
        {
            READY,
            RUNNING,
            PAUSED,
            DONE
        }

        public State mState
        {
            get
            {
                return mState;
            }
            
            private set
            {
                mState = value;
            }
        }*/

        private List<IControllable> mControllableItems = new List<IControllable>();
        private List<IControllable> mStartItems = new List<IControllable>();

        internal void setControllableItems(params IControllable[] controllables)
        {
            mControllableItems.AddRange(controllables);
        }

        internal void setStartItems(params IControllable[] controllables)
        {
            mStartItems.AddRange(controllables);
        }

        public void start()
        {
            foreach (IControllable item in mStartItems)
            {
                item.start();
            }
        }

        public void pause()
        {
            foreach (IControllable item in mControllableItems)
            {
                if(item != null)
                {
                    item.pause();
                }
            }
        }

        public void resume()
        {
            foreach (IControllable item in mControllableItems)
            {
                if(item != null)
                {
                    item.resume();
                }
            }
        }
        
        public void stop()
        {
            foreach (IControllable item in mControllableItems)
            {
                if(item != null)
                {
                    item.stop();
                }
            }
        }
    }
}
