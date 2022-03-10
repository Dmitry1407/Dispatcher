using System;
using System.Windows.Threading;

using Windows = System.Windows;
using Media = System.Windows.Media;

using Core.model;
using Core.model.design.graphics.shape;
using Core.model.design.window;
using Core.model.design.graphics;
using Core.model.design.graphics.control;
using Core.service.graphics;
using System.Windows.Input;
using System.Collections.Generic;
using Core.model.core.archive;

namespace Editor.view.workspace.run
{
    public class WSRunView : WSView
    {
        public ViewController ViewController { get; set; }
        private DispatcherTimer redrawTimer;

        private Boolean isMouseLBPressed = false;
        Control selectedControl;

        public WSRunView() : this(Model.GetInstance()) { }

        public WSRunView(Model model)
        {
            Model = model;
            Controller = new WSRunController(model);
            ViewController = null;

            // Mouse events
            this.MouseLeftButtonDown += OnMouseLBDown;
            this.MouseLeftButtonUp += OnMouseLBUp;
            this.MouseMove += MyOnMouseMove;

            // Redraw Timer
            redrawTimer = new DispatcherTimer();
            redrawTimer.Tick += new EventHandler(DispatcherTimer_Tick);
        }

        public override void DrawWindow(Window window)
        {
            if (window != null)
            {
                this.Background = new Media.SolidColorBrush(window.BackColor);
                this.Width = window.Width;
                this.Height = window.Height;

                foreach (GElement element in window.ElementStorage.Values)
                {
                    if (element != null && element.IsVisible)
                    {
                        if (elementDictionary.ContainsKey(element.ID) && elementDictionary[element.ID] != null)
                        {
                            if (element.EType == GElementType.Shape && ((Shape)element).SType == ShapeType.Picture) { continue; }
                            GVisual visual = elementDictionary[element.ID];
                            ////RedrawElement(element);
                            RemoveVisual(visual);
                            DrawElement(element);
                        }
                        else
                        {
                            DrawElement(element);
                        }
                    }
                }
            }
        }

        public override void DrawLine(Line line)
        {
            GVisual visual = new GVisual();
            using (Media.DrawingContext dc = visual.RenderOpen())
            {
                Int32 startX = line.X;
                Int32 endX = line.X + line.DX;
                Int32 startY = line.Y;
                Int32 endY = line.Y + line.DY;

                // Snap Element 
                SnapElement(dc, startX, startY);

                // Draw backlayer Rectangle 
                dc.DrawRectangle(Media.Brushes.Transparent, null,
                    new Windows.Rect(Math.Min(startX, endX) - 5, Math.Min(startY, endY) - 5,
                    Math.Abs(line.DX) + 10, Math.Abs(line.DY) + 10));

                // Draw Line
                Media.Pen pen = GetPen(line.GetBackColor(), line.GetThickness());
                dc.DrawLine(pen, new Windows.Point(startX, startY), new Windows.Point(endX, endY));
            }
            visual.GElement = line;
            AddVisual(visual);
        }

        public override void DrawPath(Path path)
        {
            GVisual visual = new GVisual();
            using (Media.DrawingContext dc = visual.RenderOpen())
            {
                Media.Pen transparentPen = GetPen(Media.Colors.Transparent, path.Thickness + 5);
                Media.Pen pen = GetPen(path.GetBackColor(), path.GetThickness());

                // Snap Element 
                SnapElement(dc, path.X, path.Y);

                Int32 startX = path.X;
                Int32 startY = path.Y;
                Int32 endX = 0;
                Int32 endY = 0;

                // Draw segments
                for (Int32 i = 0; i < path.Points.Count; i++)
                {
                    endX = startX + path.Points[i].X;
                    endY = startY + path.Points[i].Y;

                    Windows.Point point1 = new Windows.Point(startX, startY);
                    Windows.Point point2 = new Windows.Point(endX, endY);
                    dc.DrawLine(transparentPen, point1, point2);
                    dc.DrawLine(pen, point1, point2);

                    startX = endX;
                    startY = endY;
                }
            }
            visual.GElement = path;
            AddVisual(visual);
        }

        public override void DrawRectangle(Rectangle rectangle)
        {
            GVisual visual = new GVisual();
            using (Media.DrawingContext dc = visual.RenderOpen())
            {
                Media.Brush brush = GetBrush(rectangle.GetBackColor());
                Media.Pen pen = GetPen(rectangle.GetBorderColor(), rectangle.GetThickness());
                Windows.Rect winRect = new Windows.Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

                // Snap Element 
                SnapElement(dc, rectangle.X, rectangle.Y);
                // Draw Rectangle
                if (rectangle.Round > 0) { dc.DrawRoundedRectangle(brush, pen, winRect, rectangle.Round, rectangle.Round); }
                else { dc.DrawRectangle(brush, pen, winRect); }
            }
            visual.GElement = rectangle;
            AddVisual(visual);
        }

        public override void DrawCircle(Circle circle)
        {
            GVisual visual = new GVisual();
            using (Media.DrawingContext dc = visual.RenderOpen())
            {
                Media.Brush brush = GetBrush(circle.GetBackColor());
                Media.Pen pen = GetPen(circle.GetBorderColor(), circle.GetThickness());
                // Snap Element 
                SnapElement(dc, circle.X, circle.Y);
                dc.DrawEllipse(brush, pen, new Windows.Point(circle.X + circle.Radius, circle.Y + circle.Radius), circle.Radius, circle.Radius);
            }
            visual.GElement = circle;
            AddVisual(visual);
        }

        public override void DrawPolygon(Polygon polygon)
        {
            GVisual visual = new GVisual();
            using (Media.DrawingContext dc = visual.RenderOpen())
            {
                Media.Brush brush = GetBrush(polygon.GetBackColor());
                Media.Pen pen = GetPen(polygon.GetBorderColor(), polygon.GetThickness());

                // Snap Element 
                SnapElement(dc, polygon.X, polygon.Y);

                Int32 startX = polygon.X;
                Int32 startY = polygon.Y;
                Int32 nextX = 0;
                Int32 nextY = 0;

                // Init LineSegments
                var segments = new Media.LineSegment[polygon.Points.Count];
                for (Int32 i = 0; i < polygon.Points.Count; i++)
                {
                    nextX = startX + polygon.Points[i].X;
                    nextY = startY + polygon.Points[i].Y;
                    segments[i] = new Media.LineSegment(new Windows.Point(nextX, nextY), true);
                    startX = nextX;
                    startY = nextY;
                }

                // Draw Polygon
                Windows.Point startPoint = new Windows.Point(polygon.X, polygon.Y);
                var figure = new Media.PathFigure(startPoint, segments, true);
                var geometry = new Media.PathGeometry(new[] { figure });
                dc.DrawGeometry(brush, pen, geometry);
            }
            visual.GElement = polygon;
            AddVisual(visual);
        }

        public override void DrawText(Text text)
        {
            GVisual visual = new GVisual();
            using (Media.DrawingContext dc = visual.RenderOpen())
            {
                Media.Brush brush = GetBrush(text.TextColor);
                Media.FormattedText formattedText = new Media.FormattedText(
                    text.Value,
                    System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                    Windows.FlowDirection.LeftToRight,
                    new Media.Typeface(text.FontName != null && text.FontName.Length > 0 ? text.FontName : "Verdana"),
                    text.FontSize > 0 ? text.FontSize : 12,
                    brush);

                // Snap Element 
                SnapElement(dc, text.X, text.Y);

                // Draw Transparent fone
                Media.Pen rectPen = GetPen(text.BorderColor, text.Thickness);
                Media.Brush rectBrush = GetBrush(text.BackColor);
                Windows.Rect winRect = new Windows.Rect(text.X, text.Y, formattedText.Width, formattedText.Height);
                dc.DrawRectangle(rectBrush, rectPen, winRect);

                // Draw Text
                dc.DrawText(formattedText, new System.Windows.Point(text.X, text.Y));
            }
            visual.GElement = text;
            AddVisual(visual);
        }

        public override void DrawPicture(Picture picture)
        {
            if (picture != null && !elementDictionary.ContainsKey(picture.ID))
            {
                GVisual visual = new GVisual();
                using (Media.DrawingContext dc = visual.RenderOpen())
                {
                    if (picture != null)
                    {
                        // Draw Picture
                        Media.Imaging.BitmapImage image = null;
                        if (picture.FileName != null && picture.FileName.Length > 0)
                        {
                            if (pictureStorage.ContainsKey(picture.FileName))
                            {
                                image = pictureStorage[picture.FileName];
                            }
                            else if (System.IO.File.Exists(picture.FileName))
                            {
                                image = new Media.Imaging.BitmapImage(new Uri(picture.FileName));
                                pictureStorage[picture.FileName] = image;
                            }
                        }
                        if (image != null)
                        {
                            dc.DrawImage(image, new Windows.Rect(picture.X, picture.Y, picture.Width, picture.Height));
                        }
                        else
                        {
                            // Draw Rectangle
                            Media.Brush brush = Media.Brushes.Gray;
                            Media.Pen pen = GetPen(Media.Colors.Black, 1);
                            Windows.Rect winRect = new Windows.Rect(picture.X, picture.Y, picture.Width, picture.Height);

                            // Snap Element 
                            SnapElement(dc, picture.X, picture.Y);
                            // Draw Rectangle
                            dc.DrawRectangle(brush, pen, winRect);
                        }
                    }
                }
                visual.GElement = picture;
                AddVisual(visual);
            }
        }

        public override void DrawButton(Button button)
        {
            GVisual visual = new GVisual();
            using (Media.DrawingContext dc = visual.RenderOpen())
            {
                Media.Brush brush = GetBrush(button.GetBackColor());
                Media.Pen pen = GetPen(button.BorderColor, button.Thickness);
                Windows.Rect winRect = new Windows.Rect(button.X, button.Y, button.Width, button.Height);
                Media.FormattedText formattedText = new Media.FormattedText(
                    button.GetText(),
                    System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                    Windows.FlowDirection.LeftToRight,
                    new Media.Typeface(button.FontName != null && button.FontName.Length > 0 ? button.FontName : "Verdana"),
                    button.FontSize > 0 ? button.FontSize : 12,
                    Media.Brushes.Black);
                formattedText.TextAlignment = Windows.TextAlignment.Center;

                // Snap Element 
                SnapElement(dc, button.X, button.Y);
                // Draw Rectangle
                if (button.Round > 0) { dc.DrawRoundedRectangle(brush, pen, winRect, button.Round, button.Round); }
                else { dc.DrawRectangle(brush, pen, winRect); }
                dc.DrawText(formattedText, new System.Windows.Point(button.X + button.Width / 2, button.Y + (button.Height / 2 - formattedText.Height / 2)));
            }
            visual.GElement = button;
            AddVisual(visual);
        }

        public override void DrawBar(Bar bar)
        {
            GVisual visual = new GVisual();
            using (Media.DrawingContext dc = visual.RenderOpen())
            {
                Media.Brush backBrush = GetBrush(bar.BackColor);
                Media.Brush barBrush = GetBrush(bar.BarColor);
                Media.Pen backPen = GetPen(bar.BorderColor, bar.Thickness);
                Media.Pen barPen = GetPen(bar.BarColor, bar.Thickness);

                // Snap Element 
                SnapElement(dc, bar.X, bar.Y);
                dc.DrawRectangle(backBrush, backPen, new Windows.Rect(bar.X, bar.Y, bar.Width, bar.Height));
                if (bar.Orientation == BarOrientation.Vertical)
                {
                    Int32 height = (bar.Height - 2) * bar.Value / (bar.MaxValue - bar.MinValue);
                    dc.DrawRectangle(barBrush, barPen, new Windows.Rect(bar.X + 1, bar.Y - 1 + bar.Height - height, bar.Width - 2, height));
                }
                else if (bar.Orientation == BarOrientation.Horizontal)
                {
                    Int32 width = (bar.Width - 2) * bar.Value / (bar.MaxValue - bar.MinValue);
                    dc.DrawRectangle(barBrush, barPen, new Windows.Rect(bar.X + 1, bar.Y + 1, width, bar.Height - 2));
                }
            }
            visual.GElement = bar;
            AddVisual(visual);
        }

        public void StartPeriodicRedraw()
        {
            redrawTimer.Interval = new TimeSpan(0, 0, 0, 0, Model.Properties.RedrawTime);
            redrawTimer.Start();
        }

        public void StopPeriodicRedraw()
        {
            redrawTimer.Stop();
        }

        private void DispatcherTimer_Tick(Object sender, EventArgs e)
        {
            DrawCurrentWindow();
            Console.WriteLine("REDRAW OK!");
        }

        protected void OnMouseLBDown(object sender, MouseButtonEventArgs e)
        {
            // Retrieve the mouse pointer location relative to the Window
            Windows.Point location = e.GetPosition(this);
            // Perform visual hit testing
            Media.HitTestResult result = Media.VisualTreeHelper.HitTest(this, location);

            // If we hit Button
            if (result.VisualHit.GetType() == typeof(GVisual)
                && ((GVisual)result.VisualHit).GElement != null
                && ((GVisual)result.VisualHit).GElement is Button)
            {
                ////List<ArchiveItem> allItems = Model.GetInstance().ChArchive.ReadAll();
                ////List<ArchiveItem> ch1Items = Model.GetInstance().ChArchive.Read(1);
                ////List<ArchiveItem> ch2Items = Model.GetInstance().ChArchive.Read(2);

                selectedControl = (Button)((GVisual)result.VisualHit).GElement;
                Button button = (Button)selectedControl;
                switch (button.ButtonType)
                {
                    case ButtonType.ChangeWindow:
                        ViewController.ChangeWindow(button.ChangedWindowID);
                        return;
                    default:
                        button.ButtonPress();
                        RedrawElement(button);
                        break;
                }
                isMouseLBPressed = true;
            }
        }

        protected void OnMouseLBUp(object sender, MouseButtonEventArgs e)
        {
            // Retrieve the mouse pointer location relative to the Window
            Windows.Point location = e.GetPosition(this);
            // Perform visual hit testing
            Media.HitTestResult result = Media.VisualTreeHelper.HitTest(this, location);

            // If Selected Control is Button
            if (selectedControl != null && selectedControl.CType == ControlType.Button)
            {
                Button button = (Button)selectedControl;
                switch (button.ButtonType)
                {
                    case ButtonType.ChangeWindow:
                        break;
                    default:
                        button.ButtonRelease();
                        RedrawElement(button);
                        break;
                }
            }
            selectedControl = null;
            this.ReleaseMouseCapture();
            isMouseLBPressed = false;
        }

        protected void MyOnMouseMove(object sender, MouseEventArgs e) { }
    }
}
