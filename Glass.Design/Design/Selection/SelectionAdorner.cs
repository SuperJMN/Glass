using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Glass.Design.Selection
{
    public class SelectionAdorner : Adorner
    {
        private readonly VisualCollection _visualCollection;
        private readonly UIElement _adornedelement;
        private readonly Canvas _canvas;
        private FrameworkElement _mask;

        public SelectionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _adornedelement = adornedElement;
            _canvas = new Canvas();
            //_canvas.Background = Brushes.Red;

            _visualCollection = new VisualCollection(_adornedelement);
            _visualCollection.Add(_canvas);
        }

        #region Template

        /// <summary>
        /// Template Dependency Property
        /// </summary>
        public static readonly DependencyProperty TemplateProperty = DependencyProperty.Register("Template", typeof(ControlTemplate), typeof(SelectionAdorner), new FrameworkPropertyMetadata(null, OnTemplateChanged));

        /// <summary>
        /// Gets or sets the Template property. This dependency property 
        /// indicates ....
        /// </summary>
        public ControlTemplate Template
        {
            get { return (ControlTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Template property.
        /// </summary>
        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectionAdorner target = (SelectionAdorner)d;
            ControlTemplate oldTemplate = (ControlTemplate)e.OldValue;
            ControlTemplate newTemplate = target.Template;
            target.OnTemplateChanged(oldTemplate, newTemplate);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Template property.
        /// </summary>
        protected virtual void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            _mask = (FrameworkElement)newTemplate.LoadContent();
            _canvas.Children.Clear();
            _canvas.Children.Add(_mask);
            _mask.Height = 100;
            SetBindings();
        }

        private void SetBindings()
        {
            if (_mask!=null)
            {
                Binding binding=new Binding();
                binding.Path = new PropertyPath("Left");
                binding.Source = this;
                _mask.SetBinding(Canvas.LeftProperty, binding);

                binding = new Binding();
                binding.Path = new PropertyPath("Top");
                binding.Source = this;
                _mask.SetBinding(Canvas.TopProperty, binding);

                binding = new Binding();
                binding.Path = new PropertyPath("Width");
                binding.Source = this;
                _mask.SetBinding(WidthProperty, binding);

                binding = new Binding();
                binding.Path = new PropertyPath("Height");
                binding.Source = this;
                _mask.SetBinding(HeightProperty, binding);
            }
        }

        #endregion

        protected override Size MeasureOverride(Size constraint)
        {
            _canvas.Measure(_adornedelement.RenderSize);
            return _adornedelement.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {

            var rect = new Rect(new Point(0, 0), finalSize);
            _canvas.Arrange(rect);

            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visualCollection[0];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _visualCollection.Count;
            }
        }

        #region Left

        /// <summary>
        /// Left Dependency Property
        /// </summary>
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(double), typeof(SelectionAdorner),
                new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the Left property. This dependency property 
        /// indicates ....
        /// </summary>
        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        #endregion

        #region Top

        /// <summary>
        /// Top Dependency Property
        /// </summary>
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(SelectionAdorner),
                new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the Top property. This dependency property 
        /// indicates ....
        /// </summary>
        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        #endregion

    }
}