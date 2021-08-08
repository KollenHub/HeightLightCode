using CefSharp.Wpf;
using HeightLightCode.Models;
using HeightLightCode.UI;
using HeightLightCode.Views;
using HeightLightCode.VMBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HeightLightCode.ViewModels
{
    public class MainVm : YINotify
    {
        #region 私有字段
        private static string _datasPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Datas");

        private static string _htmlPath = Path.Combine(_datasPath, @"SyntaxHighLighter\Code.html");

        private SettingModel _model;

        private MainWindow _window;

        private ChromiumWebBrowser _chromWebBrowser;

        private bool _lastTopMost;

        /// <summary>
        /// 窗体正常时的状态
        /// </summary>
        private Rect rcnormal;
        #endregion

        #region 属性

        #region 标题栏

        private Visibility _normalVisiblity = Visibility.Collapsed;
        /// <summary>
        /// 正常按钮显示
        /// </summary>
        public Visibility NormalVisiblity
        {
            get { return _normalVisiblity; }
            set
            {
                _normalVisiblity = value;
                this.Notify(o => NormalVisiblity);
            }
        }


        private Visibility _pinVisiblity = Visibility.Collapsed;
        /// <summary>
        /// 固定按钮显示
        /// </summary>
        public Visibility PinVisiblity
        {
            get { return _pinVisiblity; }
            set
            {
                _pinVisiblity = value;
                this.Notify(o => PinVisiblity);
            }
        }

        #endregion

        /// <summary>
        /// 字体名称
        /// </summary>
        public string FontFamily
        {
            set
            {
                _model.FontFamily = value;
                this.Notify(o => FontFamily);
                Render();
            }
            get
            {
                return _model.FontFamily;
            }
        }



        /// <summary>
        /// 字体集合
        /// </summary>
        public ObservableCollection<string> FontFamilyCollection
        {
            set
            {
                _model.FontFamilyCollection = value;
            }
            get
            {
                return _model.FontFamilyCollection;
            }
        }

        /// <summary>
        /// 字号大小
        /// </summary>
        public FontSize FontSize
        {
            set
            {
                _model.FontSize = value;
                this.Notify(o => FontSize);
                Render();
            }
            get
            {
                return _model.FontSize;
            }
        }

        /// <summary>
        /// 字号集合
        /// </summary>
        public ObservableCollection<FontSize> FontSizeCollection
        {
            get
            {
                return _model.FontSizeCollection;
            }
            set
            {
                _model.FontSizeCollection = value;
            }
        }

        /// <summary>
        /// 代码样式
        /// </summary>
        public string CodeStyle
        {
            get
            {
                return _model.CodeStyle;
            }
            set
            {
                _model.CodeStyle = value;
                this.Notify(o => CodeStyle);
                Render();
            }
        }

        /// <summary>
        /// 代码样式集合
        /// </summary>
        public ObservableCollection<string> CodeStyleCollection
        {
            get
            {
                return _model.CodeStyleCollection;
            }
            set
            {
                _model.CodeStyleCollection = value;
                this.Notify(o => CodeStyleCollection);
            }
        }

        /// <summary>
        /// 编程语言
        /// </summary>
        public LanguageEnum CodeLanguage
        {
            get
            {
                return _model.CodeLanguage;
            }
            set
            {
                _model.CodeLanguage = value;
                this.Notify(o => CodeLanguage);
                Render();
            }
        }

        /// <summary>
        /// 编程语言集合
        /// </summary>
        public ObservableCollection<LanguageEnum> CodeLanguageCollection
        {
            get
            {
                return _model.CodeLanguageCollection;
            }
            set
            {
                _model.CodeLanguageCollection = value;
                this.Notify(o => CodeLanguageCollection);
            }
        }

        /// <summary>
        /// 行号
        /// </summary>
        public bool GutterStatus
        {
            get
            {
                return _model.GutterStatus;
            }
            set
            {
                _model.GutterStatus = value;
                this.Notify(o => GutterStatus);
                Render();
            }
        }


        private bool _isClip;

        /// <summary>
        /// 是否裁剪
        /// </summary>
        public bool IsClip
        {
            get { return _isClip; }
            set
            {
                _isClip = value;
                this.Notify(o => IsClip);
            }
        }

        /// <summary>
        /// 代码数据
        /// </summary>
        public string CodeText { get; set; } = "";

        #endregion

        #region 命令

        /// <summary>
        /// 最大化窗体
        /// </summary>
        public YICommand MaximizedCmd { get; set; }

        /// <summary>
        /// 最小化窗体
        /// </summary>
        public YICommand MinimizedCmd { get; set; }

        /// <summary>
        /// 正常化窗体
        /// </summary>
        public YICommand NormalCmd { get; set; }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        public YICommand CloseCmd { get; set; }

        /// <summary>
        /// 窗体初始化
        /// </summary>
        public YICommand WinInitCmd { get; set; }

        /// <summary>
        /// 固定
        /// </summary>
        public YICommand PinCmd { get; set; }

        /// <summary>
        /// 取消固定
        /// </summary>
        public YICommand UnpinCmd { get; set; }

        /// <summary>
        /// 拖动移动
        /// </summary>
        public YICommand WindowMoveCmd { get; set; }

        /// <summary>
        /// 标题图片鼠标按钮按下
        /// </summary>
        public YICommand TitleImageDownCmd { get; set; }

        /// <summary>
        /// 标题图片鼠标按钮抬起
        /// </summary>
        public YICommand TitleImageUpCmd { get; set; }

        /// <summary>
        /// 标题图片鼠标离开
        /// </summary>
        public YICommand TitleImageLeaveCmd { get; set; }

        /// <summary>
        /// 标题图片鼠标进入
        /// </summary>
        public YICommand TitleImageEnterCmd { get; set; }

        /// <summary>
        /// 标题图片鼠标
        /// </summary>
        public YICommand TitleImageRightDownCmd { get; set; }

        /// <summary>
        /// 复制网页内容
        /// </summary>
        public YICommand CopyCmd { get; set; }

        /// <summary>
        /// 粘贴
        /// </summary>
        public YICommand PasteCmd { get; set; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainVm()
        {
            InitModel();
            PinVisiblity = Visibility.Visible;
            WinInitCmd = new YICommand(WinInit);
            CopyCmd = new YICommand(Copy);
            PasteCmd = new YICommand(Paste);
            PinCmd = new YICommand(Pin);
            UnpinCmd = new YICommand(Unpin);
            CloseCmd = new YICommand(Close);
            NormalCmd = new YICommand(Normal);
            MaximizedCmd = new YICommand(Maximized);
            MinimizedCmd = new YICommand(Minimized);
            WindowMoveCmd = new YICommand(WindowMove);
            TitleImageRightDownCmd = new YICommand(TitleImageRightDown);
            TitleImageUpCmd = new YICommand(TitleImageUp, o => IsClip);
            TitleImageDownCmd = new YICommand(TitleImageDown, o => IsClip);
            TitleImageEnterCmd = new YICommand(TitleImageEnter, o => IsClip);
            TitleImageLeaveCmd = new YICommand(TitleImageLeave, o => IsClip);
        }

        #region 方法

        #region 标题图片变化
        /// <summary>
        /// 标题图片右键点击
        /// </summary>
        /// <param name="obj"></param>
        private void TitleImageRightDown(object obj)
        {
            IsClip = !IsClip;
            if (IsClip)
            {
                _window.ResizeMode = ResizeMode.NoResize;
                _lastTopMost = _window.Topmost;
                _window.Topmost = true;
            }
            else
            {
                _window.ResizeMode = ResizeMode.CanResizeWithGrip;
                _window.Topmost = _lastTopMost;
            }
        }

        /// <summary>
        /// 鼠标离开
        /// </summary>
        /// <param name="obj"></param>
        private void TitleImageLeave(object obj)
        {
            (obj as Image).Margin = new Thickness(5);
        }

        /// <summary>
        /// 鼠标点击
        /// </summary>
        /// <param name="obj"></param>
        private void TitleImageEnter(object obj)
        {
            (obj as Image).Margin = new Thickness(6);
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="obj"></param>
        private void TitleImageDown(object obj)
        {
            (obj as Image).Margin = new Thickness(4);
            Thread thread = new Thread(new ThreadStart(() =>
              {
                  _window.Dispatcher.Invoke(() =>
                  {
                      //粘贴代码
                      Paste(null);
                      //复制代码
                      if (!string.IsNullOrWhiteSpace(CodeText))
                          Copy(null);
                  });
              }));
            thread.Start();

        }

        /// <summary>
        /// 鼠标抬起
        /// </summary>
        /// <param name="obj"></param>
        private void TitleImageUp(object obj)
        {
            (obj as Image).Margin = new Thickness(5);
        }

        #endregion

        #region 窗体方法
        /// <summary>
        /// 窗体初始化
        /// </summary>
        /// <param name="obj"></param>
        private void WinInit(object obj)
        {
            _window = obj as MainWindow;
            _chromWebBrowser = _window.ChromBrower;
            Render();
        }
        /// <summary>
        /// 窗体移动
        /// </summary>
        /// <param name="obj"></param>
        private void WindowMove(object obj)
        {
            var par = obj as YICommandParameter;
            if (par.EventArgs is MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (NormalVisiblity == Visibility.Visible)
                    {
                        NormalVisiblity = Visibility.Collapsed;
                        //当前窗体高度
                        double windowH = _window.ActualHeight;
                        //当前窗体宽度
                        double windowB = _window.ActualWidth;
                        //正常窗体高度
                        double oldWinH = rcnormal.Height;
                        //正常窗体宽度
                        double oldWinB = rcnormal.Width;
                        //当前鼠标所在位置
                        Point mousePoint = e.GetPosition(_window);
                        //小窗左距鼠标位置
                        double leftValue = _window.Left + mousePoint.X - mousePoint.X * oldWinB / windowB;
                        //小窗上距鼠标位置
                        double topValue = _window.Top;
                        _window.Left = leftValue;
                        _window.Top = topValue;
                        _window.Width = oldWinB;
                        _window.Height = oldWinH;
                        NormalVisiblity = Visibility.Collapsed;
                    }
                    _window.DragMove();
                }
            }
        }

        /// <summary>
        /// 取消窗口置顶
        /// </summary>
        /// <param name="obj"></param>
        private void Unpin(object obj)
        {
            _window.Topmost = false;
            PinVisiblity = Visibility.Collapsed;
        }

        /// <summary>
        /// 置顶窗口
        /// </summary>
        /// <param name="obj"></param>
        private void Pin(object obj)
        {
            _window.Topmost = true;
            PinVisiblity = Visibility.Visible;
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="obj"></param>
        private void Close(object obj)
        {
            SaveModel();
            //终止所有线程 
            Environment.Exit(Environment.ExitCode);
        }

        /// <summary>
        /// 正常化窗口
        /// </summary>
        /// <param name="obj"></param>
        private void Normal(object obj)
        {
            _window.Left = rcnormal.Left;
            _window.Top = rcnormal.Top;
            _window.Width = rcnormal.Width;
            _window.Height = rcnormal.Height;
            NormalVisiblity = Visibility.Collapsed;
        }

        /// <summary>
        /// 最小化窗口
        /// </summary>
        /// <param name="obj"></param>
        private void Minimized(object obj)
        {
            IsClip = true;
            _window.Topmost = true;
        }

        /// <summary>
        /// 最大化窗口
        /// </summary>
        /// <param name="obj"></param>
        private void Maximized(object obj)
        {
            rcnormal = new Rect(_window.Left, _window.Top, _window.Width, _window.Height);//保存下当前位置与大小
            //查找所有的显示器
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                //获取工作区大小
                var workArea = screen.WorkingArea;
                if (_window.Top >= workArea.Top && _window.Top <= workArea.Top + workArea.Height && _window.Left + _window.Width <= workArea.Right && _window.Left + _window.Width >= _window.Left)
                {
                    //获取工作区大小
                    _window.Left = workArea.Left;//设置位置
                    _window.Top = workArea.Top;
                    _window.Width = workArea.Width;
                    _window.Height = workArea.Height;
                }
            }
            NormalVisiblity = Visibility.Visible;
        }
        #endregion

        #region 窗体配置
        /// <summary>
        /// 初始化Model
        /// </summary>
        private void InitModel()
        {
            //拿到配置路径
            string path = Path.Combine(_datasPath, "Setting.json");
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    _model = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingModel>(json);
                }
                catch
                {

                }
            }
            if (_model == null)
            {
                _model = new SettingModel();
            }

        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private void SaveModel()
        {
            //拿到配置路径
            string path = Path.Combine(_datasPath, "Setting.json");
            try
            {
                string json = JsonConvert.SerializeObject(_model);
                File.WriteAllText(path, json);
            }
            catch
            {

            }
        }
        #endregion

        #region 网页

        /// <summary>
        /// 粘贴到剪切板
        /// </summary>
        /// <param name="obj"></param>
        private void Paste(object obj)
        {
            string text = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            else
            {
                CodeText = text;
                Render();
            }
        }

        private void Copy(object obj)
        {
            var mainFrame = _chromWebBrowser.GetBrowser().MainFrame;
            mainFrame.SelectAll();
            mainFrame.Copy();
        }
        /// <summary>
        /// 更改Html文件，重新加载渲染网页
        /// </summary>
        private void Render()
        {
            if (_chromWebBrowser == null) return;
            var htmlLineList = File.ReadAllLines(_htmlPath, Encoding.UTF8).ToList();
            string gutterStatus = GutterStatus ? "true" : "false";
            for (int i = 0; i < htmlLineList.Count; i++)
            {
                var current = htmlLineList[i];
                if (current.Contains("script") && current.Contains("codeTypeJs"))
                {
                    htmlLineList[i] = Regex.Replace(current, "scripts.+.js", $@"scripts/shBrush{CodeLanguage}.js");
                }
                else if (current.Contains("CShape代碼調用方法"))
                {
                    htmlLineList[i + 1] = $"HeightLight('{CodeStyle}','{FontFamily}',{(int)FontSize},{gutterStatus});";
                }
                else if (current.Contains("pre") && current.Contains("code_pre"))
                {
                    htmlLineList[i] = $"<pre id=\"code_pre\" class=\"brush:{CodeLanguage}\">";
                    if (!string.IsNullOrEmpty(CodeText))
                    {
                        i++;
                        while (true)
                        {
                            if (i > htmlLineList.Count - 1) throw new Exception("示例代码文件Code.html已损坏");
                            var htmlLineValue = htmlLineList[i];
                            if (htmlLineValue.Contains("</pre>"))
                            {
                                break;
                            }
                            else
                            {
                                htmlLineList.RemoveAt(i);
                            }
                        }
                        htmlLineList.Insert(i++, CodeText);
                        break;
                    }
                }
            }
            File.WriteAllLines(_htmlPath, htmlLineList, Encoding.UTF8);
            _chromWebBrowser.Load(_htmlPath);
            Thread.Sleep(100);
        }
        #endregion
        #endregion
    }
}
