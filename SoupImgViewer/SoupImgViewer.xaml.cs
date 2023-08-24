using HalconDotNet;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Soup
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class ImgViewer : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion


        #region 事件

        /// <summary>
        /// 
        /// 需要等到HWindowSmartControl初始化完成，在这之前对Window的操作都是无效的
        /// </summary>
        public Action OnInited { get; set; }

        #endregion


        #region 控件依赖属性

        public static readonly DependencyProperty IsDisplayTopToolBarProperty =
        DependencyProperty.Register("IsDisplayTopToolBar", typeof(bool), typeof(UserControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsDisplaySideBarProperty =
        DependencyProperty.Register("IsDisplaySideBar", typeof(bool), typeof(UserControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsDisplayBottomBarProperty =
        DependencyProperty.Register("IsDisplayBottomBar", typeof(bool), typeof(UserControl), new PropertyMetadata(false));


        /// <summary>
        /// 是否显示顶部工具栏
        /// </summary>
        public bool IsDisplayTopToolBar
        {
            get { return (bool)GetValue(IsDisplayTopToolBarProperty); }
            set { SetValue(IsDisplayTopToolBarProperty, value); }
        }


        /// <summary>
        /// 是否显示左侧工具栏
        /// </summary>
        public bool IsDisplaySideBar
        {
            get { return (bool)GetValue(IsDisplaySideBarProperty); }
            set { SetValue(IsDisplaySideBarProperty, value); }
        }


        /// <summary>
        /// 是否显示底边工具栏
        /// </summary>
        public bool IsDisplayBottomBar
        {
            get { return (bool)GetValue(IsDisplayBottomBarProperty); }
            set { SetValue(IsDisplayBottomBarProperty, value); }
        }

        #endregion


        /// <summary>
        /// 窗口背景颜色，可以是RGB字符串,例如"#aabbcc"
        /// </summary>
        public string WindowBackgourdColor
        {
            get;
            set;
        } = "black";


        public bool IsInit { get; set; } = false;




        public ImgViewer()
        {
            InitializeComponent();
            InitHalconDefaultPara();
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SmartWindow2D_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Visibility != Visibility.Visible) return;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
        }


        /// <summary>
        /// clear window
        /// </summary>
        public void Clear2D()
        {
            if (CurrentImg2D != null && CurrentImg2D.IsInitialized())
            {
                CurrentImg2D.Dispose();
            }
            SmartWindow2D.Items.Clear();
            _hWind2D.ClearWindow();
        }



        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitHalconDefaultPara()
        {
            HOperatorSet.SetSystem("width", 2000);
            HOperatorSet.SetSystem("height", 2000);
        }

   

        private void InvokeIfRequired(Action action)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(action);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}
