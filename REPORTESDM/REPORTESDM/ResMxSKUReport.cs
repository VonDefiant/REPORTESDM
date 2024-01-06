using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;

namespace TuAppXamarin
{
    public class ResMxSKUReportA
    {
        private string dbPath;

        public ResMxSKUReportA(string databasePath)
        {
            dbPath = databasePath;
        }

        public List<SKUReportData> ObtenerDatos(string mfecha, string companiadm)
        {
            try
            {
                string path = dbPath;
                string fechaBuscada = mfecha;
                string clasificacionSeleccion = "%";

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"No se encontró la base de datos en la ruta especificada: {path}");
                }

                using (SQLiteConnection conn = new SQLiteConnection(path))
                {
                    conn.CreateTable<SKUReportData>(); // Asegurar que la tabla está creada

                    var datosConsulta = RealizarConsulta(conn, fechaBuscada, companiadm, clasificacionSeleccion);
                    var descripcionesClasificacion = ObtenerDescripcionesClasificacion(conn, fechaBuscada, companiadm);

                    // Puedes manejar las dos consultas según tus necesidades

                    return datosConsulta;
                }
            }
            catch (Exception e)
            {
                // Manejo de errores
                Console.WriteLine($"Error: {e.Message}");
                return new List<SKUReportData>();
            }
        }

        public List<SKUReportData> RealizarConsulta(SQLiteConnection conn, string fechaBuscada, string companiadm, string clasificacionSeleccion)
        {
            string consulta = @"
                SELECT 
                    PROD.COD_ART AS COD_ART,
                    DES_ART AS DESCRIPCION,
                    SUM(CNT_MAX + (CNT_MIN * 0.1)) AS UNIDADES,
                    'Q ' || ROUND(SUM((MON_TOT - DET.MON_DSC) * 1.12), 2) AS VENTA,
                    COUNT(DISTINCT COD_CLT) AS NUMERO_COBERTURAS
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
                    AND CLA.DESCRIPCION LIKE ?
                GROUP BY 
                    PROD.COD_ART, DES_ART
                HAVING
                    SUM((MON_TOT - DET.MON_DSC) * 1.12) > 0
            ";

            var datosConsulta = conn.Query<SKUReportData>(consulta, fechaBuscada, companiadm, clasificacionSeleccion);
            return datosConsulta;
        }

        public static string ObtenerConsultaClasificacion()
        {
            return @"
                SELECT 
                    CLA.DESCRIPCION
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
                    CLA.DESCRIPCION
                HAVING
                    SUM((MON_TOT - DET.MON_DSC) * 1.12) > 0
            ";
        }
        public List<string> ObtenerDescripcionesClasificacion(SQLiteConnection conn, string fechaBuscada, string companiadm)
        {
            string consultaClasificacion = ObtenerConsultaClasificacion();

            var clasificacionConsulta = conn.Query<ClasificacionData>(consultaClasificacion, fechaBuscada, companiadm);

            // Extraer las descripciones de clasificación y convertirlas a una lista de strings
            var opcionesClasificacion = clasificacionConsulta.Select(c => c.DESCRIPCION).ToList();

            return opcionesClasificacion;
        }

    }

    public class SKUReportData
    {
        public string COD_ART { get; set; }
        public string DESCRIPCION_CLASIFICACION { get; set; }
        public string DESCRIPCION { get; set; }
        public double UNIDADES { get; set; }
        public string VENTA { get; set; }
        public int NUMERO_COBERTURAS { get; set; }
    }

    public class ClasificacionData
    {
        public string DESCRIPCION { get; set; }
    }
}
