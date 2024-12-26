using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

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

        // Dictionary to track active drag states by their keys
        private readonly Dictionary<int, DragState> _activeDrags = new();

        // Variable to track if a touch drag is already active
        private bool _isTouchDragActive = false;

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseDown += AssociatedObjectOnPreviewMouseDown;
            AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
            AssociatedObject.MouseUp += AssociatedObjectOnMouseUp;

            AssociatedObject.TouchDown += AssociatedObjectOnTouchDown;
            AssociatedObject.TouchMove += AssociatedObjectOnTouchMove;
            AssociatedObject.TouchUp += AssociatedObjectOnTouchUp;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseDown -= AssociatedObjectOnPreviewMouseDown;
            AssociatedObject.MouseMove -= AssociatedObjectOnMouseMove;
            AssociatedObject.MouseUp -= AssociatedObjectOnMouseUp;

            AssociatedObject.TouchDown -= AssociatedObjectOnTouchDown;
            AssociatedObject.TouchMove -= AssociatedObjectOnTouchMove;
            AssociatedObject.TouchUp -= AssociatedObjectOnTouchUp;
        }

        private void BeginDrag(Point position, FrameworkElement element, int key, TouchDevice? touchDevice = null)
        {
            // For touch, ensure only one touch drag is active
            if (key != -1 && _isTouchDragActive)
                return; // Another touch drag is already active

            if (_activeDrags.ContainsKey(key))
                return; // Drag already in progress for this key

            var dragState = new DragState
            {
                MouseDownPosition = position,
                DraggingElement = element,
                AdornerSourcePosition = element.TranslatePoint(new Point(), AssociatedObject),
                TouchDevice = touchDevice
            };

            if (element != null)
            {
                if (key == -1)
                {
                    // Mouse
                    Mouse.Capture(AssociatedObject);
                }
                else if (touchDevice != null)
                {
                    // Touch
                    AssociatedObject.CaptureTouch(touchDevice);
                    _isTouchDragActive = true; // Mark that a touch drag is active
                }

                ShowAdornerFromElement(element, dragState);
                MoveAdorner(position, dragState);
                element.Visibility = Visibility.Hidden;
            }

            _activeDrags[key] = dragState;
        }

        private void EndDrag(int key)
        {
            if (!_activeDrags.TryGetValue(key, out var dragState))
                return;

            if (key == -1)
            {
                // Mouse
                Mouse.Capture(null);
            }
            else if (dragState.TouchDevice != null)
            {
                // Touch
                AssociatedObject.ReleaseTouchCapture(dragState.TouchDevice);
                _isTouchDragActive = false; // Reset the touch drag active flag
            }

            HideAdorner(dragState);

            if (dragState.DraggingElement != null)
            {
                dragState.DraggingElement.Visibility = Visibility.Visible;
            }

            _activeDrags.Remove(key);
        }

        private void DragMove(Point position, int key)
        {
            if (!_activeDrags.TryGetValue(key, out var dragState))
                return;

            var currentElement = GetItemByPoint(position);
            MoveAdorner(position, dragState);

            if (currentElement == null || currentElement == dragState.DraggingElement)
                return;

            var draggingIndex = AssociatedObject.ItemContainerGenerator.IndexFromContainer(dragState.DraggingElement);
            var currentIndex = AssociatedObject.ItemContainerGenerator.IndexFromContainer(currentElement);

            if (AssociatedObject.ItemsSource is IList itemList &&
                CheckListIndex(itemList, draggingIndex) &&
                CheckListIndex(itemList, currentIndex))
            {
                var temp = itemList[draggingIndex];
                itemList.RemoveAt(draggingIndex);
                itemList.Insert(currentIndex, temp);

                dragState.DraggingElement = AssociatedObject.ItemContainerGenerator.ContainerFromIndex(currentIndex) as FrameworkElement;

                if (dragState.DraggingElement != null)
                    dragState.DraggingElement.Visibility = Visibility.Hidden;
            }
        }

        private void AssociatedObjectOnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsDragging) return;

            var position = e.GetPosition(AssociatedObject);
            var element = GetItemByPoint(position);
            if (element != null)
            {
                BeginDrag(position, element, -1); // -1 for mouse
            }
        }

        private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDragging) return;
            if (e.LeftButton != MouseButtonState.Pressed) return;

            var position = e.GetPosition(AssociatedObject);
            DragMove(position, -1); // -1 for mouse
        }

        private void AssociatedObjectOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsDragging) return;
            EndDrag(-1); // -1 for mouse
        }

        private void AssociatedObjectOnTouchDown(object sender, TouchEventArgs e)
        {
            if (!IsDragging) return;

            var touchDevice = e.TouchDevice;
            var key = touchDevice.Id;

            // Prevent multiple touch drags by checking if a touch drag is already active
            if (_isTouchDragActive)
                return;

            var position = e.GetTouchPoint(AssociatedObject).Position;
            var element = GetItemByPoint(position);
            if (element != null)
            {
                BeginDrag(position, element, key, touchDevice);
            }
        }

        private void AssociatedObjectOnTouchMove(object sender, TouchEventArgs e)
        {
            if (!IsDragging) return;

            var touchDevice = e.TouchDevice;
            var key = touchDevice.Id;
            var position = e.GetTouchPoint(AssociatedObject).Position;
            DragMove(position, key);
        }

        private void AssociatedObjectOnTouchUp(object sender, TouchEventArgs e)
        {
            if (!IsDragging) return;

            var touchDevice = e.TouchDevice;
            var key = touchDevice.Id;
            EndDrag(key);
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

        private void ShowAdornerFromElement(FrameworkElement element, DragState dragState)
        {
            dragState.AdornerControl = new ContentControl();
            if (element is ContentPresenter contentPresenter)
            {
                dragState.AdornerControl.Content = contentPresenter.ContentTemplate.LoadContent();
                dragState.AdornerControl.DataContext = contentPresenter.DataContext;
            }

            var adornerCanvas = new Canvas
            {
                IsHitTestVisible = false
            };
            adornerCanvas.Children.Add(dragState.AdornerControl);

            dragState.TemplateAdorner = new TemplateAdorner(AssociatedObject, adornerCanvas);

            dragState.AdornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            dragState.AdornerLayer?.Add(dragState.TemplateAdorner);
        }

        private void HideAdorner(DragState dragState)
        {
            if (dragState.AdornerLayer == null || dragState.TemplateAdorner == null)
                return;

            dragState.AdornerLayer.Remove(dragState.TemplateAdorner);
            dragState.TemplateAdorner = null;
            dragState.AdornerControl = null;
            dragState.AdornerLayer = null;
        }

        private void MoveAdorner(Point mousePosition, DragState dragState)
        {
            if (dragState.AdornerControl == null)
                return;

            var mouseDeltaMove = mousePosition - dragState.MouseDownPosition;
            var adornerPosition = dragState.AdornerSourcePosition + mouseDeltaMove;

            Canvas.SetLeft(dragState.AdornerControl, adornerPosition.X);
            Canvas.SetTop(dragState.AdornerControl, adornerPosition.Y);
        }

        // Helper class to store drag state
        private class DragState
        {
            public Point AdornerSourcePosition { get; set; }
            public Point MouseDownPosition { get; set; }
            public FrameworkElement? DraggingElement { get; set; }
            public ContentControl? AdornerControl { get; set; }
            public AdornerLayer? AdornerLayer { get; set; }
            public TemplateAdorner? TemplateAdorner { get; set; }
            public TouchDevice? TouchDevice { get; set; }
        }

        public class TemplateAdorner : Adorner
        {
            private readonly FrameworkElement _frameworkElementAdorner;

            public TemplateAdorner(UIElement adornedElement, FrameworkElement frameworkElementAdorner) : base(adornedElement)
            {
                _frameworkElementAdorner = frameworkElementAdorner;
                AddVisualChild(_frameworkElementAdorner);
                AddLogicalChild(_frameworkElementAdorner);
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
                _frameworkElementAdorner.Measure(constraint);
                return constraint;
            }
        }
    }
}
