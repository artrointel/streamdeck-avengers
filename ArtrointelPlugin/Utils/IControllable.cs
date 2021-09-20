using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtrointelPlugin.Utils
{
    public interface IControllable
    {
        /// <summary>
        /// Start business logic.
        /// - If the method called again while running, it should restart the logic.
        /// </summary>
        void start();

        /// <summary>
        /// Pause the business logic.
        /// - If the method called again, nothing should be done.
        /// - If the method called when it is not running, nothing should be done.
        /// </summary>
        void pause();

        /// <summary>
        /// Resume the business logic.
        /// - If the method called again, nothing should be done.
        /// </summary>
        void resume();

        /// <summary>
        /// Stop the business logic.
        /// - If the method called again, nothing should be done.
        /// </summary>
        void stop();
    }
}
