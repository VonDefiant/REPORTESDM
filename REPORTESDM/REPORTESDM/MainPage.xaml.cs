﻿using System;
using System.IO;
using Xamarin.Forms;

namespace TuAppXamarin
{
    public partial class MainPage : ContentPage
    {
        private efectivreport efectReport;
        private ResMxFamReport resMxFamReport; // Agregado

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
            resMxFamReport = new ResMxFamReport(filePath); // Agregado
        }

        private void OnGenerarButtonClicked(object sender, EventArgs e)
        {
            // Obtener la fecha seleccionada del DatePicker
            DateTime fechaSeleccionada = FechaDatePicker.Date;

            // Formatear la fecha seleccionada como "M/d/yyyy"
            string fechaBuscada = fechaSeleccionada.ToString("M/d/yyyy");

            // Obtener el tipo de informe seleccionado
            string tipoInforme = TipoInformePicker.SelectedItem?.ToString();
            string companiadm = "DISMOGT";

            if (!string.IsNullOrEmpty(tipoInforme))
            {
                // Seleccionar el informe según el tipo
                switch (tipoInforme)
                {
                    case "Efectividad":
                        efectReport.ActualizarDatos(fechaBuscada, DataLabel, ErrorLabel, tipoInforme);
                        break;
                    case "Venta por familia": 
                        resMxFamReport.GenerarReporte(DataLabel, ErrorLabel, fechaBuscada, companiadm);
                        break;
                    default:
                        ErrorLabel.Text = "Tipo de informe no reconocido";
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
