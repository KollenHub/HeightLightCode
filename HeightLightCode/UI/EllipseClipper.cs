using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HeightLightCode.UI
{
    /// <summary>
    /// 剪切图标
    /// </summary>
    public static class EllipseClipper
    {
        private static UIElement sourceObject;


        /// <summary>
        /// 标识 SourceObject 的附加属性。
        /// </summary>
        public static readonly DependencyProperty ClipReferObjectProperty = DependencyProperty.RegisterAttached(
            "ClipReferObject", typeof(DependencyObject), typeof(EllipseClipper), new UIPropertyMetadata(null, OnSourceChanged));


        public static void SetClipReferObject(DependencyObject element, DependencyObject value)
           => element.SetValue(ClipReferObjectProperty, value);

        public static DependencyObject GetClipReferObject(DependencyObject element)
            => (DependencyObject)element.GetValue(ClipReferObjectProperty);

        /// <summary>
        /// 窗体是否裁剪
        /// </summary>
        public static readonly DependencyProperty IsWinClipProperty = DependencyProperty.RegisterAttached(
            "IsWinClip", typeof(bool), typeof(EllipseClipper), new UIPropertyMetadata(false, OnIsClipProperty));


        public static void SetIsWinClip(DependencyObject element, bool value)
           => element.SetValue(IsWinClipProperty, value);

        public static bool GetIsWinClip(DependencyObject element)
            => (bool)element.GetValue(IsWinClipProperty);

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement window = d as Window;
            if (window == null) return;
            if (!(e.NewValue is UIElement clipReferObject))
            {
                // 如果SourceObject附加属性被设置为 null，则清除 UIElement.Clip 属性。
                window.ClearValue(UIElement.ClipProperty);
                return;
            }
            sourceObject = clipReferObject;
            if (GetIsWinClip(window) == false)
            {
                window.ClearValue(UIElement.ClipProperty);
                SetClipReferObject(window, null);
                return;
            }
            // 如果 UIElement.Clip 属性被用作其他用途，则抛出异常说明问题所在。
            var ellipse = window.Clip as EllipseGeometry;
            if (window?.Clip != null && ellipse == null)
            {
                throw new InvalidOperationException(
                    $"{typeof(EllipseClipper).FullName}.{ClipReferObjectProperty.Name} " +
                    $"is using {window.GetType().FullName}.{UIElement.ClipProperty.Name} " +
                    "for clipping, dont use this property manually.");
            }

            // 使用 UIElement.Clip 属性。
            ellipse = ellipse ?? new EllipseGeometry();
            window.Clip = ellipse;

            // 使用绑定来根据控件的宽高更新椭圆裁剪范围。
            var xBinding = new Binding(FrameworkElement.ActualWidthProperty.Name)
            {
                Source = clipReferObject,
                Mode = BindingMode.OneWay,
                Converter = new HalfConverter(),
            };
            var yBinding = new Binding(FrameworkElement.ActualHeightProperty.Name)
            {
                Source = clipReferObject,
                Mode = BindingMode.OneWay,
                Converter = new HalfConverter(),
            };
            var xyBinding = new MultiBinding
            {
                Converter = new SizeToClipCenterConverter(),
            };
            xyBinding.Bindings.Add(xBinding);
            xyBinding.Bindings.Add(yBinding);
            BindingOperations.SetBinding(ellipse, EllipseGeometry.RadiusXProperty, xBinding);
            BindingOperations.SetBinding(ellipse, EllipseGeometry.RadiusYProperty, yBinding);
            BindingOperations.SetBinding(ellipse, EllipseGeometry.CenterProperty, xyBinding);
        }

        /// <summary>
        /// 是否剪切属性变更
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIsClipProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is true)
            {
                SetClipReferObject(d, sourceObject);
            }
            else
            {
                SetClipReferObject(d, null);
            }
        }



        private sealed class SizeToClipCenterConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                Point p = new Point(28.5, 28.5);
                return p;
            }
            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
                => throw new NotSupportedException();
        }

        private sealed class HalfConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                double b = (double)value / 2;
                return b;
            }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                => throw new NotSupportedException();
        }
    }
}