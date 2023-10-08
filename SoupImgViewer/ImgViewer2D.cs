using System;
using System.ComponentModel;
using HalconDotNet;

namespace Soup
{
    public partial class ImgViewer
    {
        #region fields
        private HWindow _hWind2D;
        #endregion


        #region props

        public HImage CurrentImg2D { get; set; }


        /// <summary>
        /// default line width of hxldcontour
        /// </summary>
        public double HxldLineWidth { get; set; } = 2.0;

        #endregion


        #region bottom toolBar props

        private int _imageX;
        private int _imageY;
        private string _pointGray;


        public int ImageX
        {
            get { return _imageX; }
            set
            {
                _imageX = value;
                OnPropertyChanged(nameof(ImageX));
            }
        }


        public int ImageY
        {
            get { return _imageY; }
            set
            {
                _imageY = value;
                OnPropertyChanged(nameof(ImageY));
            }
        }


        /// <summary>
        /// gray value of mouse point
        /// </summary>
        public string PointGray
        {
            get { return _pointGray; }
            set
            {
                _pointGray = value;
                OnPropertyChanged(nameof(PointGray));
            }
        }
        #endregion


        #region  init
        /// <summary>
        /// halcon window init
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SmartWindow2D_Initialized(object sender, EventArgs e)
        {
            _hWind2D = new HWindow();
            _hWind2D = SmartWindow2D.HalconWindow;


            var dpd = DependencyPropertyDescriptor.FromProperty(HSmartWindowControlWPF.HImagePartProperty, typeof(HSmartWindowControlWPF));
            dpd.AddValueChanged(SmartWindow2D, (o, es) =>
            {
                var imgPart = SmartWindow2D.HImagePart;
                _k = imgPart.Height / SmartWindow2D.ActualHeight;
                _tx = imgPart.X;
                _ty = imgPart.Y;
            });


            InitDrawRoi();
        }
        #endregion


        #region private methods
        /// <summary>
        /// display pixel’s Gray Value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SmartWindow2D_HMouseMove(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {
            //实时计算鼠标指向的图像坐标和灰度值
            if (CurrentImg2D != null && CurrentImg2D.IsInitialized())
            {
                int row = (int)(e.Row);
                int col = (int)(e.Column);
                ImageX = col;
                ImageY = row;

                CurrentImg2D.GetImageSize(out int imgWidth, out int imgHeight);
                if (row >= 0 && col >= 0 && row <= imgHeight - 1 && col <= imgWidth - 1)
                {
                    HTuple gray = CurrentImg2D.GetGrayval(row, col);
                    if (gray.Type == HTupleType.INTEGER)
                    {
                        PointGray = gray.I.ToString();
                    }
                    if (gray.Type == HTupleType.DOUBLE)
                    {
                        PointGray = gray.D.ToString("F3");
                    }
                    if (gray.Type == HTupleType.LONG)
                    {
                        PointGray = gray.L.ToString();
                    }

                }
                else
                {
                    PointGray = "0";
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="smartWindCtrl"></param>
        /// <param name="obj2d"></param>
        /// <param name="isAppend"></param>
        private void InnerDisp2D(HSmartWindowControlWPF smartWindCtrl, HObject obj2d, bool isAppend = false)
        {
            //we clear the window if it's not append mode 
            if (!isAppend)
            {
                smartWindCtrl.HalconWindow.ClearWindow();
                smartWindCtrl.Items.Clear();
            }

            //we also clear the window if the halcon window has too much objects
            if (smartWindCtrl.Items.Count > 150)
            {
                smartWindCtrl.HalconWindow.ClearWindow();
                smartWindCtrl.Items.Clear();
            }

            // set default color 
            _hWind2D.SetColor(SoupColor.Green25);
            // set default hxld width
            _hWind2D.SetLineWidth(HxldLineWidth);

            int count = obj2d.CountObj();
            if (count == 1)
            {
                HTuple type = obj2d.GetObjClass();
                if (type.S == "image")
                {
                    if (CurrentImg2D != null && CurrentImg2D.IsInitialized()) CurrentImg2D.Dispose();
                    CurrentImg2D = new HImage(obj2d);
                }
                smartWindCtrl.Items.Add(obj2d);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    HObject obj = obj2d.SelectObj(i + 1);
                    HTuple type = obj.GetObjClass();
                    if (type.S != "image")
                    {
                        smartWindCtrl.HalconWindow.SetColor(SoupColor.GetColorRandom());
                    }
                    else
                    {
                        if (CurrentImg2D != null && CurrentImg2D.IsInitialized()) CurrentImg2D.Dispose();
                        CurrentImg2D = new HImage(obj2d);
                    }
                    smartWindCtrl.Items.Add(obj);
                }
            }
        }

        #endregion


        #region public methods
        /// <summary>
        /// display object 2d
        /// </summary>
        /// <param name="obj2d"></param>
        /// <param name="isAppend"></param>
        public void DisplayObject2D(HObject obj2d, bool isAppend = false)
        {
            InvokeIfRequired(() =>
            {
                InnerDisp2D(SmartWindow2D, obj2d, isAppend);
            });
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
        #endregion
    }
}
