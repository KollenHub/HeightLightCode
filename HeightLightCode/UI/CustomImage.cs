using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HeightLightCode.UI
{
    public class CustomImage : Image
    {
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set { SetValue(IsPressedProperty, value); }
        }
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(CustomImage), new PropertyMetadata(false));



        public bool IsMoveOver
        {
            get { return (bool)GetValue(IsMoveOverProperty); }
            set { SetValue(IsMoveOverProperty, value); }
        }

        public static readonly DependencyProperty IsMoveOverProperty =
            DependencyProperty.Register("IsMoveOver", typeof(bool), typeof(CustomImage), new PropertyMetadata(false));



        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            base.Margin = new Thickness(4);
            e.Handled = true;
            IsPressed = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            base.Margin = new Thickness(5);
            e.Handled = true;
            IsPressed = false;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            base.Margin = new Thickness(5);
            e.Handled = true;
            IsMoveOver = false;
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            base.Margin = new Thickness(6);
            e.Handled = true;
            IsMoveOver = true;
        }

    }
}
