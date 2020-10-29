using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace P42.Uno.AsyncNavigation
{
    class ActionAnimator : NormalizedActionAnimator
    {
        public double From { get; private set; }

        public double To { get; private set; }



        public ActionAnimator(double from, double to, TimeSpan timeSpan, EasingFunctionBase easingFunction, Action<double> action)
            : base(timeSpan, easingFunction, action)
        {
            From = from;
            To = to;
        }

        protected override double Value(double normalizedValue)
        {
            var value = From + normalizedValue * (To - From);
            return value;
        }
    }
}
