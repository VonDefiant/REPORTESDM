using REPORTESDM;
using SQLite;
using System;
using System.IO;
using Xamarin.Forms;

namespace TuAppXamarin
{
    public partial class MainPage : ContentPage
    {
        private efectivreport efectReport;
        private ResMxFamReport resMxFamReport;
        private ResMxSKUReportA resMxSKUReport;
        private SQLiteConnection _conn;
        public string rutaSeleccionada;  

        public MainPage()
        {
            InitializeComponent();

            // Intenta encontrar el archivo en la primera ubicación
            string filePath = "/storage/emulated/0/FRM600.db";

            // Si no se encuentra en la primera ubicación, intenta en la segunda
            if (!File.Exists(filePath))
            {
                filePath = "otra_ruta/FRM600.db"; // Reemplaza "otra_ruta" con la ubicación real
            }

            _conn = new SQLiteConnection(filePath); // Inicializa _conn
            efectReport = new efectivreport(filePath);
            resMxFamReport = new ResMxFamReport(filePath);
            resMxSKUReport = new ResMxSKUReportA(filePath);


            rutaSeleccionada = ObtenerRutaDesdeBD();
        }

        // Método para ejecutar la consulta y devolver la ruta
        public string ObtenerRutaDesdeBD()
        {
            try
            {
                // Consulta SQL para obtener la ruta
                string consulta = "SELECT RUTA FROM ERPADMIN_RUTA_CFG LIMIT 1";  // LIMIT 1 para obtener solo un resultado

                // Ejecuta la consulta y obtén el resultado
                var rutaSeleccionada = _conn.ExecuteScalar<string>(consulta);

                return rutaSeleccionada;
            }
            catch (Exception ex)
            {
                // Maneja cualquier excepción que pueda ocurrir al ejecutar la consulta
                Console.WriteLine("Error al obtener la ruta desde la base de datos: " + ex.Message);
                return null;
            }
        }


        private async void OnGenerarButtonClicked(object sender, EventArgs e) 
        {
            // Obtener la fecha seleccionada del DatePicker
            DateTime fechaSeleccionada = FechaDatePicker.Date;

            // Formatear la fecha seleccionada como "M/d/yyyy"
            string fechaBuscada = fechaSeleccionada.ToString("M/d/yyyy");   

            // Obtener el tipo de informe seleccionado
            string tipoInforme = TipoInformePicker.SelectedItem?.ToString();
            string companiadm = "DISMOGT";
            DataLabel.Text = string.Empty;

            if (!string.IsNullOrEmpty(tipoInforme))
            {
                // Seleccionar el informe según el tipo
                switch (tipoInforme)
                {
                    case "Efectividad":
                        efectReport.ActualizarDatos(fechaBuscada, DataLabel, ErrorLabel, tipoInforme);
                        break;
                    case "Venta por familia":
                        var resultFamilia = resMxFamReport.ObtenerDatos(fechaBuscada, companiadm);
                        await Navigation.PushAsync(new ResMxFamReportPage(resultFamilia, fechaBuscada, rutaSeleccionada)); 
                        break;
                    case "Venta por SKU":
                        var resultSKUA = resMxSKUReport.ObtenerDatos(fechaBuscada, companiadm);
                        await Navigation.PushAsync(new ResMxSKUReport(resultSKUA, fechaBuscada, _conn, companiadm, rutaSeleccionada));
                        break;
                }
            }
            else
            {
                // Manejar el caso en que no se ha seleccionado un tipo de informe
                ErrorLabel.Text = "Seleccione un tipo de informe";
            }
        }
    }
}
