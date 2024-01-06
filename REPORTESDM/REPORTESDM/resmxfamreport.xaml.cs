using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TuAppXamarin
{
    public partial class ResMxFamReportPage : ContentPage
    {
        private readonly List<ReporteData> _reportData;
        private readonly string _fechaBuscada;
        private readonly string _rutaSeleccionada;

        public ResMxFamReportPage(List<ReporteData> reportData, string fechaBuscada, string rutaSeleccionada)
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
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // COD
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }); // DESCRIPCION
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // UNIDADES
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }); // VENTA
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }); // COBERTURAS

            // Encabezados
            reportGrid.Children.Add(new Label { Text = "COD", FontAttributes = FontAttributes.Bold, FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center , TextColor = Color.White }, 0, 0);
            reportGrid.Children.Add(new Label { Text = "DESCRIPCION", FontAttributes = FontAttributes.Bold, FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 1, 0);
            reportGrid.Children.Add(new Label { Text = "UND", FontAttributes = FontAttributes.Bold, FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 2, 0);
            reportGrid.Children.Add(new Label { Text = "VENTA", FontAttributes = FontAttributes.Bold, FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center , TextColor = Color.White }, 3, 0);
            reportGrid.Children.Add(new Label { Text = "COB", FontAttributes = FontAttributes.Bold, FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center , TextColor = Color.White }, 4, 0);

            // Datos
            for (int i = 0; i < _reportData.Count; i++)
            {
                reportGrid.Children.Add(new Label { Text = _reportData[i].COD_FAM, FontSize = 13, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center , TextColor = Color.White }, 0, i + 1);
                reportGrid.Children.Add(new Label { Text = _reportData[i].DESCRIPCION, FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center , TextColor = Color.White }, 1, i + 1);
                reportGrid.Children.Add(new Label { Text = _reportData[i].UNIDADES.ToString(), FontSize = 13, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center , TextColor = Color.White }, 2, i + 1);
                reportGrid.Children.Add(new Label { Text = _reportData[i].VENTA, FontSize = 13, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 3, i + 1);
                reportGrid.Children.Add(new Label { Text = _reportData[i].NUMERO_CLIENTES.ToString(), FontSize = 13, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 4, i + 1);
            }

            // Ajustar el espaciado del StackLayout
            Content = new StackLayout
            {
                Spacing = 10, // Puedes ajustar el valor según sea necesario
                Children = {
                    new Label { Text = $"REPORTE VENTA POR PROVEEDOR {_fechaBuscada} - RUTA: {_rutaSeleccionada} ", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, FontSize = 21,TextColor = Color.White },
                    reportGrid
                }
            };
        }
    }
}
