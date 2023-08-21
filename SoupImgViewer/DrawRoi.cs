using HalconDotNet;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Soup
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ImgViewer
    {
        #region 字段
        //鼠标起始点和结束点
        private Point _startPoint;
        private Point _endPoint;

        //canvas上的对象
        //Rectangle
        private Rectangle _canvasRect = new Rectangle();

        //圆形ROI
        private Ellipse _canvasEllipse = new Ellipse();
        private Path _cross = new Path();

        //多边形ROI
        private Polygon _canvasPolygon = new Polygon();
        private Polyline _canvasPolyline = new Polyline();
        private Line _canvasMouseLine = new Line();

        //roi
        private HDrawingObject _roi; 
        private HRegion _roiRegion = new HRegion();

        private double _r1 = 0;
        private double _c1 = 0;
        private double _r2 = 0;
        private double _c2 = 0;

        private double _circleRow;
        private double _circleCol;
        private double _circleRadius;

        private double _k = 1;
        private double _tx = 0;
        private double _ty = 0;

        private bool _polyLineStartDrawed = false;
        private bool _rectangleDrawed = false;
        private bool _circleDrawed = false;
        #endregion


        #region 属性
        public static readonly DependencyProperty IsMouseArrowBtnCheckedProperty =
        DependencyProperty.Register("IsMouseArrowBtnChecked", typeof(bool), typeof(UserControl), new PropertyMetadata(true));

        public bool IsMouseArrowBtnChecked
        {
            get { return (bool)GetValue(IsMouseArrowBtnCheckedProperty); }
            set { SetValue(IsMouseArrowBtnCheckedProperty, value); }
        }


        public string StrokeColor { get; set; } = "#6495ed";

        public double StokeThickness { get; set; } = 2;


        public HRegion RoiRegion
        {
            get
            {
                if (_roi == null || !_roi.IsInitialized()) return null;
                _roiRegion = new HRegion();
                HTuple type = _roi.GetDrawingObjectParams(new HTuple("type"));
                if(type.S == "rectangle1")
                {
                    HTuple para = _roi.GetDrawingObjectParams(new HTuple("row1", "column1", "row2", "column2"));
                    _roiRegion.GenRectangle1(para[0].D, para[1].D, para[2].D, para[3].D);
                 }
                if(type.S == "circle")
                {
                    HTuple para = _roi.GetDrawingObjectParams(new HTuple("row", "column", "radius"));
                    _roiRegion.GenCircle(para[0].D, para[1], para[2]);
                }
                if(type.S == "xld")
                {
                    HObject xldObj = _roi.GetDrawingObjectIconic();
                    HXLDCont xld = new HXLDCont(xldObj);
                    _roiRegion = xld.GenRegionContourXld("filled");
                }

                return _roiRegion;
            }
        }

        #endregion


        #region UI事件交互

        private void MouseCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _startPoint = e.GetPosition(MouseCanvas);

                if (RoiCustomDraw.IsChecked == true && e.ClickCount == 1)
                {
                    _polyLineStartDrawed = true;

                    _canvasPolyline.Visibility = Visibility.Visible;
                    var currentPoint = e.GetPosition(MouseCanvas);
                    _canvasPolyline.Points.Add(currentPoint);
                    _startPoint = currentPoint;
                }

                //鼠标双击，结束绘制多边形
                if (RoiCustomDraw.IsChecked == true && e.ClickCount == 2)
                {
                    _canvasPolygon.Visibility = Visibility.Visible;

                    HTuple rows = new HTuple();
                    HTuple cols = new HTuple();
                    foreach (var p in _canvasPolyline.Points)
                    {
                        _canvasPolygon.Points.Add(p);

                        ConvertPoint(p.X, p.Y, _k, _tx, _ty, out double c, out double r);
                        rows.Append(r);
                        cols.Append(c);
                    }

                    _canvasPolyline.Visibility = Visibility.Collapsed;
                    _canvasMouseLine.Visibility = Visibility.Collapsed;
                    RoiCustomDraw.IsChecked = false;
                    _polyLineStartDrawed = false;

                    rows.Append(rows[0]);
                    cols.Append(cols[0]);
                    _roi = HDrawingObject.CreateDrawingObject(HDrawingObject.HDrawingObjectType.XLD_CONTOUR, rows, cols);
                    _roi.SetDrawingObjectParams(new HTuple("color"), new HTuple(StrokeColor));
                    _hWind2D.AttachDrawingObjectToWindow(_roi);
                }
            }
        }

        private void MouseCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //矩形ROI
            if (e.LeftButton == MouseButtonState.Pressed && RoiRect.IsChecked == true)
            {
                _rectangleDrawed = true;
                _endPoint = e.GetPosition(MouseCanvas);

                _canvasRect.Visibility = Visibility.Visible;
                _canvasRect.Height = Math.Abs(_endPoint.Y - _startPoint.Y);
                _canvasRect.Width = Math.Abs(_endPoint.X - _startPoint.X);

                //左上到右下
                if (_endPoint.X > _startPoint.X && _endPoint.Y > _startPoint.Y)
                {
                    Canvas.SetLeft(_canvasRect, _startPoint.X);
                    Canvas.SetTop(_canvasRect, _startPoint.Y);

                    //坐标系转换
                    ConvertPoint(_startPoint.X, _startPoint.Y, _k, _tx, _ty, out double sx, out double sy);
                    ConvertPoint(_endPoint.X, _endPoint.Y, _k, _tx, _ty, out double ex, out double ey);

                    _r1 = sy;
                    _c1 = sx;
                    _r2 = ey;
                    _c2 = ex;
                }

                // 右下到左上
                if (_endPoint.X < _startPoint.X && _endPoint.Y < _startPoint.Y)
                {
                    Canvas.SetLeft(_canvasRect, _endPoint.X);
                    Canvas.SetTop(_canvasRect, _endPoint.Y);

                    ConvertPoint(_startPoint.X, _startPoint.Y, _k, _tx, _ty, out double endX, out double endY);
                    ConvertPoint(_endPoint.X, _endPoint.Y, _k, _tx, _ty, out double startX, out double startY);
                    _r1 = startY;
                    _c1 = startX;
                    _r2 = endY;
                    _c2 = endX;
                }

                // 左下到右上
                if (_endPoint.X > _startPoint.X && _endPoint.Y < _startPoint.Y)
                {
                    Canvas.SetLeft(_canvasRect, _startPoint.X);
                    Canvas.SetTop(_canvasRect, _endPoint.Y);

                    ConvertPoint(_startPoint.X, _endPoint.Y, _k, _tx, _ty, out double startX, out double startY);
                    ConvertPoint(_endPoint.X, _startPoint.Y, _k, _tx, _ty, out double endX, out double endY);
                    _r1 = startY;
                    _c1 = startX;
                    _r2 = endY;
                    _c2 = endX;
                }

                // 右上到左下
                if (_endPoint.X < _startPoint.X && _endPoint.Y > _startPoint.Y)
                {
                    Canvas.SetLeft(_canvasRect, _endPoint.X);
                    Canvas.SetTop(_canvasRect, _startPoint.Y);

                    ConvertPoint(_endPoint.X, _startPoint.Y, _k, _tx, _ty, out double startX, out double startY);
                    ConvertPoint(_startPoint.X, _endPoint.Y, _k, _tx, _ty, out double endX, out double endY);
                    _r1 = startY;
                    _c1 = startX;
                    _r2 = endY;
                    _c2 = endX;
                }
            }


            //圆形ROI
            if (e.LeftButton == MouseButtonState.Pressed && RoiCircle.IsChecked == true)
            {
                _circleDrawed = true;
                _endPoint = e.GetPosition(MouseCanvas);
                _canvasEllipse.Visibility = Visibility.Visible;
                _cross.Visibility = Visibility.Visible;

                _circleRadius = Math.Sqrt(Math.Pow((_endPoint.Y - _startPoint.Y), 2) + Math.Pow((_endPoint.X - _startPoint.X), 2));
                _canvasEllipse.Width = _circleRadius * 2;
                _canvasEllipse.Height = _circleRadius * 2;
                _cross.Width = 8;
                _cross.Height = 8;

                ConvertPoint(_startPoint.X, _startPoint.Y, _k, _tx, _ty, out double sx, out double sy);
                _circleRow = sy;
                _circleCol = sx;


                Canvas.SetLeft(_canvasEllipse, _startPoint.X - _circleRadius);
                Canvas.SetTop(_canvasEllipse, _startPoint.Y - _circleRadius);
                Canvas.SetLeft(_cross, _startPoint.X);
                Canvas.SetTop(_cross, _startPoint.Y);
            }


            // 多边形ROI
            if (RoiCustomDraw.IsChecked == true && _polyLineStartDrawed)
            {
                _canvasPolyline.Visibility = Visibility.Visible;
                _canvasMouseLine.Visibility = Visibility.Visible;

                _canvasMouseLine.X1 = _startPoint.X;
                _canvasMouseLine.Y1 = _startPoint.Y;

                _endPoint = e.GetPosition(MouseCanvas);

                _canvasMouseLine.X2 = _endPoint.X;
                _canvasMouseLine.Y2 = _endPoint.Y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (RoiRect.IsChecked == true && _rectangleDrawed == true)
            {
                _roi = HDrawingObject.CreateDrawingObject(HDrawingObject.HDrawingObjectType.RECTANGLE1, _r1, _c1, _r2, _c2);
                _roi.SetDrawingObjectParams(new HTuple("color"), new HTuple(StrokeColor));
                _hWind2D.AttachDrawingObjectToWindow(_roi);
                RoiRect.IsChecked = false;
                _rectangleDrawed = false;
            }


            if (RoiCircle.IsChecked == true && _circleDrawed == true)
            {
                _roi = HDrawingObject.CreateDrawingObject(HDrawingObject.HDrawingObjectType.CIRCLE, _circleRow, _circleCol, _circleRadius);
                _roi.SetDrawingObjectParams(new HTuple("color"), new HTuple(StrokeColor));
                _hWind2D.AttachDrawingObjectToWindow(_roi);
                RoiCircle.IsChecked = false;
                _circleDrawed = false;
            }
        }


        private void RoiRect_Checked(object sender, RoutedEventArgs e)
        {
            _canvasRect.Visibility = Visibility.Collapsed;
            DisplayCanvasAndClearDrawingObject();
        }

        private void RoiRect_Unchecked(object sender, RoutedEventArgs e)
        {
            MouseCanvas.Visibility = Visibility.Collapsed;
            _canvasRect.Visibility = Visibility.Collapsed;
        }


        private void RoiCircle_Checked(object sender, RoutedEventArgs e)
        {
            _canvasEllipse.Visibility = Visibility.Collapsed;
            DisplayCanvasAndClearDrawingObject();
        }


        private void RoiCircle_Unchecked(object sender, RoutedEventArgs e)
        {
            MouseCanvas.Visibility = Visibility.Collapsed;
            _canvasEllipse.Visibility = Visibility.Collapsed;
            _cross.Visibility = Visibility.Collapsed;
        }


        private void RoiCustomDraw_Checked(object sender, RoutedEventArgs e)
        {
            _canvasPolygon.Visibility = Visibility.Collapsed;
            _canvasPolyline.Visibility = Visibility.Collapsed;
            _canvasMouseLine.Visibility = Visibility.Collapsed;

            _canvasPolyline.Points.Clear();
            _canvasPolygon.Points.Clear();
            _canvasMouseLine.X1 = 0;
            _canvasMouseLine.Y1 = 0;
            _canvasMouseLine.X2 = 0;
            _canvasMouseLine.Y2 = 0;
            DisplayCanvasAndClearDrawingObject();
        }


        private void RoiCustomDraw_Unchecked(object sender, RoutedEventArgs e)
        {
            MouseCanvas.Visibility = Visibility.Collapsed;
            _canvasPolygon.Visibility = Visibility.Collapsed;
        }

        
        /// <summary>
        /// 此时恢复窗口默认状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragBtn_Checked(object sender, RoutedEventArgs e)
        {
            ClearROI();
        }


        private void DragBtn_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        #endregion


        #region 私有方法
        /// <summary>
        /// 初始化操作
        /// </summary>
        private void InitDrawRoi()
        {
            MouseCanvas.Children.Add(_canvasRect);
            MouseCanvas.Children.Add(_canvasEllipse);
            MouseCanvas.Children.Add(_cross);

            MouseCanvas.Children.Add(_canvasPolyline);
            MouseCanvas.Children.Add(_canvasMouseLine);
            MouseCanvas.Children.Add(_canvasPolygon);

            //矩形
            _canvasRect.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(StrokeColor));
            _canvasRect.StrokeThickness = StokeThickness;
            _canvasRect.Visibility = Visibility.Collapsed;

            //圆形
            _canvasEllipse.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(StrokeColor));
            _canvasEllipse.StrokeThickness = StokeThickness;
            _canvasEllipse.Visibility = Visibility.Collapsed;

            //圆心十字
            _cross.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(StrokeColor));
            _cross.StrokeThickness = StokeThickness;
            _cross.Data = PathGeometry.Parse("M0,4 L8,4 M4,0 L4,8");
            _cross.Visibility = Visibility.Collapsed;

            //多边形
            _canvasPolyline.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(StrokeColor));
            _canvasPolyline.StrokeThickness = StokeThickness;
            _canvasPolyline.Visibility = Visibility.Collapsed;

            _canvasMouseLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(StrokeColor));
            _canvasMouseLine.StrokeThickness = StokeThickness;
            _canvasMouseLine.Visibility = Visibility.Collapsed;

            _canvasPolygon.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(StrokeColor));
            _canvasPolygon.StrokeThickness = StokeThickness;
            _canvasPolygon.Visibility = Visibility.Collapsed;


            // HImagePart随着窗口大小变化实时变动，注册事件用于实时更新变换信息
            var dpd = DependencyPropertyDescriptor.FromProperty(HSmartWindowControlWPF.HImagePartProperty, typeof(HSmartWindowControlWPF));
            dpd.AddValueChanged(SmartWindow2D, (o, e) =>
            {
                var imgPart = SmartWindow2D.HImagePart;
                _k = imgPart.Height / SmartWindow2D.ActualHeight;
                _tx = imgPart.X;
                _ty = imgPart.Y;
            });
        }



        /// 坐标系转换Canvas => HSmartWindowControl
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="k"></param>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        private void ConvertPoint(double x, double y, double k, double tx, double ty, out double px, out double py)
        {
            px = k * x + tx;
            py = k * y + ty;
        }


        private void DisplayCanvasAndClearDrawingObject()
        {
            MouseCanvas.Visibility = Visibility.Visible;

            //清除当前ROI
            if (_roi != null && _roi.IsInitialized())
            {
                _hWind2D.DetachDrawingObjectFromWindow(_roi);
                _roi.Dispose();
            }
        }

        #endregion


        #region 公开方法

        public void ClearROI()
        {
            //清除当前ROI
            if (_roi != null && _roi.IsInitialized())
            {
                _hWind2D.DetachDrawingObjectFromWindow(_roi);
                _roi.Dispose();
            }
        }
        #endregion
    }
}
