using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos
{
    public class ConexionJena
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint = "http://localhost:3030/Reserv/sparql";
        private readonly string _endpointUpdate = "http://localhost:3030/Reserv/update";

        private readonly string _prefixes = @"PREFIX : <http://www.semanticweb.org/ProyectoGC/> 
            PREFIX owl: <http://www.w3.org/2002/07/owl#> 
            PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
            PREFIX xml: <http://www.w3.org/XML/1998/namespace> 
            PREFIX xsd: <http://www.w3.org/2001/XMLSchema#> 
            PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> 
            BASE <http://www.semanticweb.org/ProyectoGC/> 
        ";

        public ConexionJena()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> EjecutarConsultaAsync(string consulta)
        {
            var fullConsulta = _prefixes + "\n" + consulta;

            var request = new HttpRequestMessage(HttpMethod.Post, _endpoint);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("query", fullConsulta)
            });
            request.Content = content;
            request.Headers.Add("Accept", "application/sparql-results+json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"Error en la consulta SPARQL: {response.StatusCode}");
            }
        }

        public async Task EjecutarUpdate(string sparqlUpdate)
        {
            var fullUpdate = _prefixes + "\n" + sparqlUpdate;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("update", fullUpdate)
            });

            var response = await _httpClient.PostAsync(_endpointUpdate, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error ejecutando SPARQL UPDATE: {response.StatusCode} - {error}");
            }
        }

    }
}