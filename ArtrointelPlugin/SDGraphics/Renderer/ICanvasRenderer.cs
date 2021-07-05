using System.Drawing;

namespace ArtrointelPlugin.SDGraphics.Renderer
{
    interface ICanvasRenderer
    {
        /// <summary>
        /// Set 'need to update' flag to be true to render the renderer in the next render loop.
        /// </summary>
        void invalidate();

        bool needToUpdate();

        void onRender(Graphics graphics);

        void onDestroy();
    }
}
