using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace Glass.Design
{
    public class DesignableGroup : DependencyObject, IDesignable
    {

        private Point anchorPoint;
        private ObservableCollection<IDesignable> items;

        private double left;
        private double top;

        private double width;
        private double height;

        private Dictionary<IDesignable, ChildProportions> childProportionsDictionary;

        public DesignableGroup()
        {
            Items = new ObservableCollection<IDesignable>();
            MinWidth = 10;
            MinHeight = 10;            
        }

        public ObservableCollection<IDesignable> Items
        {
            get { return items; }
            set
            {

                if (value != null)
                    value.CollectionChanged += ItemsOnCollectionChanged;

                items = value;

                var location = GetCurrentLocation(items);
                left = location.X;
                top = location.Y;

                var size = GetCurrentSize(location);
                width = size.Width;
                height = size.Height;

                childProportionsDictionary = CreateChildProportionsDictionary(value, new Rect(location, size));
            }
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (IDesignable child in notifyCollectionChangedEventArgs.NewItems)
                {
                    RegisterNewChild(child);
                }
            }

            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (IDesignable child in notifyCollectionChangedEventArgs.OldItems)
                {
                    //UnregisterChild(child);
                }
            }

            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Reset)
            {
                childProportionsDictionary.Clear();
                left = 0;
                width = 0;
                top = 0;
                height = 0;
            }
        }

        private void RegisterNewChild(IDesignable child)
        {
            Rect newBounds;

            if (items.Count == 1)
            {
                newBounds = new Rect
                                 {
                                     Location = new Point(child.Left, child.Top),
                                     Size = new Size(child.Width, child.Height),
                                 };
            }
            else
            {
                var currentBounds = new Rect(left, top, width, height);
                newBounds = GetCombinedBoundsForChildAddition(child, currentBounds);
            }
     
            childProportionsDictionary = CreateChildProportionsDictionary(items, newBounds);


            Console.WriteLine(string.Format("Left={0}, Width={1}", newBounds.Left, newBounds.Width));
            Console.WriteLine(string.Format("Top={0}, Height={1}", newBounds.Top, newBounds.Height));
            foreach (var childProportionse in childProportionsDictionary)
            {
                Console.WriteLine(string.Format("Lefts, Proporción={0}", childProportionse.Value.RelativeLocation.X));
            }
            foreach (var childProportionse in childProportionsDictionary)
            {
                Console.WriteLine(string.Format("Widths, Proporción={0}", childProportionse.Value.RelativeSize.Width));
            }

            left = newBounds.Left;
            top = newBounds.Top;
            width = newBounds.Width;
            height = newBounds.Height;

            RaiseLocationChanged();

            const double epsilon = 0.1;
            
            RaiseSizeChanged();            
        }

        private Rect GetCombinedBoundsForChildAddition(IDesignable child, Rect currentBounds)
        {
            var bounds = new Rect();
            var leftDiff = Math.Max(currentBounds.Left - child.Left, 0);
            var rightDiff = Math.Max(child.Left + child.Width - (currentBounds.Left + width), 0);
            var totalDiff = leftDiff + rightDiff;

            bounds.X = currentBounds.Left - leftDiff;
            bounds.Width = width + totalDiff;

            var topDiff = Math.Max(currentBounds.Top - child.Top, 0);
            var bottomDiff = Math.Max(child.Top + child.Height - (currentBounds.Top + height), 0);
            var totalVertDiff = topDiff + bottomDiff;

            bounds.Y = currentBounds.Top - topDiff;
            bounds.Height = height + totalVertDiff;
            return bounds;
        }

        private Dictionary<IDesignable, ChildProportions> CreateChildProportionsDictionary(IEnumerable<IDesignable> children, Rect parentBounds)
        {
            var proportionsDictionary = new Dictionary<IDesignable, ChildProportions>();

            foreach (var child in children)
            {
                proportionsDictionary.Add(
                    child,
                    new ChildProportions { RelativeLocation = GetRelativeLocation(child, parentBounds), RelativeSize = GetRelativeSize(child, parentBounds.Size) });
            }

            return proportionsDictionary;
        }

        private static Size GetRelativeSize(ISizable child, Size parentSize)
        {
            var widthProportion = child.Width / parentSize.Width;
            var heightProportion = child.Height / parentSize.Height;

            return new Size(widthProportion, heightProportion);
        }

        private static Point GetRelativeLocation(IMovable child, Rect parentBounds)
        {

            var localLeft = child.Left - parentBounds.Left;
            var localTop = child.Top - parentBounds.Top;

            var leftProportion = localLeft / parentBounds.Width;
            var topProportion = localTop / parentBounds.Height;

            return new Point(leftProportion, topProportion);
        }

        private static Point GetCurrentLocation(ObservableCollection<IDesignable> observableCollection)
        {
            if (observableCollection.Count == 0)
                return new Point(0, 0);

            var minLeft = observableCollection.Min(designable => designable.Left);
            var minTop = observableCollection.Min(designable => designable.Top);
            return new Point(minLeft, minTop);
        }

        public Point AnchorPoint
        {
            get { return anchorPoint; }
            set { anchorPoint = value; }
        }

        public double Width
        {
            get { return width; }
            set
            {
                if (value < MinWidth)
                    value = MinWidth;

                var newLeft = RecalculateLeft(value);
                left = newLeft;
                ArrangeChildrenWidth(value);
                width = value;
                RaiseSizeChanged();
            }
        }

        public double Height
        {
            get { return height; }
            set
            {
                if (value < MinWidth)
                    value = MinHeight;

                var newTop = RecalculateTop(value);
                top = newTop;
                ArrangeChildrenHeight(value);
                height = value;
                RaiseSizeChanged();
            }
        }

        public double MinWidth { get; set; }

        public double MinHeight { get; set; }

        private void RaiseSizeChanged()
        {
            if (SizeChanged != null)
                SizeChanged(this, EventArgs.Empty);
        }

        private void RaiseLocationChanged()
        {
            if (LocationChanged != null)
                LocationChanged(this, EventArgs.Empty);
        }

        public double Left
        {
            get { return left; }
            set
            {
                var diff = value - left;
                foreach (var designable in Items)
                {
                    designable.Left += diff;
                }
                left = value;
                RaiseLocationChanged();
            }
        }

        public double Top
        {
            get { return top; }
            set
            {
                var diff = value - top;
                foreach (var designable in Items)
                {
                    designable.Top += diff;
                }
                top = value;
                RaiseLocationChanged();
            }
        }

        private void ArrangeChildrenWidth(double parentWidth)
        {
            foreach (var designable in Items)
            {
                ResizeChildWidth(designable, parentWidth);
                RelocateChildWidth(designable, parentWidth);
            }
        }

        private void ArrangeChildrenHeight(double parentHeight)
        {
            foreach (var designable in Items)
            {
                ResizeChildHeight(designable, parentHeight);
                RelocateChildHeight(designable, parentHeight);
            }
        }

        private void RelocateChildWidth(IDesignable designable, double newWidth)
        {
            var proportion = childProportionsDictionary[designable].RelativeLocation.X;
            designable.Left = left + proportion * newWidth;
        }

        private void RelocateChildHeight(IDesignable designable, double newHeight)
        {
            var proportion = childProportionsDictionary[designable].RelativeLocation.Y;
            designable.Top = top + proportion * newHeight;
        }

        private void ResizeChildWidth(IDesignable designable, double newWidth)
        {
            var proportion = childProportionsDictionary[designable].RelativeSize.Width;
            designable.Width = newWidth * proportion;
        }

        private void ResizeChildHeight(IDesignable designable, double value)
        {
            var proportion = childProportionsDictionary[designable].RelativeSize.Height;
            designable.Height = value * proportion;
        }

        private double RecalculateLeft(double parentWidth)
        {
            var anchorLeftInParentCoords = anchorPoint.X * width + left;
            var newLeft = anchorLeftInParentCoords - (parentWidth * anchorPoint.X);

            return newLeft;
        }

        private double RecalculateTop(double parentHeight)
        {
            var anchorTopInParentCoords = anchorPoint.Y * height + top;
            var newTop = anchorTopInParentCoords - (parentHeight * anchorPoint.Y);

            return newTop;
        }

        private Size GetCurrentSize(Point resizingLocation)
        {
            if (Items.Count == 0)
                return new Size(0, 0);

            var maxRight = items.Max(designable => designable.Left + designable.Width);
            var maxBottom = items.Max(designable => designable.Top + designable.Height);

            return new Size(maxRight - resizingLocation.X, maxBottom - resizingLocation.Y);
        }

        public event EventHandler LocationChanged;
        public event EventHandler SizeChanged;
    }
}