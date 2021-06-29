namespace ArtrointelPlugin.SDGraphics.Renderer
{
    interface IAnimatableRenderer
    {
        /// <summary>
        /// Start the animator. animation will be re-played if restart is true, 
        /// else it will keep the state of the animation.
        /// </summary>
        /// /// <param name="delayInSecond"></param>
        /// <param name="restart"></param>
        void animate(double delayInSecond = 0, bool restart = true);

        /// <summary>
        /// pause the animation.
        /// </summary>
        void pause();
    }
}
