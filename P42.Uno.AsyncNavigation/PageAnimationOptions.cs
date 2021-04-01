using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;

namespace P42.Uno.AsyncNavigation
{
    /// <summary>
    /// Options to customize the page animation
    /// </summary>
    public class PageAnimationOptions
    {
        /// <summary>
        /// Direction of page animation
        /// </summary>
        public AnimationDirection AnimationDirection { get; set; } = AnimationDirection.RightToLeft;

        /// <summary>
        /// Should opacity fade with animation?
        /// </summary>
        public bool ShouldFade { get; set; } = false;

        /// <summary>
        /// Animation duration
        /// </summary>
        public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(600);

        /// <summary>
        /// Animation easing function
        /// </summary>
        public EasingFunctionBase EasingFunction { get; set; } = new CubicEase { EasingMode = EasingMode.EaseOut };
    }

}
