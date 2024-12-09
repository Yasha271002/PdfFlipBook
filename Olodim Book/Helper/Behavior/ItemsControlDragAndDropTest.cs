using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using PdfFlipBook.Helper.Singleton;
using PdfFlipBook.Models;

namespace PdfFlipBook.Helper.Behavior
{
    internal class ItemsControlDragItemBehavior : Behavior<ItemsControl>
    {
        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register(
            nameof(IsDragging), typeof(bool), typeof(ItemsControlDragItemBehavior), new PropertyMetadata(default(bool)));

        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

        private FrameworkElement? _draggingElement;
        private ContentControl? _adornerControl;
        private AdornerLayer? _adornerLayer;
        private TemplateAdorner? _templateAdorner;

        private Point _adornerSourcePosition;
        private Point _mouseDownPosition;

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseDown += AssociatedObjectOnPreviewMouseDown;
            AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
            AssociatedObject.MouseUp += AssociatedObjectOnMouseUp;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseDown -= AssociatedObjectOnPreviewMouseDown;
            AssociatedObject.MouseMove -= AssociatedObjectOnMouseMove;
            AssociatedObject.MouseUp -= AssociatedObjectOnMouseUp;
        }

        private void AssociatedObjectOnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsDragging) return;

            _mouseDownPosition = e.GetPosition(AssociatedObject);
            _draggingElement = GetItemByPoint(_mouseDownPosition);

            if (_draggingElement is null)
                return;

            Mouse.Capture(AssociatedObject);
            ShowAdornerFromElement(_draggingElement);
            MoveAdorner(_mouseDownPosition);
            _draggingElement.Visibility = Visibility.Hidden;
        }

        private void AssociatedObjectOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsDragging) return;

            Mouse.Capture(null);
            HideAdorner();

            if (_draggingElement is null)
                return;

            _draggingElement.Visibility = Visibility.Visible;
            _draggingElement = null;
        }

        private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDragging) return;

            if (_draggingElement is null)
                return;

            var point = e.GetPosition(AssociatedObject);
            var currentElement = GetItemByPoint(point);

            MoveAdorner(point);

            if (currentElement is null || currentElement == _draggingElement)
                return;

            var draggingElementIndex = AssociatedObject.ItemContainerGenerator.IndexFromContainer(_draggingElement);
            var currentElementIndex = AssociatedObject.ItemContainerGenerator.IndexFromContainer(currentElement);

            if (AssociatedObject.ItemsSource is not IList itemList ||
                !CheckListIndex(itemList, draggingElementIndex) ||
                !CheckListIndex(itemList, currentElementIndex))
                return;

            var temp = itemList[draggingElementIndex];
            itemList.RemoveAt(draggingElementIndex);
            itemList.Insert(currentElementIndex, temp);

            _draggingElement = AssociatedObject.ItemContainerGenerator.ContainerFromIndex(currentElementIndex) as FrameworkElement;

            if (_draggingElement is null)
                return;

            _draggingElement.Visibility = Visibility.Hidden;
        }

        private bool CheckListIndex(IList list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        private FrameworkElement? GetItemByPoint(Point point)
        {
            for (var i = 0; i < AssociatedObject.Items.Count; i++)
            {
                if (AssociatedObject.ItemContainerGenerator.ContainerFromIndex(i) is not FrameworkElement element)
                    continue;

                var elementBoundsRect = element.TransformToAncestor(AssociatedObject)
                    .TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));

                if (elementBoundsRect.Contains(point.X, point.Y))
                    return element;
            }

            return null;
        }

        private void ShowAdornerFromElement(FrameworkElement element)
        {
            _adornerControl = new ContentControl();
            _adornerControl.Content = (element as ContentPresenter)?.ContentTemplate.LoadContent();
            _adornerControl.DataContext = element.DataContext;

            _adornerSourcePosition = element.TranslatePoint(new Point(), AssociatedObject);

            var adornerCanvas = new Canvas();
            adornerCanvas.IsHitTestVisible = false;
            adornerCanvas.Children.Add(_adornerControl);

            _templateAdorner = new TemplateAdorner(AssociatedObject, adornerCanvas);

            _adornerLayer ??= AdornerLayer.GetAdornerLayer(AssociatedObject);
            _adornerLayer?.Add(_templateAdorner);
        }

        private void HideAdorner()
        {
            if (_adornerLayer is null || _templateAdorner is null)
                return;

            _adornerLayer.Remove(_templateAdorner);
        }

        private void MoveAdorner(Point mousePosition)
        {
            if (_adornerControl is null)
                return;

            var mouseDeltaMove = mousePosition - _mouseDownPosition;
            var adornerPosition = _adornerSourcePosition + mouseDeltaMove;

            Canvas.SetLeft(_adornerControl, adornerPosition.X);
            Canvas.SetTop(_adornerControl, adornerPosition.Y);
        }
    }

    public class TemplateAdorner : Adorner
    {
        private readonly FrameworkElement _frameworkElementAdorner;

        public TemplateAdorner(UIElement adornedElement, FrameworkElement frameworkElementAdorner) : base(adornedElement)
        {
            _frameworkElementAdorner = frameworkElementAdorner;
            AddVisualChild(frameworkElementAdorner);
            AddLogicalChild(frameworkElementAdorner);
        }

        protected override int VisualChildrenCount => 1;

        protected override Size ArrangeOverride(Size finalSize)
        {
            _frameworkElementAdorner.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _frameworkElementAdorner;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _frameworkElementAdorner.Width = constraint.Width;
            _frameworkElementAdorner.Height = constraint.Height;

            return constraint;
        }
    }
}