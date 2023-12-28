using System;
using System.Collections.Generic;
using System.IO;
using SQLite;

namespace TuAppXamarin
{
    public class ResMxFamReport
    {
        private string dbPath;

        public ResMxFamReport(string databasePath)
        {
            dbPath = databasePath;
        }

        public List<ReporteData> ObtenerDatos(string mfecha, string companiadm)
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
                    conn.CreateTable<ReporteData>(); // Asegurar que la tabla está creada

                    var datosConsulta = RealizarConsulta(conn, fechaBuscada, companiadm);

                    return datosConsulta;
                }
            }
            catch (Exception e)
            {
                // Manejo de errores
                Console.WriteLine($"Error: {e.Message}");
                return new List<ReporteData>();
            }
        }

        private List<ReporteData> RealizarConsulta(SQLiteConnection conn, string fechaBuscada, string companiadm)
        {
            // Corrige la cadena de consulta
            string consulta = @"
                        SELECT 
                            COD_FAM,
                            CLA.DESCRIPCION,
                            SUM(CNT_MAX + (CNT_MIN * 0.1)) AS UNIDADES,
                            'Q ' || ROUND(SUM((MON_TOT - DET.MON_DSC) * 1.12), 2) AS VENTA,
                            COUNT(DISTINCT COD_CLT) AS NUMERO_CLIENTES
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
                            AND COMPANIA = ?
                        GROUP BY 
                            COD_FAM, CLA.DESCRIPCION
                    ";

            var datosConsulta = conn.Query<ReporteData>(consulta, fechaBuscada, companiadm);
            return datosConsulta;
        }
    }
}

public class ReporteData
{
    public string COD_FAM { get; set; }
    public string DESCRIPCION { get; set; }
    public double UNIDADES { get; set; }
    public string VENTA { get; set; }
    public int NUMERO_CLIENTES { get; set; }
}
