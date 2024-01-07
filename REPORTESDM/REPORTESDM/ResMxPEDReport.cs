using System;
using System.Collections.Generic;
using System.IO;
using SQLite;
using Xamarin.Forms;

namespace TuAppXamarin
{
    public class ResMxPedidoReport
    {
        private string dbPath;

        public ResMxPedidoReport(string databasePath)
        {
            dbPath = databasePath;
        }

        public List<ReportePedidos> ObtenerDatos(string fechaBuscada, string companiadm)
        {
            try
            {
                string path = dbPath;

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"No se encontró la base de datos en la ruta especificada: {path}");
                }

                using (SQLiteConnection conn = new SQLiteConnection(path))
                {
                    conn.CreateTable<ReportePedidos>(); // Asegurar que la tabla está creada

                    return RealizarConsulta(conn, fechaBuscada, companiadm);
                }
            }
            catch (Exception e)
            {
                // Manejo de errores
                Console.WriteLine($"Error: {e.Message}");
                return new List<ReportePedidos>();
            }
        }

        private List<ReportePedidos> RealizarConsulta(SQLiteConnection conn, string fechaBuscada, string companiadm)
        {
            // Nueva consulta para venta por pedido
            string nuevaConsulta = @"
                    SELECT 
                        PED.[NUM_PED],
                        PED.[COD_CLT],
                        CLIE.NOM_CLT,
                        CASE PED.[COD_CND] 
                            WHEN '01' THEN 'CONTADO'
                            ELSE 'CREDITO'
                        END as 'CONDICION',
                        CASE PED.[COD_PAIS]
                            WHEN 'FCF' THEN 'FACTURA'
                            WHEN 'CCF' THEN 'CREDITO FISCAL'
                        END as 'TIPO_DOC',
                        'Q' || ROUND(CAST(PED.[MON_CIV] AS REAL), 2) AS 'MONTO'
                    FROM [ERPADMIN_ALFAC_ENC_PED] PED 
                    JOIN [ERPADMIN_CLIENTE] CLIE ON PED.COD_CLT = CLIE.COD_CLT
                    WHERE FEC_PED LIKE ? || '%'
                    AND ESTADO <> 'C' AND TIP_DOC='1'
                ";

            return conn.Query<ReportePedidos>(nuevaConsulta, fechaBuscada);
        }

    }
}


public class ReportePedidos
{
    public string NUM_PED { get; set; }
    public string COD_CLT { get; set; }
    public string NOM_CLT { get; set; }
    public string MONTO { get; set; }
}
