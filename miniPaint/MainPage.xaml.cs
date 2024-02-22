using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace miniPaint
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // ReSharper disable IdentifierTypo
        private bool _czyRysuje = false;
        private Point _punktStartowy;
        private SolidColorBrush _pisak = new SolidColorBrush(Windows.UI.Colors.Black);
        private Line _poprzedniaKreska;
        private Stack<Shape> _stosUndo = new Stack<Shape>();
        private Stack<int> _stosPunkty = new Stack<int>();
        private int _punktyRysowania = 0;

        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(1200, 900);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        private SolidColorBrush _pędzel;
        private void poleRysowania_PointerMoved(object sender, PointerRoutedEventArgs e)
        {

            if (_czyRysuje)
            {
                var punktAktualny = e.GetCurrentPoint(PoleRysowania).Position;
                var kreska = new Line()
                {
                    Stroke = _pisak,
                    StrokeThickness = SldGrubosc.IntermediateValue,
                    X2 = punktAktualny.X,
                    Y2 = punktAktualny.Y,
                    X1 = _punktStartowy.X,
                    Y1 = _punktStartowy.Y,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round
                };
                PoleRysowania.Children.Add(kreska);

                if (Dowolna.IsChecked == true)
                {
                    _punktStartowy = punktAktualny;
                    _punktyRysowania++;
                    _stosUndo.Push(kreska);
                }
                else
                {
                    if(_poprzedniaKreska != null)
                        PoleRysowania.Children.Remove(_poprzedniaKreska);
                    _poprzedniaKreska = kreska;
                }
            }
        }

        private void poleRysowania_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _czyRysuje = true;
            _punktStartowy = e.GetCurrentPoint(PoleRysowania).Position;
        }

        private void poleRysowania_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _czyRysuje = false;
            if (Prosta.IsChecked == true)
            {
                _stosPunkty.Push(0);
                _stosUndo.Push(_poprzedniaKreska);
            }
            else
                _stosPunkty.Push(_punktyRysowania);

            _punktyRysowania = 0;
            _poprzedniaKreska = null;
            

        }

        private void kolorCzerwony(object sender, PointerRoutedEventArgs e)
        {
            _pisak = new SolidColorBrush(Windows.UI.Colors.Red);
        }
        private void kolorZielony(object sender, PointerRoutedEventArgs e)
        {
            _pisak = new SolidColorBrush(Windows.UI.Colors.Green);
        }
        private void kolorNiebieski(object sender, PointerRoutedEventArgs e)
        {
            _pisak = new SolidColorBrush(Windows.UI.Colors.Blue);
        }

        private void BlackButton(object sender, PointerRoutedEventArgs e)
        {
            _pisak = new SolidColorBrush(Windows.UI.Colors.Black);
        }

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (_stosUndo.Count > 0)
            {
                var drawPoints = _stosPunkty.Pop();
                if (drawPoints == 0)
                {
                    var undo = _stosUndo.Pop();
                    PoleRysowania.Children.Remove(undo);
                }
                else
                {
                    for (var i = 0; i < drawPoints; i++)
                    {
                        var undo = _stosUndo.Pop();
                        PoleRysowania.Children.Remove(undo);
                    }
                }
            }
        }

        private async void BtnWyjscie_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Czy chcesz wyjść?");
            var tak = new UICommand("Tak", c => Application.Current.Exit());
            var no = new UICommand("Nie");

            dialog.Commands.Add(tak);
            dialog.Commands.Add(no);
            dialog.DefaultCommandIndex = 0;

            await dialog.ShowAsync();
        }
    }
}
