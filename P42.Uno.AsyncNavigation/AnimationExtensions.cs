using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;

namespace P42.Uno.AsyncNavigation
{
    static class AnimationExtensions
    {

        public static AnimationDirection FlipDirection(this AnimationDirection direction)
        {
            switch (direction)
            {
                case AnimationDirection.LeftToRight:
                    return AnimationDirection.RightToLeft;
                case AnimationDirection.RightToLeft:
                    return AnimationDirection.LeftToRight;
                case AnimationDirection.TopToBottom:
                    return AnimationDirection.BottomToTop;
                case AnimationDirection.BottomToTop:
                    return AnimationDirection.TopToBottom;
                default:
                    return direction;
            }
        }

        public static PageAnimationOptions FlipDirection(this PageAnimationOptions options)
        {
            options.AnimationDirection = options.AnimationDirection.FlipDirection();
            return options;
        }

        public static Action<double> ToEntranceAction(this PageAnimationOptions options, PagePresenter pagePresenter, Size size)
        {
            switch (options.AnimationDirection)
            {
                case AnimationDirection.LeftToRight:
                    return d =>
                    {
                        pagePresenter.Arrange(new Rect(new Point((d - 1) * size.Width, 0), size));
                        if (options.ShouldFade)
                            pagePresenter.Opacity = d;
                    };
                case AnimationDirection.TopToBottom:
                    return d =>
                    {
                        pagePresenter.Arrange(new Rect(new Point(0, (d - 1) * size.Height), size));
                        if (options.ShouldFade)
                            pagePresenter.Opacity = d;
                    };
                case AnimationDirection.RightToLeft:
                    return d =>
                    {
                        pagePresenter.Arrange(new Rect(new Point((1 - d) * size.Width, 0), size));
                        if (options.ShouldFade)
                            pagePresenter.Opacity = d;
                    };
                case AnimationDirection.BottomToTop:
                    return d =>
                    {
                        pagePresenter.Arrange(new Rect(new Point(0, (1 - d) * size.Height), size));
                        if (options.ShouldFade)
                            pagePresenter.Opacity = d;
                    };
                default:
                    return d =>
                    {
                        pagePresenter.Arrange(new Rect(new Point(0, 0), size));
                        if (options.ShouldFade)
                            pagePresenter.Opacity = d;
                    };
            }
        }

        public static Action<double> ToExitAction(this PageAnimationOptions options, PagePresenter pagePresenter, Size size)
        {
            switch (options.AnimationDirection)
            {
                case AnimationDirection.LeftToRight:
                    return d =>
                    {
                        pagePresenter.Arrange(new Rect(new Point(d * size.Width, 0), size));
                        if (options.ShouldFade)
                            pagePresenter.Opacity = (1 - d);
                    };
                case AnimationDirection.TopToBottom:
                    return d =>
                    {
                        pagePresenter.Arrange(new Rect(new Point(0, d * size.Height), size));
                        if (options.ShouldFade)
                            pagePresenter.Opacity = (1 - d);
                    };
                case AnimationDirection.RightToLeft:
                    return d =>
                    {
                        pagePresenter.Arrange(new Rect(new Point(-d * size.Width, 0), size));
                        if (options.ShouldFade)
                            pagePresenter.Opacity = (1 - d);
                    };
                case AnimationDirection.BottomToTop:
                    return d =>
                    {
                        pagePresenter.Arrange(new Rect(new Point(0, -d * size.Height), size));
                        if (options.ShouldFade)
                            pagePresenter.Opacity = (1 - d);
                    };
                default:
                    return d =>
                    {
                        if (options.ShouldFade)
                            pagePresenter.Opacity = (1 - d);
                    };
            }
        }

    }
}
