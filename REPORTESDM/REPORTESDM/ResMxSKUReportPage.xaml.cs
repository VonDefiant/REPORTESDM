using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TuAppXamarin
{
    public partial class ResMxSKUReport : ContentPage
    {
        private readonly List<SKUReportData> _reportData;
        private readonly string _fechaBuscada;

        public ResMxSKUReport(List<SKUReportData> reportData, string fechaBuscada)
        {
            InitializeComponent();
            _reportData = reportData;
            _fechaBuscada = fechaBuscada;

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
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) }); // DESCRIPCION
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }); // UNIDADES
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }); // VENTA
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }); // COBERTURAS


            // Encabezados
            reportGrid.Children.Add(new Label { Text = "DESCRIPCION", FontAttributes = FontAttributes.Bold, FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 0, 0);
            reportGrid.Children.Add(new Label { Text = "UND", FontAttributes = FontAttributes.Bold, FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 1, 0);
            reportGrid.Children.Add(new Label { Text = "VENTA", FontAttributes = FontAttributes.Bold, FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 2, 0);
            reportGrid.Children.Add(new Label { Text = "COB", FontAttributes = FontAttributes.Bold, FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 3, 0);

            // Datos
            for (int i = 0; i < _reportData.Count; i++)
            {
                // Ajusta el índice para reflejar la eliminación de la columna "COD"
                reportGrid.Children.Add(new Label { Text = _reportData[i].DESCRIPCION, FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 0, i + 1);
                reportGrid.Children.Add(new Label { Text = _reportData[i].UNIDADES.ToString(), FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 1, i + 1);
                reportGrid.Children.Add(new Label { Text = _reportData[i].VENTA, FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 2, i + 1);
                reportGrid.Children.Add(new Label { Text = _reportData[i].NUMERO_COBERTURAS.ToString(), FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 3, i + 1);
            }


            // Ajustar el espaciado del StackLayout
            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Spacing = 8, // Puedes ajustar el valor según sea necesario
                    Children = {
            new Label { Text = $"REPORTE VENTA POR ARTICULO  {_fechaBuscada}", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, FontSize = 21 },
            reportGrid
        }
                }
            };
        }
    }
}
