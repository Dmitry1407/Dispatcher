using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

using Windows = System.Windows;
using Media = System.Windows.Media;

using Core.model;
using Core.model.core.channel;
using Core.model.design.graphics.shape;
using Core.model.design.graphics.control;

using Editor.view;
using Editor.view.projectpanel;
using Editor.view.propertiespanel;
using Editor.view.statusbar;
using Editor.view.workspace;
using Core.model.core.source;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewController viewController;
        private Model model;

        public MainWindow()
        {
            // Initialize Component
            InitializeComponent();

            // Initialize View
            viewController = new ViewController();
            model = viewController.Model;

            // Initialize Panels
            WSPanel.Content = viewController.CurrentView;
            PropertiesPanel.Content = viewController.PropertiesPanel;
            ProjectPanel.Content = viewController.ProjectPanel;

            // Init Panels
            PanelSB.Content = viewController.SBPanel;

            UpdateTitle("???");
            editModeButton.IsEnabled = false;
            editModeMenuItem.IsEnabled = false;
        }

        private void UpdateTitle(String fName)
        {
            this.Title = "Editor [" + fName + "]";
        }

        private void CNewButton(Object sender, RoutedEventArgs e) { }

        private void COpenButton(Object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select Project file...",
                DefaultExt = ".proj",
                Filter = "PROJ file (.proj)|*.proj",
                CheckFileExists = true
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    String fileName = dlg.FileName;

                    // Clear old storages and panels
                    viewController.CurrentView.Clear();
                    model.Clear();
                    viewController.ProjectPanel.Clear();

                    // Load new model
                    model.LoadProject(fileName);
                    UpdateTitle(dlg.FileName);
                    viewController.UpdateProjectPanel();

                    // Get first Window
                    var enumerator = model.WindowStorage.GetEnumerator();
                    enumerator.MoveNext();
                    var anElement = enumerator.Current;

                    viewController.CurrentWindow = anElement.Value;
                    viewController.CurrentView.CurrentWindow = viewController.CurrentWindow;
                    viewController.CurrentView.DrawCurrentWindow();
                    viewController.CurrentView.Focus();
                    ////WorkSpacePanel.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occured: " + ex.Message));
                }
            }
        }

        private void CSaveButton(Object sender, RoutedEventArgs e)
        {
            if (!model.SaveProject())
            {
                CSaveAsButton(sender, e);
            }
        }

        private void CSaveAsButton(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save Project as...",
                DefaultExt = ".proj",
                Filter = "PROJ file (.proj)|*.proj",
                CheckFileExists = true
            };

            if (dlg.ShowDialog() == true)
            {
                String fileName = dlg.FileName;
                model.SaveProject(fileName);
                UpdateTitle(dlg.FileName);
            }
        }

        private void CCloseButton(Object sender, RoutedEventArgs e)
        {
            viewController.CurrentView.Clear();
            model.Clear();
            viewController.ProjectPanel.Clear();
            UpdateTitle("???");
            viewController.CurrentView.DrawCurrentWindow();
        }

        private void CExitButton(Object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CCutButton(object sender, RoutedEventArgs e) { }

        private void CCopyButton(Object sender, RoutedEventArgs e) { }

        private void CPasteButton(Object sender, RoutedEventArgs e) { }

        private void CDeleteButton(object sender, RoutedEventArgs e) { }

        private void CAddModbusButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewSource(SourceType.Modbus);
        }

        private void CAddOPCButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewSource(SourceType.OPC);
        }

        private void CAddSGButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewSource(SourceType.SG);
        }

        private void CAddMBDeviceButton(object sender, RoutedEventArgs e) { }

        private void CAddBitChannelButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewChannel(ChannelType.Bit);
        }

        private void CAddByteChannelButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewChannel(ChannelType.Byte);
        }

        private void CAddInt16ChannelButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewChannel(ChannelType.Int16);
        }

        private void CAddUInt16ChannelButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewChannel(ChannelType.UInt16);
        }

        private void CAddInt32ChannelButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewChannel(ChannelType.Int32);
        }

        private void CAddUInt32ChannelButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewChannel(ChannelType.UInt32);
        }

        private void CAddFloatChannelButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewChannel(ChannelType.Float);
        }

        private void CAddDoubleChannelButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewChannel(ChannelType.Double);
        }


        private void CAddWindowButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewWindow();
            viewController.CurrentView.Clear();
            viewController.CurrentView.DrawCurrentWindow();
        }

        private void CAddLineButton(Object sender, RoutedEventArgs e)
        {
            viewController.AddNewShape(ShapeType.Line);
            viewController.CurrentView.DrawCurrentWindow();
        }

        private void CAddPathButton(Object sender, RoutedEventArgs e)
        {
            viewController.AddNewShape(ShapeType.Path);
            viewController.CurrentView.DrawCurrentWindow();
        }

        private void CAddCircleButton(Object sender, RoutedEventArgs e)
        {
            viewController.AddNewShape(ShapeType.Circle);
            viewController.CurrentView.DrawCurrentWindow();
        }

        private void CAddRectangleButton(Object sender, RoutedEventArgs e)
        {
            viewController.AddNewShape(ShapeType.Rectangle);
            viewController.CurrentView.DrawCurrentWindow();
        }

        private void CAddPolygonButton(Object sender, RoutedEventArgs e)
        {
            viewController.AddNewShape(ShapeType.Polygon);
            viewController.CurrentView.DrawCurrentWindow();
        }

        private void CAddTextButton(Object sender, RoutedEventArgs e)
        {
            viewController.AddNewShape(ShapeType.Text);
            viewController.CurrentView.DrawCurrentWindow();
        }

        private void CAddPictureButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewShape(ShapeType.Picture);
            viewController.CurrentView.DrawCurrentWindow();
        }

        private void CAddButtonButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewControl(ControlType.Button);
            viewController.CurrentView.DrawCurrentWindow();
        }

        private void CAddBarButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewControl(ControlType.Bar);
            viewController.CurrentView.DrawCurrentWindow();
        }

        private void CAddNFieldButton(object sender, RoutedEventArgs e)
        {
            viewController.AddNewControl(ControlType.NField);
            viewController.CurrentView.DrawCurrentWindow();
        }


        private void CEditModeButton(Object sender, RoutedEventArgs e)
        {
            viewController.SetEditMode();
            WSPanel.Content = viewController.CurrentView;

            editModeButton.IsEnabled = false;
            editModeMenuItem.IsEnabled = false;
            runModeButton.IsEnabled = true;
            runModeMenuItem.IsEnabled = true;
        }

        private void CRunModeButton(Object sender, RoutedEventArgs e)
        {
            viewController.SetRunMode();
            WSPanel.Content = viewController.CurrentView;

            editModeButton.IsEnabled = true;
            editModeMenuItem.IsEnabled = true;
            runModeButton.IsEnabled = false;
            runModeMenuItem.IsEnabled = false;
        }

        private void CRandomButton(Object sender, RoutedEventArgs e)
        {
            viewController.CurrentView.PrintRandom();
        }

        private void CHelpButton(object sender, RoutedEventArgs e) { }
    }
}
