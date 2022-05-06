using GoogleAnalytics.Model;
using Google.Analytics.Data.V1Beta;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GoogleAnalytics.Classes
{
    public class AnalyticsService
    {
        BetaAnalyticsDataClient clientAnalytics;

        public AnalyticsService(string pathJson)
        {
            clientAnalytics = new BetaAnalyticsDataClientBuilder
            {
                CredentialsPath = pathJson
            }.Build();
        }

        public async Task<List<AnaliticsData>> getPaginaVista(
            string idPropriedade, 
            DateRange data
            ) {
            List<AnaliticsData> lista = new List<AnaliticsData>();
            try
            {
                RunReportRequest request = new RunReportRequest
                {
                    Property = "properties/" + idPropriedade,
                    Dimensions = { new Dimension { Name = "fullPageUrl" }, },
                    Metrics = { new Metric { Name = "activeUsers" }, },
                    DateRanges = { data },
                    Limit = 100
                };

                var response = await clientAnalytics.RunReportAsync(request);
        
                foreach (Row row in response.Rows)
                {
                    string valor = row.DimensionValues[0].Value;
                    lista.Add(new AnaliticsData { dimension = valor, metric = row.MetricValues[0].Value });

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Debug.WriteLine(ex.Message);
            }
            return lista;

        }

        public async Task<List<AnaliticsData>> getVisitas(
            string idPropriedade,
            DateRange data
            )
        {
            List<AnaliticsData> lista = new List<AnaliticsData>();
            try
            {
                RunReportRequest request = new RunReportRequest
                {
                    Property = "properties/" + idPropriedade,
                    Dimensions = { new Dimension { Name = "date" }},
                    Metrics = { new Metric { Name = "sessions" }, new Metric { Name = "screenPageViews" } },
                    DateRanges = { data },
                    OrderBys = { new OrderBy { Dimension = new OrderBy.Types.DimensionOrderBy{ DimensionName = "date" }, Desc = true }} 
                };

                var response = await clientAnalytics.RunReportAsync(request);

                foreach (Row row in response.Rows)
                {
                    lista.Add(new AnaliticsData { dimension = row.DimensionValues[0].Value, metric = row.MetricValues[0].Value, metric2 = row.MetricValues[1].Value });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Debug.WriteLine(ex.Message);
            }
            return lista;

        }

        public async Task<List<AnaliticsData>> getVisitasPizza(string idPropriedade, DateRange data)
        {
            List<AnaliticsData> lista = new List<AnaliticsData>();
            try
            {
                RunReportRequest request = new RunReportRequest
                {
                    Property = "properties/" + idPropriedade,
                    Dimensions = { new Dimension { Name = "sessionMedium" } },
                    Metrics = { new Metric { Name = "sessions" }},
                    DateRanges = { data },
                    Limit = 50
                };

                var response = await clientAnalytics.RunReportAsync(request);

                foreach (Row row in response.Rows)
                {
                    lista.Add(new AnaliticsData { dimension = row.DimensionValues[0].Value, metric = row.MetricValues[0].Value });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Debug.WriteLine(ex.Message);
            }
            return lista;

        }
    }
}
