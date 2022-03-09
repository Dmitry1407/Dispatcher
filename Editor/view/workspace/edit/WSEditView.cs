using System;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

using Windows = System.Windows;
using Media = System.Windows.Media;

using Core.model;
using Core.model.design.graphics.shape;
using Core.model.design.window;
using Core.model.design.graphics;
using Core.model.design.graphics.control;
using Core.service.graphics;

namespace Editor.view.workspace.edit
{
    public class WSEditView : WSView
    {
        public static readonly Int32 MIN_GRID_SNAP = 5;

        public ViewController ViewController { get; set; }

        private Boolean isMouseLBPressed = false;
        private Boolean isStartPoint = true;
        private Boolean isSnapPoint = true;
        private GVisual focusedVisual;
        private GVisual selectedVisual;
        private GVisual draggedVisual;

        private GVisual focusedElementBorders;
        private GVisual selectedElementBorders;

        private Windows.Point? prevMousePosition;
        private Windows.Point? startMousePosition;

        public WSEditView() : this(Model.GetInstance()) { }

        public WSEditView(Model model)
        {
            Model = model;
            Controller = new WSEditController(model);
            ViewController = null;

            this.ClipToBounds = true;
            this.Background = Media.Brushes.Transparent;

            // Mouse events
            this.MouseLeftButtonDown += OnMouseLBDown;
            this.MouseLeftButtonUp += OnMouseLBUp;
            this.MouseMove += MyOnMouseMove;
            this.KeyDown += keyDown;

            Boolean bFocus = this.Focus();
            this.Focusable = true;

            DrawCurrentWindow();
        }


        public override void DrawWindow(Window window)
        {
            if (window != null)
            {
                this.Background = new Media.SolidColorBrush(window.BackColor);
                this.Width = window.Width;
                this.Height = window.Height;

                // Draw GrigDots
                if (visualCollection.Count == 0)
                {   // GrigDots is first visual
                    DrawGrigDots();
                }

                // Draw elements
                foreach (GElement element in window.ElementStorage.Values)
                {
                    if (element != null && !elementDictionary.ContainsKey(element.ID))
                    {
                        DrawElement(element);
                    }
                }
            }
        }

        private void DrawGrigDots()
        {
            if (Model.Properties.IsGridDots)
            {
                Brush brush = Brushes.Azure;
                Media.Pen pen = GetPen(Colors.Black, 1);
                pen.DashStyle = new DashStyle(new Double[] { 0D, (Double)Model.Properties.GridSize }, 1D);

                GVisual visual = new GVisual();
                using (Media.DrawingContext dc = visual.RenderOpen())
                {
                    SnapElement(dc, 0, 0);
                    for (Int32 i = Model.Properties.GridSize; i < (this.Height - 1); i += Model.Properties.GridSize)
                    {
                        dc.DrawLine(pen, new Windows.Point(0D, (Double)i), new Windows.Point(this.Width - 2, (Double)i));
                    }
                }
                AddVisual(visual);
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
                Media.Pen pen = GetPen(line.BackColor, line.Thickness);
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
                Media.Pen pen = GetPen(path.BackColor, path.Thickness);

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
                Media.Brush brush = GetBrush(rectangle.BackColor);
                Media.Pen pen = GetPen(rectangle.BorderColor, rectangle.Thickness);
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
                Media.Brush brush = GetBrush(circle.BackColor);
                Media.Pen pen = GetPen(circle.BorderColor, circle.Thickness);
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
                Media.Brush brush = GetBrush(polygon.BackColor);
                Media.Pen pen = GetPen(polygon.BorderColor, polygon.Thickness);

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

        public override void DrawButton(Button button)
        {
            GVisual visual = new GVisual();
            using (Media.DrawingContext dc = visual.RenderOpen())
            {
                Media.Brush brush = GetBrush(button.BackColorOFF);
                Media.Pen pen = GetPen(button.BorderColor, button.Thickness);
                Windows.Rect winRect = new Windows.Rect(button.X, button.Y, button.Width, button.Height);
                Media.FormattedText formattedText = new Media.FormattedText(
                    button.TextOFF,
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
                Media.Brush brush = GetBrush(bar.BackColor);
                Media.Pen pen = GetPen(bar.BorderColor, bar.Thickness);

                // Snap Element 
                SnapElement(dc, bar.X, bar.Y);
                dc.DrawRectangle(brush, pen, new Windows.Rect(bar.X, bar.Y, bar.Width, bar.Height));
            }
            visual.GElement = bar;
            AddVisual(visual);
        }

        protected void OnMouseLBDown(object sender, MouseButtonEventArgs e)
        {
            // Retrieve the mouse pointer location relative to the Window
            Windows.Point curPosition = e.GetPosition(this);
            // Perform visual hit testing
            Media.HitTestResult result = Media.VisualTreeHelper.HitTest(this, curPosition);

            // If we hit any DrawingVisual
            if (result.VisualHit.GetType() == typeof(GVisual))
            {
                // If hit to new GElement
                if (selectedVisual == null)
                {
                    selectedVisual = result.VisualHit as GVisual;
                    DrawSelectedBorders(selectedVisual);
                }
                // If hit to other GElement
                else if (selectedVisual != result.VisualHit)
                {
                    RemoveVisual(selectedElementBorders);
                    selectedVisual = result.VisualHit as GVisual;
                    DrawSelectedBorders(selectedVisual);
                }

                draggedVisual = selectedVisual;
                startMousePosition = curPosition;
                prevMousePosition = curPosition;
                isStartPoint = true;

                if (selectedVisual != null && selectedVisual.GElement != null)
                {
                    ViewController.PropertiesPanel.PropertyGrid.SelectedObject = selectedVisual.GElement;
                    ViewController.SBPanel.SetElementPosition((Int32)selectedVisual.GElement.X, (Int32)selectedVisual.GElement.Y);
                }
                this.CaptureMouse();
            }
            else
            {
                if (selectedVisual != null)
                {
                    RemoveVisual(selectedElementBorders);
                    selectedVisual = null;
                }
                ViewController.PropertiesPanel.PropertyGrid.SelectedObject = null;
            }
            isMouseLBPressed = true;
        }

        protected void OnMouseLBUp(object sender, MouseButtonEventArgs e)
        {
            draggedVisual = null;
            prevMousePosition = null;
            startMousePosition = null;

            // selectedElement.ReleaseMouseCapture();
            this.ReleaseMouseCapture();
            isMouseLBPressed = false;
            isStartPoint = false;

            // Set focus to canvas
            Boolean bFocus = this.Focus();
            this.Focusable = true;
        }

        protected void MyOnMouseMove(object sender, MouseEventArgs e)
        {
            Windows.Point curPosition = e.GetPosition(this);
            ViewController.SBPanel.SetMousePosition((Int32)curPosition.X, (Int32)curPosition.Y);

            if (!isMouseLBPressed)
            {
                Media.HitTestResult result = Media.VisualTreeHelper.HitTest(this, curPosition);

                // If hit to GElement
                if (result.VisualHit.GetType() == typeof(GVisual) && ((GVisual)result.VisualHit).GElement != null)
                {
                    // If hit to new GElement
                    if (focusedVisual == null)
                    {
                        focusedVisual = result.VisualHit as GVisual;
                        DrawFocusedBorders(focusedVisual);
                    }
                    // If hit to other GElement
                    else if (focusedVisual != result.VisualHit)
                    {
                        RemoveVisual(focusedElementBorders);
                        focusedVisual = result.VisualHit as GVisual;
                        DrawFocusedBorders(focusedVisual);
                    }
                }
                else if (focusedVisual != null)
                {
                    RemoveVisual(focusedElementBorders);
                    focusedVisual = null;
                }
            }

            if (prevMousePosition == null) { return; }

            // Drag GElement
            if (draggedVisual != null && isMouseLBPressed)
            {
                // Move GElement
                Windows.Point shift = curPosition - (Windows.Vector)prevMousePosition.Value;

                // Snap to grid
                if (Model.Properties.SnapToGrid)
                {
                    double halfSnap = Model.Properties.GridSize / 2;
                    if (Math.Abs(shift.X) > halfSnap || Math.Abs(shift.Y) > halfSnap)
                    {
                        // Elements new coordinates
                        Int32 elementX = draggedVisual.GElement.X + (Int32)shift.X;
                        Int32 elementY = draggedVisual.GElement.Y + (Int32)shift.Y;
                        // Elements snap delta
                        Int32 deltaX = elementX % Model.Properties.GridSize;
                        Int32 deltaY = elementY % Model.Properties.GridSize;
                        // Shift + delta
                        shift.X = deltaX > halfSnap ? shift.X + Model.Properties.GridSize - deltaX : shift.X - deltaX;
                        shift.Y = deltaY > halfSnap ? shift.Y + Model.Properties.GridSize - deltaY : shift.Y - deltaY;

                        MoveGElement(draggedVisual, shift);
                        prevMousePosition = new Windows.Point(prevMousePosition.Value.X + shift.X, prevMousePosition.Value.Y + shift.Y);
                    }
                }

                // No Snap to grid
                else
                {
                    Windows.Point totalShift = curPosition - (Windows.Vector)startMousePosition.Value;
                    if (Math.Abs(totalShift.X) > (MIN_GRID_SNAP - 0.1D) || Math.Abs(totalShift.Y) > (MIN_GRID_SNAP - 0.1D))
                    {
                        // Move to new position
                        shift = isStartPoint ? totalShift : curPosition - (Windows.Vector)prevMousePosition.Value;
                        isStartPoint = false;
                    }
                    else if (!isStartPoint)
                    {
                        // Return to start position
                        shift = startMousePosition.Value - (Windows.Vector)prevMousePosition;
                        isStartPoint = true;
                    }
                    MoveGElement(draggedVisual, shift);
                    prevMousePosition = curPosition;
                }
            }
        }

        private void keyDown(Object sender, KeyEventArgs e)
        {
            Windows.Point shift = new Windows.Point(0, 0);

            // Flags for key combinations
            Boolean ctrlShiftKey = false;
            Boolean anyCtrlShiftKey = false;
            Boolean shiftKey = false;
            Boolean ctrlKey = false;
            Boolean anyCtrlKey = false;

            // SHIFT + KEY
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                shiftKey = true;
                switch (e.Key)
                {
                    case Key.Right:
                        shift.X = 10;
                        MoveGElement(selectedVisual, shift);
                        break;
                    case Key.Left:
                        shift.X = -10;
                        MoveGElement(selectedVisual, shift);
                        break;
                    case Key.Up:
                        shift.Y = -10;
                        MoveGElement(selectedVisual, shift);
                        break;
                    case Key.Down:
                        shift.Y = 10;
                        MoveGElement(selectedVisual, shift);
                        break;
                    default:
                        shiftKey = false;
                        break;
                }
            }

            // CTRL + KEY
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                ctrlKey = true;
                switch (e.Key)
                {
                    case Key.A:
                        ////selectAll();
                        break;
                    case Key.X:
                        ////cutToSystemClipboard();
                        break;
                    case Key.C:
                        ////copyToSystemClipboard();
                        break;
                    case Key.V:
                        ////pasteFromSystemClipboard();
                        break;
                    case Key.Z:
                        ////undo();
                        break;
                    default:
                        ctrlKey = false;
                        break;
                }
            }

            // KEY
            if (!anyCtrlShiftKey && !ctrlShiftKey
                && !shiftKey && !anyCtrlKey && !ctrlKey)
            {
                switch (e.Key)
                {
                    case Key.Right:
                        shift.X = 1;
                        MoveGElement(selectedVisual, shift);
                        break;
                    case Key.Left:
                        shift.X = -1;
                        MoveGElement(selectedVisual, shift);
                        break;
                    case Key.Up:
                        shift.Y = -1;
                        MoveGElement(selectedVisual, shift);
                        break;
                    case Key.Down:
                        shift.Y = 1;
                        MoveGElement(selectedVisual, shift);
                        break;

                    case Key.Delete:
                    case Key.Back:
                        ////deleteElement();
                        break;

                    // ESCAPE
                    case Key.Escape:
                        if (focusedVisual != null)
                        {
                            RemoveVisual(focusedElementBorders);
                            focusedVisual = null;
                        }
                        if (selectedVisual != null)
                        {
                            RemoveVisual(selectedElementBorders);
                            selectedVisual = null;
                        }
                        ViewController.PropertiesPanel.PropertyGrid.SelectedObject = null;
                        break;

                    // OTHER_KEY
                    default:
                        break;
                }
            }
            e.Handled = true;
        }

        private void MoveGElement(GVisual element, Windows.Point shift)
        {
            if (element != null)
            {
                // Move element
                if (element.Transform == null) { element.Transform = new Media.TranslateTransform(); }
                (element.Transform as Media.TranslateTransform).X += shift.X;
                (element.Transform as Media.TranslateTransform).Y += shift.Y;

                // Move Borders
                if (element != focusedElementBorders && element != selectedElementBorders)
                {
                    MoveGElement(focusedElementBorders, shift);
                    MoveGElement(selectedElementBorders, shift);
                }

                // Change element coordinates
                if (selectedVisual != null && selectedVisual.GElement != null && element == selectedVisual)
                {
                    selectedVisual.GElement.X += (Int32)shift.X;
                    selectedVisual.GElement.Y += (Int32)shift.Y;
                    ViewController.PropertiesPanel.PropertyGrid.Refresh();
                    ViewController.SBPanel.SetElementPosition((Int32)selectedVisual.GElement.X, (Int32)selectedVisual.GElement.Y);

                    if (selectedVisual.GElement.Parent != null)
                    {
                        selectedVisual.GElement.Parent.X = selectedVisual.GElement.X;
                        selectedVisual.GElement.Parent.Y = selectedVisual.GElement.Y;
                    }
                }
            }
        }

        private void DrawFocusedBorders(GVisual fElement)
        {
            if (fElement != null && fElement.GElement != null)
            {
                focusedElementBorders = new GVisual();
                DrawElementBorders(fElement, focusedElementBorders);
            }
        }

        private void DrawSelectedBorders(GVisual fElement)
        {
            if (fElement != null && fElement.GElement != null)
            {
                selectedElementBorders = new GVisual();
                DrawElementBorders(fElement, selectedElementBorders);
            }
        }

        private void DrawElementBorders(GVisual fElement, GVisual borders)
        {
            using (Media.DrawingContext dc = borders.RenderOpen())
            {
                GElement gElement = fElement.GElement;
                Int32 x = gElement.X - 3;
                Int32 y = gElement.Y - 3;
                Int32 height = (Int32)fElement.ContentBounds.Height + 5;
                Int32 width = (Int32)fElement.ContentBounds.Width + 5;

                Media.Pen pen = new Media.Pen(Media.Brushes.DarkRed, 1);
                if (borders == focusedElementBorders)
                {
                    pen.DashStyle = new DashStyle(new Double[] { 3D, 3D }, 0D);
                }

                Windows.Point pointLT = new Windows.Point(x, y);
                Windows.Point pointRT = new Windows.Point(x + width, y);
                Windows.Point pointRB = new Windows.Point(x + width, y + height);
                Windows.Point pointLB = new Windows.Point(x, y + height);

                if (gElement is Shape && ((Shape)gElement).SType == ShapeType.Line)
                {
                    Line line = (Line)gElement;
                    Int32 startX = Math.Min(line.X, line.X + line.DX) - 5;
                    Int32 endX = Math.Max(line.X, line.X + line.DX) + 5;
                    Int32 startY = Math.Min(line.Y, line.Y + line.DY) - 5;
                    Int32 endY = Math.Max(line.Y, line.Y + line.DY) + 5;

                    pointLT = new Windows.Point(startX, startY);
                    pointRT = new Windows.Point(endX, startY);
                    pointRB = new Windows.Point(endX, endY);
                    pointLB = new Windows.Point(startX, endY);
                }
                else if (gElement is Shape && ((Shape)gElement).SType == ShapeType.Text)
                {
                    x = gElement.X - 2;
                    y = gElement.Y - 2;
                    width = (Int32)fElement.ContentBounds.Width + 4;
                    height = (Int32)fElement.ContentBounds.Height + 4;

                    pointLT = new Windows.Point(x, y);
                    pointRT = new Windows.Point(x + width, y);
                    pointRB = new Windows.Point(x + width, y + height);
                    pointLB = new Windows.Point(x, y + height);
                }

                // Snap Element 
                SnapElement(dc, x, y);

                dc.DrawLine(pen, pointLT, pointRT);
                dc.DrawLine(pen, pointRT, pointRB);
                dc.DrawLine(pen, pointRB, pointLB);
                dc.DrawLine(pen, pointLB, pointLT);
            }
            AddVisual(borders);
        }
    }
}
