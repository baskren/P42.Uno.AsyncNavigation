using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace P42.Uno.AsyncNavigation
{
    class BaseActionAnimator

    {
        public TimeSpan TimeSpan { get; private set; }

        public EasingFunctionBase EasingFunction { get; private set; }

        public Action<double> Action { get; private set; }

        protected DateTime StartTime;

        public BaseActionAnimator(TimeSpan timeSpan, Action<double> action, EasingFunctionBase easingFunction = null)
        {
            TimeSpan = timeSpan;
            EasingFunction = easingFunction;
            Action = action;
        }

        public async Task RunAsync()
        {
            StartTime = DateTime.Now;
            var normalTime = 0.0;
            do
            {
                await Task.Delay(10);
                normalTime = Math.Min((DateTime.Now - StartTime).TotalMilliseconds / TimeSpan.TotalMilliseconds, 1.0);
#if NETFX_CORE
                var value = EasingFunction?.Ease(normalTime) ?? normalTime;
#else
                var value = EasingFunction?.Ease(normalTime, 0, 1, 1) ?? normalTime;
#endif
                Action?.Invoke(value);
            }
            while (normalTime < 1.0);
        }

        protected virtual double Value(double normalValue)
        {
            return normalValue;
        }
    }
}

