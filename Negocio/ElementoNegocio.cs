using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dominio;

namespace Negocio
{
    public class ElementoNegocio
    {
        public List<Elemento> listar()
        {
            List<Elemento> lista = new List<Elemento>();
            SqlConnection conexion = new SqlConnection("server=.\\SQLEXPRESS; database=POKEDEX_DB; integrated security=true");
            SqlCommand comando = new SqlCommand("SELECT Id, Descripcion FROM ELEMENTOS", conexion);
            SqlDataReader lector;

            try
            {
                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    Elemento elem = new Elemento();
                    elem.Id = lector.GetInt32(0);
                    elem.Descripcion = lector.GetString(1);
                    lista.Add(elem);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexion.Close();
            }
        }
    }
}
