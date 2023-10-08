using HalconDotNet;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Soup
{
    /// <summary>
    /// UserControl1.xaml 
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


        #region DP

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


        #region props
        /// <summary>
        /// halcon window background color
        /// </summary>
        public string WindowBackgourdColor
        {
            get;
            set;
        } = "black";

        #endregion


        #region ctor
        public ImgViewer()
        {
            InitializeComponent();
            InitHalconDefaultPara();
        }

        #endregion


        #region private methods

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
        /// init halcon para
        /// </summary>
        private void InitHalconDefaultPara()
        {
            HOperatorSet.SetSystem("width", 2500);
            HOperatorSet.SetSystem("height", 2500);
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

        #endregion
    }
}
