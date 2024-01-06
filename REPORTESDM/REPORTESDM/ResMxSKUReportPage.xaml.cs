using SQLite;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TuAppXamarin
{
    public partial class ResMxSKUReport : ContentPage
    {
        private readonly List<SKUReportData> _reportData;
        private readonly string _fechaBuscada;
        private readonly SQLiteConnection _conn;
        private readonly string _companiadm;
        private readonly ResMxSKUReportA _resMxSKUReportA;
        private readonly Grid reportGrid;
        private readonly string _rutaSeleccionada; 

        public ResMxSKUReport(List<SKUReportData> reportData, string fechaBuscada, SQLiteConnection conn, string companiadm, string rutaSeleccionada)  // Modifica el constructor para aceptar la ruta
        {
            InitializeComponent();
            _reportData = reportData;
            _fechaBuscada = fechaBuscada;
            _conn = conn;
            _companiadm = companiadm;
            _resMxSKUReportA = new ResMxSKUReportA(conn.DatabasePath);  
            _rutaSeleccionada = rutaSeleccionada;  

            // Establecer el ItemSource inicial
            ReportListView.ItemsSource = _reportData;

            // Inicializar reportGrid
            reportGrid = new Grid();

            InitializePage();
        }
        private void InitializePage()
        {
            // Configurar tamaño de columnas
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }); // DESCRIPCION
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // UNIDADES
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // VENTA
            reportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // COBERTURAS

            // Espaciado entre filas
            reportGrid.RowSpacing = 5;

            // Encabezados
            reportGrid.Children.Add(new Label { Text = "DESCRIPCION", FontAttributes = FontAttributes.Bold, FontSize = 14, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 0, 0);
            reportGrid.Children.Add(new Label { Text = "UND", FontAttributes = FontAttributes.Bold, FontSize = 14, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 1, 0);
            reportGrid.Children.Add(new Label { Text = "VENTA", FontAttributes = FontAttributes.Bold, FontSize = 14, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 2, 0);
            reportGrid.Children.Add(new Label { Text = "COB", FontAttributes = FontAttributes.Bold, FontSize = 14, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 3, 0);

            // Datos
            double totalVenta = 0; // Variable para almacenar la suma de la columna VENTA

            for (int i = 0; i < _reportData.Count; i++)
            {
                reportGrid.Children.Add(new Label { Text = _reportData[i].DESCRIPCION, FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 0, i + 1);
                reportGrid.Children.Add(new Label { Text = _reportData[i].UNIDADES.ToString(), FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 1, i + 1);
                reportGrid.Children.Add(new Label { Text = _reportData[i].VENTA, FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 2, i + 1);
                reportGrid.Children.Add(new Label { Text = _reportData[i].NUMERO_COBERTURAS.ToString(), FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 3, i + 1);

                // Sumar la columna VENTA
                totalVenta += Convert.ToDouble(_reportData[i].VENTA.Replace("Q", ""));
            }

            // Agregar la fila de volumen total
            reportGrid.Children.Add(new Label { Text = "VENTA TOTAL", FontAttributes = FontAttributes.Bold, FontSize = 14, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center , TextColor = Color.White }, 0, _reportData.Count + 1);
            reportGrid.Children.Add(new Label { Text = "", FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center , TextColor = Color.White }, 1, _reportData.Count + 1);
            reportGrid.Children.Add(new Label { Text = $"Q {totalVenta:F2}", FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center , TextColor = Color.White }, 2, _reportData.Count + 1);
            reportGrid.Children.Add(new Label { Text = "", FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 3, _reportData.Count + 1);

            // Agregar el Picker
            var pickerOptions = ObtenerOpcionesParaPicker(); // Reemplaza con tu lógica para obtener opciones
            var picker = new Picker
            {
                Title = "Seleccione un proveedor",
                ItemsSource = pickerOptions
            };
            picker.SelectedIndexChanged += OnPickerSelectedIndexChanged;

            // Agregar el Picker a la vista
            var stackLayout = new StackLayout
            {
                Spacing = 8,
                Children = {
            new Label { Text = $"REPORTE VENTA POR ARTICULO {_fechaBuscada:dd/MM/yyyy} - RUTA: {_rutaSeleccionada} ", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, FontSize = 21, TextColor = Color.White },
            picker,
            reportGrid
        }
            };

            // Ajustar el espaciado del StackLayout
            Content = new ScrollView
            {
                Content = stackLayout
            };
        }

        private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedOption = ((Picker)sender).SelectedItem;

            if (selectedOption != null)
            {
                // Aplicar el filtro de clasificación
                string clasificacionSeleccion = selectedOption.ToString();

                // Volver a realizar la consulta con la nueva clasificación
                var datosConsulta = _resMxSKUReportA.RealizarConsulta(_conn, _fechaBuscada, _companiadm, clasificacionSeleccion);

                // Limpiar solo los datos existentes en el Grid
                LimpiarDatosEnGrid();

                // Llenar el Grid con los nuevos datos
                double totalVenta = 0; // Variable para almacenar la suma de la columna VENTA

                for (int i = 0; i < datosConsulta.Count; i++)
                {
                    var labelDescripcion = new Label { Text = datosConsulta[i].DESCRIPCION, FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White };
                    var labelUnidades = new Label { Text = datosConsulta[i].UNIDADES.ToString(), FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White };
                    var labelVenta = new Label { Text = datosConsulta[i].VENTA, FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White };
                    var labelCoberturas = new Label { Text = datosConsulta[i].NUMERO_COBERTURAS.ToString(), FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White };

                    // Agregar Labels al Grid
                    reportGrid.Children.Add(labelDescripcion, 0, i + 1);
                    reportGrid.Children.Add(labelUnidades, 1, i + 1);
                    reportGrid.Children.Add(labelVenta, 2, i + 1);
                    reportGrid.Children.Add(labelCoberturas, 3, i + 1);

                    // Sumar la columna VENTA
                    totalVenta += Convert.ToDouble(datosConsulta[i].VENTA.Replace("Q", ""));
                }
                // Agregar la fila de volumen total
                reportGrid.Children.Add(new Label { Text = "VOLUMEN TOTAL", FontAttributes = FontAttributes.Bold, FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 0, datosConsulta.Count + 1);
                reportGrid.Children.Add(new Label { Text = "", FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 1, datosConsulta.Count + 1);
                reportGrid.Children.Add(new Label { Text = $"Q {totalVenta:F2}", FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 2, datosConsulta.Count + 1);
                reportGrid.Children.Add(new Label { Text = "", FontSize = 10, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White }, 3, datosConsulta.Count + 1);
            }
        }



        private void LimpiarDatosEnGrid()
        {
            // Eliminar solo las filas que contienen datos
            for (int i = reportGrid.Children.Count - 1; i >= 0; i--)
            {
                var child = reportGrid.Children[i];
                var row = Grid.GetRow(child);

                if (row > 0)
                {
                    reportGrid.Children.Remove(child);
                }
            }
        }


        private List<string> ObtenerOpcionesParaPicker()
        {
            var opcionesClasificacion = _resMxSKUReportA.ObtenerDescripcionesClasificacion(_conn, _fechaBuscada, _companiadm);

            return opcionesClasificacion;
        }

    }
}
