using System;
using System.IO;
using Xamarin.Forms;

namespace TuAppXamarin
{
    public partial class MainPage : ContentPage
    {
        private efectivreport efectReport;

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

            efectReport = new efectivreport(filePath);
        }


        private void OnGenerarButtonClicked(object sender, EventArgs e)
        {
            // Obtener la fecha seleccionada del DatePicker
            DateTime fechaSeleccionada = FechaDatePicker.Date;

            // Formatear la fecha seleccionada como "M/d/yyyy"
            string fechaBuscada = fechaSeleccionada.ToString("M/d/yyyy");

            // Obtener el tipo de informe seleccionado
            string tipoInforme = TipoInformePicker.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(tipoInforme))
            {
                // Llamar al método en la nueva clase con el tipo de informe
                efectReport.ActualizarDatos(fechaBuscada, DataLabel, ErrorLabel, tipoInforme);
            }
            else
            {
                // Manejar el caso en que no se ha seleccionado un tipo de informe
                ErrorLabel.Text = "Seleccione un tipo de informe";
            }
        }
    }
}
