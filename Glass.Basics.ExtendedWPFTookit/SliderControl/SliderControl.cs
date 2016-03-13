namespace Glass.Basics.Xceed.Wpf.Toolkit.SliderControl
{
    using System.Windows;
    using System.Windows.Controls;

    public class SliderControl : Control
    {
        static SliderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliderControl), new FrameworkPropertyMetadata(typeof(SliderControl)));
        }

        #region MinValue

        public static readonly DependencyProperty MinValueProperty =
          DependencyProperty.Register("MinValue", typeof(double), typeof(SliderControl),
            new FrameworkPropertyMetadata(0.0));

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        #endregion

        #region MaxValue

        public static readonly DependencyProperty MaxValueProperty =
          DependencyProperty.Register("MaxValue", typeof(double), typeof(SliderControl),
            new FrameworkPropertyMetadata(0.0));

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        #endregion

        #region Value

        public static readonly DependencyProperty ValueProperty =
          DependencyProperty.Register("Value", typeof(double), typeof(SliderControl),
            new FrameworkPropertyMetadata(0.0));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        #region SmallChange
        public static readonly DependencyProperty SmallChangeProperty =
          DependencyProperty.Register("SmallChange", typeof(double), typeof(SliderControl),
            new FrameworkPropertyMetadata(0.0));

        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        #endregion
    }
}
