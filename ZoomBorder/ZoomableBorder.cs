using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Linq;

namespace ZoomBorder
{
    public enum ZoomBorderMouseAction
    {
        None,
        Move,
        Reset
    }

    public class ZoomBorder : Border
    {
        #region DependencyProperties

        //Left button action
        public static readonly DependencyProperty LeftButtonActionProperty =
            DependencyProperty.Register(
                nameof(LeftButtonAction),
                typeof(ZoomBorderMouseAction),
                typeof(ZoomBorder),
                new PropertyMetadata(
                    ZoomBorderMouseAction.Move,
                    null
                    )
            );
        public ZoomBorderMouseAction LeftButtonAction
        {
            get => (ZoomBorderMouseAction)GetValue(LeftButtonActionProperty);
            set => SetValue(LeftButtonActionProperty, value);
        }


        //Right button action
        public static readonly DependencyProperty RightButtonActionProperty =
            DependencyProperty.Register(
                nameof(RightButtonAction),
                typeof(ZoomBorderMouseAction),
                typeof(ZoomBorder),
                new PropertyMetadata(
                    ZoomBorderMouseAction.Reset,
                    null
                    )
            );
        public ZoomBorderMouseAction RightButtonAction
        {
            get => (ZoomBorderMouseAction)GetValue(RightButtonActionProperty);
            set => SetValue(RightButtonActionProperty, value);
        }


        //Middle button action
        public static readonly DependencyProperty MiddleButtonActionProperty =
            DependencyProperty.Register(
                nameof(MiddleButtonAction),
                typeof(ZoomBorderMouseAction),
                typeof(ZoomBorder),
                new PropertyMetadata(
                    ZoomBorderMouseAction.None,
                    null
                    )
            );
        public ZoomBorderMouseAction MiddleButtonAction
        {
            get => (ZoomBorderMouseAction)GetValue(MiddleButtonActionProperty);
            set => SetValue(MiddleButtonActionProperty, value);
        }


        //Max Scale
        public static readonly DependencyProperty ScaleMaxProperty =
            DependencyProperty.Register(
                nameof(ScaleMax),
                typeof(double),
                typeof(ZoomBorder),
                new PropertyMetadata(
                    10.0,
                    null
                    )
            );
        public double ScaleMax
        {
            get => (double)GetValue(ScaleMaxProperty);
            set => SetValue(ScaleMaxProperty, value);
        }


        //Min Scale
        public static readonly DependencyProperty ScaleMinProperty =
            DependencyProperty.Register(
                nameof(ScaleMin),
                typeof(double),
                typeof(ZoomBorder),
                new PropertyMetadata(
                    0.8,
                    null)
            );
        public double ScaleMin
        {
            get => (double)GetValue(ScaleMinProperty);
            set => SetValue(ScaleMinProperty, value);
        }


        //Scale Amount
        public static readonly DependencyProperty ScaleAmountProperty =
            DependencyProperty.Register(
                nameof(ScaleAmount),
                typeof(double),
                typeof(ZoomBorder),
                new PropertyMetadata(
                    0.005,
                    null)
            );
        public double ScaleAmount
        {
            get => (double)GetValue(ScaleAmountProperty);
            set => SetValue(ScaleAmountProperty, value);
        }


        //Zoom Enabled
        public static readonly DependencyProperty ZoomEnabledProperty =
            DependencyProperty.Register(
                nameof(ZoomEnabled),
                typeof(bool),
                typeof(ZoomBorder),
                new PropertyMetadata(
                    true,
                    null
                )
           );

        public bool ZoomEnabled
        {
            get => (bool)GetValue(ZoomEnabledProperty);
            set => SetValue(ZoomEnabledProperty, value);
        }

        //Pan Enabled
        public static readonly DependencyProperty PanEnabledProperty =
            DependencyProperty.Register(
                nameof(PanEnabled),
                typeof(bool),
                typeof(ZoomBorder),
                new PropertyMetadata(
                    true,
                    null
                )
            );
        public bool PanEnabled
        {
            get => (bool)GetValue(PanEnabledProperty);
            set => SetValue(PanEnabledProperty, value);
        }

        #endregion

        private UIElement? child = null;
        private Point origin;
        private Point start;

        /// <summary>
        /// X + Y Scale
        /// </summary>
        /// <remarks>X + Y should always be the same so combine them into 1 variable</remarks>
        public double Scale
        {
            get => _scaleTransform.ScaleX;
            set
            {
                _scaleTransform.ScaleX = value;
                _scaleTransform.ScaleY = value;
            }
        }

        public ZoomBorder()
        {
            ClipToBounds = true;
        }

        private TranslateTransform _translateTransform =>
            (TranslateTransform)((TransformGroup)Child.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);

        private ScaleTransform _scaleTransform =>
            (ScaleTransform)((TransformGroup)child.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);

        public override UIElement Child
        {
            get => base.Child;
            set
            {
                if (value != null && value != this.Child)
                    this.Initialize(value);
                base.Child = value;
            }
        }

        /// <summary>
        /// Initialise zoom + pan
        /// </summary>        
        public void Initialize(UIElement element)
        {
            this.child = element;
            if (child == null)
                return;

            var group = new TransformGroup();
            var st = new ScaleTransform();
            var tt = new TranslateTransform();
            group.Children.Add(st);
            group.Children.Add(tt);
            child.RenderTransform = group;
            child.RenderTransformOrigin = new Point(0.0, 0.0);

            //Events
            this.MouseWheel += ZoomBorder_MouseWheel;
            this.MouseDown += ZoomBorder_MouseDown;
            this.MouseUp += ZoomBorder_MouseUp;
            this.MouseMove += ZoomBorder_MouseMove;

        }

        /// <summary>
        /// Get current action of button specified
        /// </summary>
        ZoomBorderMouseAction GetAction(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return LeftButtonAction;
                case MouseButton.Right:
                    return RightButtonAction;
                case MouseButton.Middle:
                    return MiddleButtonAction;
                default:
                    return ZoomBorderMouseAction.None;
            }

        }




        /// <summary>
        /// Reset Zoom + Pan
        /// </summary>
        public void Reset()
        {
            if (child == null)
                return;

            // reset zoom            
            Scale = 1.0;

            // reset pan            
            _translateTransform.X = 0.0;
            _translateTransform.Y = 0.0;

        }

        #region Child Events

        private void MoveDown(MouseButtonEventArgs e)
        {
            if (!PanEnabled)
                return;

            if (child == null)
                return;

            var tt = _translateTransform;
            start = e.GetPosition(this);
            origin = new Point(tt.X, tt.Y);
            this.Cursor = Cursors.ScrollAll;
            child.CaptureMouse();
        }

        private void MoveUp(MouseButtonEventArgs e)
        {
            if (child == null)
                return;

            child.ReleaseMouseCapture();
            this.Cursor = null;
        }


        /// <summary>
        /// Mouse down event
        /// </summary>
        private void ZoomBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (GetAction(e.ChangedButton))
            {
                case ZoomBorderMouseAction.Move:
                    MoveDown(e);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Mouse up event
        /// </summary>
        private void ZoomBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (GetAction(e.ChangedButton))
            {
                case ZoomBorderMouseAction.Move:
                    MoveUp(e);
                    break;
                case ZoomBorderMouseAction.Reset:
                    Reset();
                    break;

                default:
                    break;
            }

        }

        /// <summary>
        /// Scroll
        /// </summary>
        private void ZoomBorder_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (child == null)
                return;

            var st = _scaleTransform;
            var tt = _translateTransform;

            //Work move
            double zoom = e.Delta * ScaleAmount;

            //Ensure action doesn't exceed the min or max scale
            double newScale = Scale + zoom;
            if (newScale > ScaleMax)
                newScale = ScaleMax;
            if (newScale < ScaleMin)
                newScale = ScaleMin;


            if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                return;

            Point relative = e.GetPosition(child);
            double absoluteX;
            double absoluteY;

            absoluteX = relative.X * Scale + tt.X;
            absoluteY = relative.Y * Scale + tt.Y;

            Scale = newScale;

            tt.X = absoluteX - relative.X * Scale;
            tt.Y = absoluteY - relative.Y * Scale;

        }

        /// <summary>
        /// Mouse move
        /// </summary>
        private void ZoomBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (!ZoomEnabled)
                return;

            if (child == null)
                return;

            if (!child.IsMouseCaptured)
                return;

            var tt = _translateTransform;
            Vector v = start - e.GetPosition(this);
            tt.X = origin.X - v.X;
            tt.Y = origin.Y - v.Y;
        }

        #endregion
    }
}
