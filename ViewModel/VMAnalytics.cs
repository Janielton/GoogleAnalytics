using GoogleAnalitics.Classes;
using GoogleAnalitics.Model;
using Google.Analytics.Data.V1Beta;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace GoogleAnalitics.ViewModel
{
    public class VMAnalytics : INotifyPropertyChanged
    {
       
        string googleCredential = @"C:\Users\Elton\Downloads\confira-concursos-c47f81fdfc0a.json";
        AnalyticsService service;

        DateTime hoje = DateTime.Now;

        List<AnaliticsData> listAll = new List<AnaliticsData>();
        List<AnaliticsData> listPizza = new List<AnaliticsData>();

        public ObservableCollection<AnaliticsData> _listData = new ObservableCollection<AnaliticsData>();
        private bool StartVM = false;

        private int Tipo = 1;
        private int TipoFiltro = 1;

        public VMAnalytics()
        {
            service = new AnalyticsService(googleCredential);
            StartVM = true;
        }
        public VMAnalytics(int tipo)
        {
            service = new AnalyticsService(googleCredential);
            this.Tipo = tipo;
            if (Tipo == 1)
            {
                DateTime startDate = DateTime.Now.AddDays(-1);
                DataInicial = startDate.ToString("dd/MM/yyyy");
                DataFinal = hoje.ToString("dd/MM/yyyy");
                IsVisibleLoading = Visibility.Visible;
                getVisitasAll(new DateRange { StartDate = startDate.ToString("yyyy-MM-dd"), EndDate = hoje.ToString("yyyy-MM-dd") });
            }
            else if(Tipo == 2)
            {
                DateTime startDate = DateTime.Now.AddDays(-7);
                DataInicial = startDate.ToString("dd/MM/yyyy");
                DataFinal = hoje.ToString("dd/MM/yyyy");
                IsVisibleLoading = Visibility.Visible;
                getVisitasGrafico(new DateRange { StartDate = startDate.ToString("yyyy-MM-dd"), EndDate = hoje.ToString("yyyy-MM-dd") });
            }
            else if (Tipo == 3)
            {
                DateTime startDate = DateTime.Now.AddDays(-7);
                DataInicial = startDate.ToString("dd/MM/yyyy");
                DataFinal = hoje.ToString("dd/MM/yyyy");
                IsVisibleLoading = Visibility.Visible;
                getVisitasPizza(new DateRange { StartDate = startDate.ToString("yyyy-MM-dd"), EndDate = hoje.ToString("yyyy-MM-dd") });
            }
            StartVM = true;

        }

        string _dataInicial = "";
        string _dataFinal = "";
        public string DataInicial
        {
            get { return _dataInicial; }
            set
            {
                _dataInicial = value;
                OnPropertyChanged("DataInicial");
            }
        }

        public string DataFinal
        {
            get { return _dataFinal; }
            set
            {
                _dataFinal = value;
                OnPropertyChanged("DataFinal");
            }
        }

        Visibility _IsVisibleLoading = Visibility.Visible;
        public Visibility IsVisibleLoading
        {
            get { return _IsVisibleLoading; }
            set
            {
                _IsVisibleLoading = value;
                OnPropertyChanged("IsVisibleLoading");
            }
        }

        Visibility _StackAcao = Visibility.Hidden;
        public Visibility StackAcao
        {
            get { return _StackAcao; }
            set
            {
                _StackAcao = value;
                OnPropertyChanged("StackAcao");
            }
        }

        Visibility _CidadeBT = Visibility.Hidden;
        public Visibility CidadeBT
        {
            get { return _CidadeBT; }
            set
            {
                _CidadeBT = value;
                OnPropertyChanged("CidadeBT");
            }
        }

        public int _selectedIndex = -1;
        public int SelectedIndex
        {
            get
            {
                if (_selectedIndex != -1) SelectedView();
                return _selectedIndex;
            }

            set
            {
                if (_selectedIndex == value)
                {
                    return;
                }
                _selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }
        
        PlotModel _Model = null;
        public PlotModel VisitasModel
        {
            get { return _Model; }
            set
            {
                _Model = value;
                OnPropertyChanged("VisitasModel");
            }
        }

        Visibility _VisitasIsVisible = Visibility.Collapsed;
        public Visibility VisitasIsVisible
        {
            get { return _VisitasIsVisible; }
            set
            {
                _VisitasIsVisible = value;
                OnPropertyChanged("VisitasIsVisible");
            }
        }

        PlotModel _ModelPiza = null;
        public PlotModel PizzaModel
        {
            get { return _ModelPiza; }
            set
            {
                _ModelPiza = value;
                OnPropertyChanged("PizzaModel");
            }
        }

        Visibility _IsVisiblePizza = Visibility.Collapsed;
        public Visibility PizzaIsVisible
        {
            get { return _IsVisiblePizza; }
            set
            {
                _IsVisiblePizza = value;
                OnPropertyChanged("PizzaIsVisible");
            }
        }

        public ObservableCollection<LegendList> _listLegends = new ObservableCollection<LegendList>();
        public ObservableCollection<LegendList> listLegends
        {
            get { return _listLegends; }
            set
            {
                _listLegends = value;
                OnPropertyChanged("listLegends");
            }
        }

        private void SelectedView()
        {
            if (Tipo == 1)
            {
                if (TipoFiltro == 2)
                {
                    CidadeBT = Visibility.Visible;
                }
                else {
                    CidadeBT = Visibility.Collapsed;
                }
                if (StackAcao == Visibility.Hidden) StackAcao = Visibility.Visible;
            }
        }
        public ObservableCollection<AnaliticsData> listData
        {
            get { return _listData; }
            set
            {
                _listData = value;
                OnPropertyChanged("listData");
            }
        }

        #region CLIKS
        /// <summary>
        /// Paginas Vistas
        /// </summary>
        public ICommand Filtrar { get { return new DelegateCommand(_Filtrar); } }
        public ICommand Todos { get { return new DelegateCommand(_Todos); } }
        public ICommand Cidade { get { return new DelegateCommand(_Cidade); } }
        public ICommand Simulado { get { return new DelegateCommand(_Simulado); } }
        public ICommand Abrir { get { return new DelegateCommand(_Abrir); } }
        public ICommand Pesquisar { get { return new DelegateCommand(_Pesquisar); } }
        public ICommand SitePrefeitura { get { return new DelegateCommand(_SitePrefeitura); } }

       
        private void _Filtrar()
        {
            if (Tipo == 1)
            {              
                _Todos();
            }else if(Tipo == 2)
            {
                var data = valideData();
                if (data != null)
                {
                    IsVisibleLoading = Visibility.Visible;
                    VisitasIsVisible = Visibility.Collapsed;
                    getVisitasGrafico(data);
                }
            }
            else
            {
                var data = valideData();
                if (data != null)
                {
                    IsVisibleLoading = Visibility.Visible;
                    PizzaIsVisible = Visibility.Collapsed;
                    getVisitasPizza(data);
                }
            }

        }

        private void _Todos()
        {
            if (TipoFiltro == 1) return;
            var data = valideData();
            if (data != null)
            {
                IsVisibleLoading = Visibility.Visible;
                getVisitasAll(data);
            }
            TipoFiltro = 1;
        }

        private void _Cidade()
        {
            if (TipoFiltro == 2) return;
            getVisitasCidade();
            TipoFiltro = 2;
        }

        private void _Simulado()
        {
            if (TipoFiltro == 3) return;
            getVisitasSimulado();
            TipoFiltro = 3;
        }

        private void _Abrir()
        {
            string url = listData[SelectedIndex].dimension;
            if (!url.Contains("https")) url = "https://" + url;
            try
            {
               
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"_Abrir: {url} - {ex.Message}");
            }
       
        }

        private void _Pesquisar()
        {
            string url = listData[SelectedIndex].dimension;
            string cidade = url.Replace("https://", "").Replace("www.confiraconcursos.com.br/cidade/", "");
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"https://www.google.com/search?q={cidade}",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"_Pesquisar:{cidade} - {ex.Message}");
            }
        }

        private void _SitePrefeitura()
        {
            string url = listData[SelectedIndex].dimension;
            string cidade = url.Replace("https://", "").Replace("www.confiraconcursos.com.br/cidade/", "");
            try
            {
                int index = cidade.Length - 3;
                cidade = cidade.Remove(index, 1).Insert(index, ".").Replace("-","");
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"http://{cidade}.gov.br",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
{
    Debug.WriteLine($"_SitePrefeitura:{cidade} - {ex.Message}");
}
        }

        #endregion

        private void ResetSelect()
        {
            _selectedIndex = -1;
            StackAcao = Visibility.Hidden;
            CidadeBT = Visibility.Hidden;
        }

        #region ListaGet
        /// <summary>
        /// Paginas Vistas
        /// </summary>
      
        private async void getVisitasAll(DateRange data)
        {
            ResetSelect();
            var list = await service.getPaginaVista("307227049", data, "all");
            listAll = list;
            listData = new ObservableCollection<AnaliticsData>(list);
            IsVisibleLoading = Visibility.Collapsed;
        }

        private void getVisitasCidade()
        {
            ResetSelect();
            //  var list = await service.getPaginaVista("307227049", data, "/cidade/");
            var list = buscarInList(listAll, "/cidade/");
            listData = new ObservableCollection<AnaliticsData>(list);
            IsVisibleLoading = Visibility.Collapsed;
        }

        private void getVisitasSimulado()
        {
            ResetSelect();
            var list = buscarInList(listAll, "simulados-e-questoes");
            listData = new ObservableCollection<AnaliticsData>(list);
            IsVisibleLoading = Visibility.Collapsed;
        }

        private List<AnaliticsData> buscarInList(List<AnaliticsData> list, string key) {
            List<AnaliticsData> lista = new List<AnaliticsData>();
            foreach (var item in list) {
               if(item.dimension.Contains(key)) lista.Add(item);
            }
            return lista;
        }

        /// <summary>
        /// Visitas
        /// </summary>

        private async void getVisitasGrafico(DateRange data)
        {
            var pnls = new List<Pnl>();
            var list = await service.getVisitas("307227049", data);
            DateTime dt;
            foreach (AnaliticsData item in list)
            {
                if (DateTime.TryParseExact(item.dimension, "yyyyMMdd",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None, out dt))
                {
                    pnls.Add(new Pnl
                    {
                        Time = dt,
                        Value = Convert.ToDouble(item.metric)
                    });
                }
                Debug.WriteLine($"{item.dimension} - {item.metric}");
            }
            SetGraficosVisitas(pnls);
        }
        private void SetGraficosVisitas(List<Pnl> pnls)
        {
            var minimum = pnls.Min(x => x.Value);
            var maximum = pnls.Max(x => x.Value);

            _Model = new PlotModel { Title = "Relatórios" };

            var series = new AreaSeries
            {
                Title = "Visitas/Sessões",
                ItemsSource = pnls,
                DataFieldX = "Time",
                DataFieldY = "Value",
                Color = OxyColor.Parse("#4CAF50"),
                Fill = OxyColor.Parse("#454CAF50"),
                MarkerSize = 5,
                MarkerFill = OxyColor.Parse("#FFFFFFFF"),
                MarkerStroke = OxyColor.Parse("#4CAF50"),
                MarkerStrokeThickness = 2,
                MarkerType = MarkerType.Circle,
                StrokeThickness = 2,
            };
            _Model.Series.Add(series);

            var annotation = new LineAnnotation
            {
                Type = LineAnnotationType.Horizontal,
                Y = 0
            };
            _Model.Annotations.Add(annotation);

            var dateTimeAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                IntervalType = DateTimeIntervalType.Days,
                IntervalLength = 100,
                StringFormat = "dd/MM/yyyy"
            };
            _Model.Axes.Add(dateTimeAxis);

            var margin = (maximum - minimum) * 0.05;

            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = minimum - margin,
                Maximum = maximum + margin,
            };
            _Model.Axes.Add(valueAxis);

            this.VisitasModel = _Model;
            VisitasIsVisible = Visibility.Visible;
            IsVisibleLoading = Visibility.Collapsed;

        }
        /// <summary>
        /// Origem
        /// </summary>
        #endregion

        private async void getVisitasPizza(DateRange data)
        {
            var list = await service.getVisitasPizza("307227049", data); 
            foreach (AnaliticsData item in list)
            {
                Debug.WriteLine($"{item.dimension} - {item.metric}");
            }
            SetGraficosPizza(list);
        }


        private void SetGraficosPizza(List<AnaliticsData> list)
        {
            _ModelPiza = new PlotModel { Title = "Origem" };

            dynamic seriesP1 = new PieSeries { StrokeThickness = 3.0, AngleSpan = 360, StartAngle = 0, OutsideLabelFormat = "{2:0.##}", InsideLabelFormat = "" };
            if (_listLegends.Count > 0) _listLegends.Clear();
            foreach (var item in list)
            {
                string corValor = item.dimension.Equals("(none)") ? "#C2185B" : item.dimension.Equals("email") ? "#FF5722" : item.dimension.Equals("feed") ? "#9E9E9E" : item.dimension.Equals("organic") ? "#4CAF50" : "#03A9F4";

                if (item.dimension.Equals("(none)")) item.dimension = "desconhecido";
                seriesP1.Slices.Add(new PieSlice(item.dimension, Convert.ToDouble(item.metric)) { IsExploded = false, Fill = OxyColor.Parse(corValor) });
                _listLegends.Add(new LegendList() { color = corValor, legenda = item.dimension, count = item.metric });
            }
            listLegends = _listLegends;
            _ModelPiza.Series.Add(seriesP1);
            PizzaModel = _ModelPiza;
            PizzaIsVisible = Visibility.Visible;
            IsVisibleLoading = Visibility.Collapsed;
        }

        private string ConvertData(string dataPassada)
        {

            if (!string.IsNullOrEmpty(dataPassada)) return Convert.ToDateTime(dataPassada.Replace("/", "-")).ToString("yyyy-MM-dd");

            return null;

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private enum DateComparisonResult
        {
            Earlier = -1,
            Later = 1,
            TheSame = 0
        };

        private DateRange valideData()
        {
            var startDate = Convert.ToDateTime(ConvertData(DataInicial));
            var endDate = Convert.ToDateTime(ConvertData(DataFinal));
            var comparison = (DateComparisonResult)endDate.CompareTo(startDate);
            if (comparison.ToString().Equals("Earlier")) return null;
            return new DateRange { StartDate = startDate.ToString("yyyy-MM-dd"), EndDate = endDate.ToString("yyyy-MM-dd") };
        }

    }
}
