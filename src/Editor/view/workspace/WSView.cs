using System;
using System.Collections.Generic;

using Windows = System.Windows;
using Media = System.Windows.Media;

using Core.model;
using Core.model.design.graphics;
using Core.model.design.graphics.control;
using Core.model.design.window;
using Core.model.design.graphics.shape;
using System.Windows.Input;
using Core.service.graphics;

namespace Editor.view.workspace
{
    public abstract class WSView : Windows.Controls.Canvas
    {
        protected static Double HALF_PIX = 0.5D;

        public Model Model { get; protected set; }
        public WSController Controller { get; protected set; }
        public Window CurrentWindow { get; set; }

        // Visual Collection
        protected IList<GVisual> visualCollection = new List<GVisual>();
        protected IDictionary<Int32, GVisual> elementDictionary = new Dictionary<Int32, GVisual>();

        // Picture Storage
        protected IDictionary<String, Media.Imaging.BitmapImage> pictureStorage = new Dictionary<String, Media.Imaging.BitmapImage>();


        public void DrawCurrentWindow()
        {
            DrawWindow(CurrentWindow);
        }

        public abstract void DrawWindow(Window window);

        public void DrawElement(GElement element)
        {
            if (element != null /*&& element.IsVisible*/)
            {
                if (element is Shape)
                {
                    Shape shape = (Shape)element;
                    switch (shape.SType)
                    {
                        case ShapeType.Line:
                            DrawLine((Line)shape);
                            break;
                        case ShapeType.Path:
                            DrawPath((Path)shape);
                            break;
                        case ShapeType.Rectangle:
                            DrawRectangle((Rectangle)shape);
                            break;
                        case ShapeType.Circle:
                            DrawCircle((Circle)shape);
                            break;
                        case ShapeType.Polygon:
                            DrawPolygon((Polygon)shape);
                            break;
                        case ShapeType.Text:
                            DrawText((Text)shape);
                            break;
                        case ShapeType.Picture:
                            DrawPicture((Picture)shape);
                            break;
                        default:
                            break;
                    }
                }
                else if (element is Control)
                {
                    Control control = (Control)element;
                    switch (control.CType)
                    {
                        case ControlType.Button:
                            DrawButton((Button)control);
                            break;
                        case ControlType.NField:
                            DrawNField((NField)control);
                            break;
                        case ControlType.Bar:
                            DrawBar((Bar)control);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public abstract void DrawLine(Line line);

        public abstract void DrawPath(Path path);

        public abstract void DrawRectangle(Rectangle rectangle);

        public abstract void DrawCircle(Circle circle);

        public abstract void DrawPolygon(Polygon polygon);

        public abstract void DrawText(Text text);

        public abstract void DrawPicture(Picture picture);

        public abstract void DrawButton(Button button);

        public abstract void DrawBar(Bar bar);

        protected void DrawNField(NField field)
        {
            GVisual visual = new GVisual();
            using (Media.DrawingContext dc = visual.RenderOpen())
            {
                Media.Brush brush = GetBrush(field.BackColor);
                Media.Pen pen = GetPen(field.BorderColor, field.Thickness);
                Media.FormattedText formattedText = new Media.FormattedText(
                    field.Value,
                    System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                    Windows.FlowDirection.LeftToRight,
                    new Media.Typeface(field.FontName != null && field.FontName.Length > 0 ? field.FontName : "Verdana"),
                    field.FontSize > 0 ? field.FontSize : 18,
                    Media.Brushes.Black);

                // Snap Element 
                SnapElement(dc, field.X, field.Y);
                dc.DrawRectangle(brush, pen, new Windows.Rect(field.X, field.Y, field.Width, field.Height));
                int y = field.Y;

                // VAlignment Apply
                switch (field.VAlignment)
                {
                    case VAlignment.Top:
                        break;
                    case VAlignment.Center:
                        y = y + (Int32)(field.Height / 2 - formattedText.Height / 2);
                        break;
                    case VAlignment.Bottom:
                        y = y + (Int32)(field.Height - formattedText.Height);
                        break;
                }

                // HAlignment Apply
                switch (field.HAlignment)
                {
                    case HAlignment.Left:
                        formattedText.TextAlignment = Windows.TextAlignment.Left;
                        dc.DrawText(formattedText, new System.Windows.Point(field.X + 3, y));
                        break;
                    case HAlignment.Center:
                        formattedText.TextAlignment = Windows.TextAlignment.Center;
                        dc.DrawText(formattedText, new System.Windows.Point(field.X + field.Width / 2, y));
                        break;
                    case HAlignment.Right:
                        formattedText.TextAlignment = Windows.TextAlignment.Right;
                        dc.DrawText(formattedText, new System.Windows.Point(field.X + field.Width - 3, y));
                        break;
                }
            }

            visual.GElement = field;
            AddVisual(visual);
        }

        public void RedrawElement(GElement element)
        {
            if (element != null && elementDictionary.ContainsKey(element.ID))
            {
                // Get Index of current element
                GVisual visual = elementDictionary[element.ID];
                Int32 index = visualCollection.IndexOf(visual);

                if (index >= 0)
                {
                    RemoveVisual(visual);
                    DrawElement(element);

                    // Remove last created Element
                    GVisual newVisual = visualCollection[visualCollection.Count - 1];
                    visualCollection.Remove(newVisual);
                    // Insert element to prev index
                    visualCollection.Insert(index, newVisual);
                }
            }
        }

        protected Media.Brush GetBrush(Media.Color color)
        {
            Media.Brush brush = new Media.SolidColorBrush(color);
            return brush;
        }

        protected Media.Pen GetPen(Media.Color color, Int32 thickness)
        {
            var penFill = new Media.SolidColorBrush(color);
            Media.Pen pen = new Media.Pen(penFill, thickness);
            return pen;
        }

        public void Clear()
        {
            foreach (Media.Visual visual in visualCollection)
            {
                this.RemoveVisualChild(visual);
                this.RemoveLogicalChild(visual);
            }
            visualCollection.Clear();
            elementDictionary.Clear();
        }

        protected void AddVisual(GVisual visual)
        {
            visualCollection.Add(visual);
            AddVisualChild(visual);
            AddLogicalChild(visual);

            if (visual.GElement != null)
            {
                elementDictionary[visual.GElement.ID] = visual;
            }
        }

        protected void RemoveVisual(GVisual visual)
        {
            if (visual != null && visualCollection.Contains(visual))
            {
                visualCollection.Remove(visual);
                this.RemoveVisualChild(visual);
                this.RemoveLogicalChild(visual);

                if (visual.GElement != null)
                {
                    elementDictionary.Remove(visual.GElement.ID);
                }
            }
        }

        //-!!!!-//
        // The two necessary overrides, implemented for the single Visual
        protected override Int32 VisualChildrenCount
        {
            get { return visualCollection.Count; }
        }

        //-!!!!-//
        protected override Media.Visual GetVisualChild(Int32 index)
        {
            if (index < 0 || index >= visualCollection.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return visualCollection[index];
        }


        protected Random random = new Random();

        public void PrintRandom()
        {
            Clear();
            for (Int32 i = 0; i < 10000; i++)
            {
                Int32 width = random.Next(CurrentWindow != null ? CurrentWindow.Width : 640);
                Int32 height = random.Next(CurrentWindow != null ? CurrentWindow.Height : 480);

                Rectangle rectangle = new Rectangle(width - 20, height - 20, 40, 40);
                rectangle.BackColorOFF = Media.Colors.Orange;
                rectangle.BorderColorOFF = Media.Colors.Black;
                rectangle.ThicknessOFF = 1;
                rectangle.ID = 2 * i;
                DrawElement(rectangle);
            }
        }

        protected void SnapElement(Media.DrawingContext dc, Int32 x, Int32 y)
        {
            var snapX = new double[] { x + HALF_PIX };
            var snapY = new double[] { y + HALF_PIX };
            dc.PushGuidelineSet(new Media.GuidelineSet(snapX, snapY));
        }
    }
}
