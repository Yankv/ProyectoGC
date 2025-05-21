using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Utilidad
{
    public static class SparqlJsonParser
    {
        public static DataTable ParseToDataTable(string jsonSparql)
        {
            var dataTable = new DataTable();

            var parsed = JObject.Parse(jsonSparql);
            var variables = parsed["head"]?["vars"]?.ToObject<List<string>>() ?? new List<string>();
            var bindings = parsed["results"]?["bindings"]?.ToArray();

            // Crear columnas
            foreach (var varName in variables)
                dataTable.Columns.Add(varName);

            // Agregar filas
            foreach (var binding in bindings!)
            {
                var row = dataTable.NewRow();
                foreach (var varName in variables)
                {
                    row[varName] = binding[varName]?["value"]?.ToString() ?? "";
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
