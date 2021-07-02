using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Drawing.Drawing2D;

namespace ArtrointelPlugin.SDGraphics
{
    /// <summary>
    /// Canvas is a thread-safe graphic buffer model to make possible to threaded rendering.
    /// 
    /// </summary>
    public class BufferedCanvas
    {
        public const int BUFFER_LIMIT = 5;
        public const int DEFAULT_IMAGE_SIZE = 144;

        public struct CreateInfo
        {
            public CompositingMode CompositingMode { get; set; }
            public SmoothingMode SmoothingMode { get; set; }
            public InterpolationMode InterpolationMode { get; set; }
            public PixelOffsetMode PixelOffsetMode { get; set; }
            public CreateInfo Clone()
            {
                CreateInfo info = new CreateInfo();
                info.CompositingMode = CompositingMode;
                info.SmoothingMode = SmoothingMode;
                info.InterpolationMode = InterpolationMode;
                info.PixelOffsetMode = PixelOffsetMode;
                return info;
            }
        }
        public static readonly CreateInfo DefaultCreateInfo = new CreateInfo()
        {
            CompositingMode = CompositingMode.SourceOver,
            SmoothingMode = SmoothingMode.AntiAlias,
            InterpolationMode = InterpolationMode.HighQualityBicubic,
            PixelOffsetMode = PixelOffsetMode.HighQuality
        };

        public class Canvas
        {
            internal Graphics mGraphics;
            internal Image mImage;

            private Canvas() { }

            internal static Canvas Create(int width, int height, CreateInfo info)
            {
                Canvas c = new Canvas();
                c.mImage = new Bitmap(width, height);
                c.mGraphics = Graphics.FromImage(c.mImage);
                c.mGraphics.CompositingMode = info.CompositingMode;
                c.mGraphics.SmoothingMode = info.SmoothingMode;
                c.mGraphics.InterpolationMode = info.InterpolationMode;
                c.mGraphics.PixelOffsetMode = info.PixelOffsetMode;
                
                return c;
            }

            internal void Dispose()
            {
                lock(mGraphics)
                {
                    mGraphics.Dispose();
                }
                lock(mImage)
                {
                    mImage.Dispose();
                }
            }

            public bool isLocked()
            {
                return Monitor.IsEntered(mGraphics);
            }
            
            public Graphics lockCanvas()
            {
                Monitor.Enter(mGraphics);
                return mGraphics;
            }

            public void unlockCanvas()
            {
                Monitor.Exit(mGraphics);
            }
        }

        private LinkedList<Canvas> mCanvasQueue = new LinkedList<Canvas>();
        private Stack<Canvas> mCanvasPool;
        private object mSyncObj = new object();

        public BufferedCanvas(CreateInfo info, int width = DEFAULT_IMAGE_SIZE, int height = DEFAULT_IMAGE_SIZE, int bufferCountLimit = BUFFER_LIMIT)
        {
            lock (mSyncObj)
            {
                if (mCanvasPool != null) return;

                mCanvasPool = new Stack<Canvas>(bufferCountLimit);
                for (int i = 0; i < bufferCountLimit; i++)
                {
                    mCanvasPool.Push(Canvas.Create(width, height, info));
                }
            }
        }

        /// <summary>
        /// Consume the canvas as a consumer. It returns a copy of image that should be disposed after consuming.
        /// It returns null if it's not possible to read the canvas.
        /// </summary>
        /// <returns>disposable image or null.</returns>
        internal Image consume()
        {
            lock (mSyncObj)
            {
                Canvas c = null;
                if (mCanvasQueue.First != null)
                {
                    c = mCanvasQueue.First.Value;
                    var nextCanvasToRead = mCanvasQueue.First.Next;
                    // If there is an incoming canvas that can be read,
                    // first canvas should be removed from the list for next consuming.
                    if (nextCanvasToRead != null && !nextCanvasToRead.Value.isLocked())
                    {
                        mCanvasPool.Push(c);
                        mCanvasQueue.RemoveFirst();
                    }

                    if (!c.isLocked())
                    {
                        return (Image)c.mImage.Clone();
                    }
                    return null;
                }
                else return null;
            }
        }

        internal Image peekImage()
        {
            lock (mSyncObj)
            {
                if(mCanvasQueue.First != null)
                {
                    Canvas c = mCanvasQueue.First.Value;
                    if(!c.isLocked())
                    {
                        return (Image) c.mImage.Clone();
                    }
                    return null;
                }
                return null;
            }
        }

        /// <summary>
        /// Requests a canvas for drawing as a canvas buffer producer.
        /// </summary>
        /// <returns>canvas from pool, or null if the pool is empty</returns>
        internal Canvas acquire()
        {
            lock (mSyncObj)
            {
                if (mCanvasPool.Count != 0)
                {
                    Canvas c = mCanvasPool.Pop();
                    mCanvasQueue.AddLast(c);
                    return c;
                }
                else
                    return null;
            }
        }

        internal void dispose()
        {
            // TODO check the canvas can be disposed. some thread might be writing the canvas.
            lock (mSyncObj)
            {

            }
        }
    }
}
