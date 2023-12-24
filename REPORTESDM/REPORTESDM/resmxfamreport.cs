using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace TuAppXamarin
{
    public class ResMxFamReport
    {
        private string dbPath;


        public ResMxFamReport(string databasePath)
        {
            dbPath = databasePath;
        }

        public void GenerarReporte(Label dataLabel, Label errorLabel, string mfecha, string companiadm)
        {
            try
            {
                string path = dbPath;
                string fechaBuscada = mfecha;

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"No se encontró la base de datos en la ruta especificada: {path}");
                }

                using (SQLiteConnection conn = new SQLiteConnection(path))
                {
                    var datosConsulta = RealizarConsulta(conn, fechaBuscada, companiadm);

                    MostrarDatosEnLabel(dataLabel, datosConsulta, fechaBuscada);

                }

                errorLabel.Text = "";
            }
            catch (Exception e)
            {
                errorLabel.Text = e.Message;
            }
        }

        private IEnumerable<string> RealizarConsulta(SQLiteConnection conn, string fechaBuscada, string companiadm)
        {
            // Corrige la cadena de consulta
            string consulta = @"
                                SELECT 
                                    COD_FAM,
                                    CLA.DESCRIPCION,
                                    SUM(CNT_MAX + (CNT_MIN * 0.1)) AS UNIDADES,
                                    'Q ' || ROUND(SUM((MON_TOT - DET.MON_DSC) * 1.13), 2) AS VENTA
                                FROM 
                                    ERPADMIN_ALFAC_DET_PED DET
                                JOIN 
                                    ERPADMIN_ALFAC_ENC_PED ENC ON DET.NUM_PED = ENC.NUM_PED
                                JOIN 
                                    ERPADMIN_ARTICULO PROD ON PROD.COD_ART = DET.COD_ART
                                JOIN 
                                    ERPADMIN_CLASIFICACION_FR CLA ON SUBSTR(PROD.COD_FAM, 1, 2) = CLA.CLASIFICACION
                                WHERE 
                                    ESTADO <> 'C' AND FEC_PED LIKE ? || '%'
                                    AND COMPANIA = 'DISMOGT'
                                GROUP BY 
                                    COD_FAM
                            ";

            Console.WriteLine("Consulta SQL: " + consulta);
            var datosConsulta = conn.Query<ReporteData>(consulta, fechaBuscada, companiadm);

            var result = new List<string>();
            Console.WriteLine("Consulta SQL: " + consulta);

            foreach (var item in datosConsulta)
            {
                string formattedData = $"{item.COD_FAM,-15}{item.DESCRIPCION,-30}{item.UNIDADES,-10}{item.VENTA,-10}";
                result.Add(formattedData);
            }

            return result;
        }


        private void MostrarDatosEnLabel(Label dataLabel, IEnumerable<string> datosConsulta, string fechaBuscada)
        {
            // Encabezados
            string header = $"{"COD_FAM",-15}{"DESCRIPCION",-30}{"UNIDADES",-10}{"VENTA",-10}";
            string tableHeader = $"{"REPORTE VOLUMEN POR FAMILIA",-45} FECHA: {fechaBuscada}\n";
            string separator = new string('-', 5);
            List<string> lines = new List<string> { tableHeader, header, separator };

            foreach (var dataLine in datosConsulta)
            {
                lines.Add(dataLine);
            }

            // Unir todas las líneas en un solo string
            string data = string.Join("\n", lines);

            // Asignar el string resultante al Text del Label
            dataLabel.Text = data;
        }



    }

    public class ReporteData
    {
        public string COD_FAM { get; set; }
        public string DESCRIPCION { get; set; }
        public double UNIDADES { get; set; }
        public string VENTA { get; set; }
    }
}
