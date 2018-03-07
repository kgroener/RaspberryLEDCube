using System;

namespace LEDCube.CanonicalSchema.Contract
{
    public interface ILEDCubeAnimation
    {
        /// <summary>
        /// Signals the animation that it will be receiving calls to the <see cref="Update"/> method.
        /// This allows the animation to setup the animation.
        /// </summary>
        /// <param name="cube"></param>
        void Prepare();

        /// <summary>
        /// Requests the animation to gracefully stop within the allowed time. 
        /// If the animation is not stopped within this time, the animation is forced to stop by calling the <see cref="Cleanup"/> method.
        /// </summary>
        /// <param name="timeout"></param>
        void RequestStop(TimeSpan timeout);

        /// <summary>
        /// Signals that the animation' Update method will no longer be called.
        /// Allows for the animation to reset and dispose of objects in memory.
        /// </summary>
        /// <param name="cube"></param>
        void Cleanup();

        /// <summary>
        /// Once the animation is started, will be called periodically.
        /// During this method the animation will be able to update the led cube colors.
        /// The update method should call the Draw method on the cube controller.
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="updateInterval"></param>
        void Update(ILEDCubeController cube, TimeSpan updateInterval);

        /// <summary>
        /// True when the animation is finished/stopped
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// True when RequestStop is called or the animation enters it's stopping sequence
        /// </summary>
        bool IsStopping { get; }

        /// <summary>
        /// True when the animation has a finite duration, false when the animation can repeat indefinately
        /// </summary>
        bool IsFinite { get; }

        /// <summary>
        /// The preffered duration of this animation
        /// </summary>
        TimeSpan PrefferedDuration { get; }

        /// <summary>
        /// If true, the animation will be automatically scheduled at random by the animation controller
        /// </summary>
        bool AutomaticSchedulingAllowed { get; }
    }
}
