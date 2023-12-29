using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;
using Xamarin.Forms;

namespace TuAppXamarin  
{
    public class efectivreport
    {
        private Dictionary<string, string> DIAS = new Dictionary<string, string>
        {
            { "Lunes", "L" },
            { "Martes", "K" },
            { "Miércoles", "M" },
            { "Jueves", "J" },
            { "Viernes", "V" },
            { "Sábado", "S" }
        };

        private string[] titulos = { "Ruta", "Pedidos", "Clientes en rutero", "Clientes con Venta", "Visitas realizadas", "Monto", "Efectividad VTA", "Efectividad visita" };

        private string[] diasSemana = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

        private string data = "";
        private string dbPath;
        public efectivreport(string databasePath)
        {
            dbPath = databasePath;
        }
        public void ActualizarDatos(string seleccion, Label dataLabel, Label errorLabel, string tipoInforme)
        {
            try
            {
                string path = dbPath;
                string fechaBuscada = seleccion;
                DateTime fecha = DateTime.ParseExact(fechaBuscada, "M/d/yyyy", null);

                string diaSemana = fecha.ToString("dddd", new System.Globalization.CultureInfo("es-ES")).CapitalizeFirstLetter();

                string letraDia = DIAS[diaSemana];

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"No se encontró la base de datos en la ruta especificada: {path}");
                }

                using (SQLiteConnection conn = new SQLiteConnection(path))
                {

                    // Nueva consulta para obtener la venta
                    string ventaConsulta = $"SELECT 'Q ' || ROUND(SUM((MON_TOT - DET.MON_DSC) * 1.12), 2) AS VENTA " +
                                            "FROM ERPADMIN_ALFAC_DET_PED DET " +
                                            "JOIN ERPADMIN_ALFAC_ENC_PED ENC ON DET.NUM_PED = ENC.NUM_PED " +
                                            "JOIN ERPADMIN_ARTICULO PROD ON PROD.COD_ART = DET.COD_ART " +
                                            "WHERE ESTADO <> 'C' AND FEC_PED LIKE ? || '%' " +
                                            "GROUP BY FEC_PED";

                    // Obtener datos de venta
                    var ventaResult = conn.ExecuteScalar<string>(ventaConsulta, fechaBuscada);


                    //cuenta los clientes con venta segun DB
                    var clientes = conn.Query<VisitaDocumento>("SELECT CLIENTE FROM ERPADMIN_VISITA_DOCUMENTO WHERE INICIO LIKE ?", fechaBuscada + "%");
                    int cuentaClientes = clientes.Count;

                    //cuenta los clientes con VISITA segun DB
                    var visitas = conn.Query<Visita>("SELECT CLIENTE FROM ERPADMIN_VISITA WHERE INICIO LIKE ?", fechaBuscada + "%");
                    int cuentaVisitas = visitas.Count;

                    string consulta = $"SELECT COUNT(*) FROM ERPADMIN_ALFAC_RUTA_ORDEN WHERE DIA != ? AND COD_CLT IN (SELECT CLIENTE FROM ERPADMIN_VISITA_DOCUMENTO WHERE INICIO LIKE ?)";
                    int cuenta = conn.ExecuteScalar<int>(consulta, letraDia, fechaBuscada + "%");

                    consulta = $"SELECT COUNT(*) FROM ERPADMIN_ALFAC_RUTA_ORDEN WHERE DIA != ? AND COD_CLT IN (SELECT CLIENTE FROM ERPADMIN_VISITA WHERE INICIO LIKE ?)";
                    int cuentaVisita = conn.ExecuteScalar<int>(consulta, letraDia, fechaBuscada + "%");

                    // subconsulta 
                    consulta = $"SELECT RUTA, " +
                               $"(SELECT COUNT(*) FROM ERPADMIN_VISITA_DOCUMENTO WHERE INICIO LIKE ? || '%' AND CLIENTE IN (SELECT CLIENTE FROM ERPADMIN_VISITA_DOCUMENTO WHERE INICIO LIKE ? || '%')) as 'pedidos_local', " +
                               "(SELECT COUNT(DISTINCT COD_CLT) FROM ERPADMIN_ALFAC_RUTA_ORDEN WHERE DIA = ?) as 'ClientesRutero', " +
                               $"ROUND((SELECT COUNT(*) FROM ERPADMIN_VISITA_DOCUMENTO WHERE INICIO LIKE ? || '%' AND CLIENTE IN (SELECT CLIENTE FROM ERPADMIN_VISITA_DOCUMENTO WHERE INICIO LIKE ? || '%')) * 100.0 / (SELECT COUNT(DISTINCT COD_CLT) FROM ERPADMIN_ALFAC_RUTA_ORDEN WHERE DIA = ?), 2) || '%' as 'EfectividadVTA', " +
                               "(SELECT COUNT(*) FROM ERPADMIN_VISITA WHERE INICIO LIKE ? || '%') as 'VisitasRealizadas', " +
                               $"ROUND((SELECT COUNT(*) FROM ERPADMIN_VISITA WHERE INICIO LIKE ? || '%' AND CLIENTE IN (SELECT CLIENTE FROM ERPADMIN_VISITA_DOCUMENTO WHERE INICIO LIKE ? || '%')) * 100.0 / (SELECT COUNT(DISTINCT COD_CLT) FROM ERPADMIN_ALFAC_RUTA_ORDEN WHERE DIA = ?), 2) || '%' as 'EfectividadVisita', " +
                               "(SELECT COUNT(CLIENTE) FROM ERPADMIN_VISITA_DOCUMENTO WHERE INICIO LIKE ? || '%') as 'ClientesVenta' " +
                               "FROM ERPADMIN_JORNADA_RUTAS";
                    var datosRutas = conn.Query<JornadaRutas>(consulta, fechaBuscada, fechaBuscada, letraDia, fechaBuscada, fechaBuscada, letraDia, fechaBuscada, fechaBuscada, letraDia, fechaBuscada);


                    data = "";
                    foreach (var ruta in datosRutas)
                    {
                        data += $"\nFecha: {fechaBuscada}\n";
                        data += $"Día: {diaSemana}\n";
                        data += $"Ruta: {ruta.RUTA,-25}\n";
                        data += $"\nPedidos: {ruta.pedidos_local}\n";
                        data += $"Venta: {ventaResult}\n";
                        data += $"Clientes en el rutero: {ruta.ClientesRutero}\n";
                        data += $"Clientes con venta: {cuentaClientes}\n";
                        data += $"Clientes visitados: {ruta.VisitasRealizadas} \n";;
                        data += $"\nEfectividad de ventas: {ruta.EfectividadVTA} \n";

                        // Calcular la efectividad de visita 
                        double efectividadVisita = (double)ruta.VisitasRealizadas / ruta.ClientesRutero * 100;
                        data += $"Efectividad de visita: {efectividadVisita.ToString("0.00")}% \n";
                        data += $"Clientes con venta fuera de ruta: {cuenta}\n";
                        data += $"Clientes visitados fuera de ruta: {cuentaVisita}\n";
                    }




                }
                dataLabel.Text = data;
                errorLabel.Text = "";
            }
            catch (Exception e)
            {
                errorLabel.Text = e.Message;
            }
        }
    }

    // Clase de extensión fuera de la clase efectivreport, como una clase estática
    public static class StringExtensions
    {
        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            char[] chars = input.ToCharArray();
            chars[0] = char.ToUpper(chars[0]);
            return new string(chars);
        }
    }
}
public class VisitaDocumento
{
    public string CLIENTE { get; set; }
}

public class Visita
{
    public string CLIENTE { get; set; }
}

public class JornadaRutas
{
    public string RUTA { get; set; }
    public int pedidos_local { get; set; }
    public int ClientesRutero { get; set; }
    public int ClientesVenta { get; set; }
    public int VisitasRealizadas { get; set; }
    public string Monto { get; set; }
    public string EfectividadVTA { get; set; }
    public string EfectividadVisita { get; set; }
}