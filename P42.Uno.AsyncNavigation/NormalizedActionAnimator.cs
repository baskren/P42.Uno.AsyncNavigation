using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace P42.Uno.AsyncNavigation
{
    class NormalizedActionAnimator

    {
        public TimeSpan TimeSpan { get; private set; }

        public EasingFunctionBase EasingFunction { get; private set; }

        public Action<double> Action { get; private set; }

        protected DateTime StartTime;

        public NormalizedActionAnimator(TimeSpan timeSpan, EasingFunctionBase easingFunction, Action<double> action)
        {
            TimeSpan = timeSpan;
            EasingFunction = easingFunction;
            Action = action;
        }

        public async Task RunAsync()
        {
            StartTime = DateTime.Now;
            double normalTime = 0.0;
            do
            {
                await Task.Delay(10);
                normalTime = Math.Min((DateTime.Now - StartTime).TotalMilliseconds / TimeSpan.TotalMilliseconds,1.0);
                var normalValue = EasingFunction.Ease(normalTime);
                var value = Value(normalValue);
                Action(value);
            }
            while (normalTime < 1.0);
        }

        protected virtual double Value(double normalValue)
        {
            return normalValue;
        }
    }
}
