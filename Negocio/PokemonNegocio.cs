using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dominio;

namespace Negocio
{
    public class PokemonNegocio
    {
        public List<Pokemon> listar()
        {
            List<Pokemon> lista = new List<Pokemon>();
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;

            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=POKEDEX_DB; integrated security=true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "Select Numero, Nombre, P.Descripcion, UrlImagen, E.Descripcion Tipo, D.Descripcion Debilidad, P.IdTipo, P.IdDebilidad, P.Id From POKEMONS P, ELEMENTOS E, ELEMENTOS D Where E.Id = P.IdTipo And D.Id = P.IdDebilidad And P.Activo = 1 ";
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    Pokemon aux = new Pokemon();
                    aux.ID = (int)lector["Id"];
                    aux.NUMERO = lector.GetInt32(0);
                    aux.NOMBRE = (string)lector["Nombre"];
                    aux.DESCRIPCION = (string)lector["Descripcion"];

                    //aux.URLIMAGEN = (string)lector["UrlImagen"];
                    //if(!(lector.IsDBNull(lector.GetOrdinal("UrlImagen"))))
                    //    aux.UrlImagen = (string)lector["UrlImagen"];
                    if (!(lector["UrlImagen"] is DBNull))
                        aux.URLIMAGEN = (string)lector["UrlImagen"];
                    else
                        aux.URLIMAGEN = "";

                    aux.TIPO = new Elemento();
                    aux.TIPO.Id = (int)lector["IdTipo"];
                    aux.TIPO.Descripcion = (string)lector["Tipo"];
                    aux.DEBILIDAD = new Elemento();
                    aux.DEBILIDAD.Id = (int)lector["IdDebilidad"];
                    aux.DEBILIDAD.Descripcion = (string)lector["Debilidad"];

                    lista.Add(aux);
                }

                conexion.Close();
                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Pokemon> filtrar(string campo, string criterio, string filtro)
        {
            List<Pokemon> lista = new List<Pokemon>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string consulta = @"SELECT Numero, Nombre, P.Descripcion, UrlImagen, 
                                   E.Descripcion AS Tipo, D.Descripcion AS Debilidad, 
                                   P.IdTipo, P.IdDebilidad, P.Id 
                            FROM POKEMONS P 
                            JOIN ELEMENTOS E ON E.Id = P.IdTipo 
                            JOIN ELEMENTOS D ON D.Id = P.IdDebilidad 
                            WHERE P.Activo = 1 AND ";

                campo = campo.ToLower().Trim(); // Normalizo el campo para que no rompa

                if (campo == "numero" || campo == "número")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += $"Numero > {filtro}";
                            break;
                        case "Menor a":
                            consulta += $"Numero < {filtro}";
                            break;
                        default:
                            consulta += $"Numero = {filtro}";
                            break;
                    }
                }
                else if (campo == "nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += $"Nombre LIKE '{filtro}%'";
                            break;
                        case "Termina con":
                            consulta += $"Nombre LIKE '%{filtro}'";
                            break;
                        default:
                            consulta += $"Nombre LIKE '%{filtro}%'";
                            break;
                    }
                }
                else if (campo == "descripcion")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += $"P.Descripcion LIKE '{filtro}%'";
                            break;
                        case "Termina con":
                            consulta += $"P.Descripcion LIKE '%{filtro}'";
                            break;
                        default:
                            consulta += $"P.Descripcion LIKE '%{filtro}%'";
                            break;
                    }
                }
                else
                {
                    // Campo desconocido, lanzo excepción para enterarme
                    throw new Exception("Campo no reconocido para filtrar.");
                }

                datos.SetearConsulta(consulta);
                datos.EjecutarConsulta();

                while (datos.Lector.Read())
                {
                    Pokemon aux = new Pokemon();
                    aux.ID = (int)datos.Lector["Id"];
                    aux.NUMERO = datos.Lector.GetInt32(0);
                    aux.NOMBRE = (string)datos.Lector["Nombre"];
                    aux.DESCRIPCION = (string)datos.Lector["Descripcion"];
                    if (!(datos.Lector["UrlImagen"] is DBNull))
                        aux.URLIMAGEN = (string)datos.Lector["UrlImagen"];

                    aux.TIPO = new Elemento
                    {
                        Id = (int)datos.Lector["IdTipo"],
                        Descripcion = (string)datos.Lector["Tipo"]
                    };

                    aux.DEBILIDAD = new Elemento
                    {
                        Id = (int)datos.Lector["IdDebilidad"],
                        Descripcion = (string)datos.Lector["Debilidad"]
                    };

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al filtrar Pokémons: " + ex.Message);
            }
        }
        public void Agregar(Pokemon nuevo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.SetearConsulta("INSERT INTO POKEMONS (Numero, Nombre, Descripcion, Activo, IdTipo, IdDebilidad, UrlImagen) " +
                                     "VALUES (@numero, @nombre, @descripcion, 1, @idTipo, @idDebilidad, @urlImagen)");
                datos.setearParametro("@numero", nuevo.NUMERO);
                datos.setearParametro("@nombre", nuevo.NOMBRE);
                datos.setearParametro("@descripcion", nuevo.DESCRIPCION);
                datos.setearParametro("@idTipo", nuevo.TIPO.Id);
                datos.setearParametro("@urlImagen", nuevo.URLIMAGEN);
                datos.setearParametro("@idDebilidad", nuevo.DEBILIDAD.Id);

                datos.ejecutarAccion(); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        public void modificar(Pokemon poke) 
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("update POKEMONS set Numero = @numero, Nombre = @nombre, Descripcion = @desc, UrlImagen = @img, IdTipo = @idTipo, IdDebilidad = @idDebilidad Where Id = @id");
                datos.setearParametro("@numero", poke.NUMERO);
                datos.setearParametro("@nombre", poke.NOMBRE);
                datos.setearParametro("@desc", poke.DESCRIPCION);
                datos.setearParametro("@img", poke.URLIMAGEN);
                datos.setearParametro("@idTipo", poke.TIPO.Id);
                datos.setearParametro("@idDebilidad", poke.DEBILIDAD.Id);
                datos.setearParametro("@id", poke.ID);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void Eliminar(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.SetearConsulta("delete from POKEMONS where Id = @id");
                datos.setearParametro("@id",id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void EliminarLogico(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.SetearConsulta("update POKEMONS set Activo = 0 where id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
    }

 }
