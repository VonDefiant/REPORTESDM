using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TuAppXamarin
{
    public partial class ResMxPEDReportPage : ContentPage
    {
        private readonly List<ReportePedidos> _reportData;
        private readonly string _fechaBuscada;
        private readonly string _rutaSeleccionada;

        public ResMxPEDReportPage(List<ReportePedidos> reportData, string fechaBuscada, string rutaSeleccionada)
        {
            InitializeComponent();
            _reportData = reportData;
            _fechaBuscada = fechaBuscada;
            _rutaSeleccionada = rutaSeleccionada;
            InitializePage();
        }

        private void InitializePage()
        {
            var reportGrid = new Grid();

            // Ajustar el tamaño de las filas
            reportGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Encabezados
            for (int i = 0; i < _reportData.Count; i++)
            {
                reportGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            // Ajustar el tamaño de las columnas
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }); // NUM_PED
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }); // COD_CLT
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2.5, GridUnitType.Star) }); // NOM_CLT
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // MON_CIV

            reportGrid.RowSpacing = 5;

            // Encabezados
            reportGrid.Children.Add(new Label { Text = "NUM PED", FontAttributes = FontAttributes.Bold, FontSize = 14, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 0, 0);
            reportGrid.Children.Add(new Label { Text = "CODIGO", FontAttributes = FontAttributes.Bold, FontSize = 14, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 1, 0);
            reportGrid.Children.Add(new Label { Text = "CLIENTE", FontAttributes = FontAttributes.Bold, FontSize = 14, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 2, 0);
            reportGrid.Children.Add(new Label { Text = "VENTA", FontAttributes = FontAttributes.Bold, FontSize = 14, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 3, 0);

            // Datos
            double totalVenta = 0; // Variable para almacenar la suma de la columna VENTA

            // Datos
            for (int i = 0; i < _reportData.Count; i++)
            {
                reportGrid.Children.Add(new Label { Text = _reportData[i].NUM_PED, FontSize = 11, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 0, i + 1);
                reportGrid.Children.Add(new Label { Text = _reportData[i].COD_CLT, FontSize = 11, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 1, i + 1);
                reportGrid.Children.Add(new Label { Text = _reportData[i].NOM_CLT, FontSize = 11, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 2, i + 1);
                reportGrid.Children.Add(new Label { Text =  _reportData[i].MONTO, FontSize = 11, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 3, i + 1);

                // Sumar la columna VENTA
                totalVenta += Convert.ToDouble(_reportData[i].MONTO.Replace("Q", ""));
            }

            int totalLineas = _reportData.Count; // Contar el total de líneas con datos

            // Agregar la fila de suma total
            reportGrid.Children.Add(new Label { Text = "TOTAL", FontAttributes = FontAttributes.Bold, FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 0, _reportData.Count + 1);
            reportGrid.Children.Add(new Label { Text = "PEDIDOS", FontAttributes = FontAttributes.Bold, FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 1, _reportData.Count + 1);
            reportGrid.Children.Add(new Label { Text = $"{totalLineas}", FontSize = 14, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 2, _reportData.Count + 1);
            reportGrid.Children.Add(new Label { Text = $"Q {totalVenta:F2}", FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 3, _reportData.Count + 1);

            // Ajustar el espaciado del StackLayout y agregar un ScrollView
            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Spacing = 10, // Puedes ajustar el valor según sea necesario
                    Children = {
                        new Label { Text = $"REPORTE DE PEDIDOS {_fechaBuscada} - RUTA: {_rutaSeleccionada}", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, FontSize = 21, TextColor = Color.White },
                        reportGrid
                    }
                }
            };
        }
    }
}
