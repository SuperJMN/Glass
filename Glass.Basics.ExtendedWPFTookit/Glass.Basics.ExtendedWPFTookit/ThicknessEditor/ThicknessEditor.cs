using System.Windows;
using System.Windows.Controls;

namespace Glass.Basics.ExtendedWPFTookit.ThicknessEditor
{
    public sealed class ThicknessEditor : Control
    {
        static ThicknessEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ThicknessEditor), new FrameworkPropertyMetadata(typeof(ThicknessEditor)));
        }

        #region Thickness

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(Thickness), typeof(ThicknessEditor),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnThicknessChanged));

        public Thickness Thickness
        {
            get { return (Thickness)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        private static void OnThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ThicknessEditor)d;
            var oldThicknessValue = (Thickness)e.OldValue;
            var newThicknessValue = target.Thickness;
            target.OnThicknessChanged(oldThicknessValue, newThicknessValue);
        }

        private void OnThicknessChanged(Thickness oldThicknessValue, Thickness newThicknessValue)
        {
            ThicknessLeft = newThicknessValue.Left;
            ThicknessRight = newThicknessValue.Right;
            ThicknessTop = newThicknessValue.Top;
            ThicknessBottom = newThicknessValue.Bottom;
        }

        #endregion

        #region ThicknessLeft

        public static readonly DependencyProperty ThicknessLeftProperty =
            DependencyProperty.Register("ThicknessLeft", typeof(double), typeof(ThicknessEditor),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnThicknessLeftChanged));

        public double ThicknessLeft
        {
            get { return (double)GetValue(ThicknessLeftProperty); }
            set { SetValue(ThicknessLeftProperty, value); }
        }

        private static void OnThicknessLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ThicknessEditor)d;
            var newThicknessLeft = target.ThicknessLeft;
            target.OnThicknessLeftChanged(newThicknessLeft);
        }

        private void OnThicknessLeftChanged(double newThicknessLeft)
        {
            Thickness = new Thickness(newThicknessLeft, Thickness.Top, Thickness.Right, Thickness.Bottom);
        }

        #endregion

        #region ThicknessRight

        public static readonly DependencyProperty ThicknessRightProperty =
            DependencyProperty.Register("ThicknessRight", typeof(double), typeof(ThicknessEditor),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnThicknessRightChanged));

        public double ThicknessRight
        {
            get { return (double)GetValue(ThicknessRightProperty); }
            set { SetValue(ThicknessRightProperty, value); }
        }

        private static void OnThicknessRightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ThicknessEditor)d;
            var newThicknessRight = target.ThicknessRight;
            target.OnThicknessRightChanged(newThicknessRight);
        }

        private void OnThicknessRightChanged(double newThicknessRight)
        {
            Thickness = new Thickness(Thickness.Left, Thickness.Top, newThicknessRight, Thickness.Bottom);
        }

        #endregion

        #region ThicknessTop

        public static readonly DependencyProperty ThicknessTopProperty =
            DependencyProperty.Register("ThicknessTop", typeof(double), typeof(ThicknessEditor),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnThicknessTopChanged));

        public double ThicknessTop
        {
            get { return (double)GetValue(ThicknessTopProperty); }
            set { SetValue(ThicknessTopProperty, value); }
        }

        private static void OnThicknessTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ThicknessEditor)d;
            var newThicknessTop = target.ThicknessTop;
            target.OnThicknessTopChanged(newThicknessTop);
        }

        private void OnThicknessTopChanged(double newThicknessTop)
        {
            Thickness = new Thickness(Thickness.Left, newThicknessTop, Thickness.Right, Thickness.Bottom);
        }

        #endregion

        #region ThicknessBottom

        public static readonly DependencyProperty ThicknessBottomProperty =
            DependencyProperty.Register("ThicknessBottom", typeof(double), typeof(ThicknessEditor),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnThicknessBottomChanged));

        public double ThicknessBottom
        {
            get { return (double)GetValue(ThicknessBottomProperty); }
            set { SetValue(ThicknessBottomProperty, value); }
        }

        private static void OnThicknessBottomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ThicknessEditor)d;
            var newThicknessBottom = target.ThicknessBottom;
            target.OnThicknessBottomChanged(newThicknessBottom);
        }

        private void OnThicknessBottomChanged(double newThicknessBottom)
        {
            Thickness = new Thickness(Thickness.Left, Thickness.Top, Thickness.Right, newThicknessBottom);
        }

        #endregion

        #region FormatString
        public static readonly DependencyProperty FormatStringProperty = 
          DependencyProperty.Register("FormatString", typeof(string), typeof(ThicknessEditor),
            new FrameworkPropertyMetadata(string.Empty));

        public string FormatString
        {
            get { return (string)GetValue(FormatStringProperty); }
            set { SetValue(FormatStringProperty, value); }
        }
        
        #endregion

        #region Suffix
        public static readonly DependencyProperty SuffixProperty = 
          DependencyProperty.Register("Suffix", typeof(string), typeof(ThicknessEditor),
            new FrameworkPropertyMetadata(string.Empty));

        public string Suffix
        {
            get { return (string)GetValue(SuffixProperty); }
            set { SetValue(SuffixProperty, value); }
        }
        
        #endregion

        #region Increment
        public static readonly DependencyProperty IncrementProperty = 
          DependencyProperty.Register("Increment", typeof(double), typeof(ThicknessEditor),
            new FrameworkPropertyMetadata(1.0));

        public double Increment
        {
            get { return (double)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }
        
        #endregion
    }
}
