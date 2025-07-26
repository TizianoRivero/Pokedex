using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using Negocio;

namespace Pokedex_project
{
    public partial class FrmPokemons : Form
    {
        private List<Pokemon> listaPokemon;
        public FrmPokemons()
        {
            InitializeComponent();
        }

        private void cargar()
        {
            PokemonNegocio negocio = new PokemonNegocio();
            listaPokemon = negocio.listar();
            DgvPokemons.DataSource = null;
            DgvPokemons.DataSource = listaPokemon;
            ocultarColumnas();

            if (listaPokemon.Count > 0)
            {
                DgvPokemons.ClearSelection();
                DgvPokemons.Rows[0].Selected = true;
                cargarImagen(listaPokemon[0].URLIMAGEN);
            }
        }

        private void ocultarColumnas()
        {
            DgvPokemons.Columns["UrlImagen"].Visible = false;
            DgvPokemons.Columns["Id"].Visible = false;
        }

        private void FrmPokemons_Load(object sender, EventArgs e)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            listaPokemon = negocio.listar();
            DgvPokemons.DataSource = listaPokemon;
            DgvPokemons.Columns["UrlImagen"].Visible = false;
            DgvPokemons.Columns["Id"].Visible = false;
            pbxPokemons.Load(listaPokemon[0].URLIMAGEN);

            cboCampo.Items.Add("Numero");
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripcion");
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(imagen))
                    pbxPokemons.Load(imagen);
                else
                    pbxPokemons.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
            catch
            {
                pbxPokemons.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
        }

        private void DgvPokemons_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvPokemons.CurrentRow != null)
            {
                Pokemon seleccionado = (Pokemon)DgvPokemons.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.URLIMAGEN);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            FrmAltaPokemon alta = new FrmAltaPokemon();
            alta.ShowDialog();

            PokemonNegocio negocio = new PokemonNegocio();
            listaPokemon = negocio.listar();
            DgvPokemons.DataSource = null;
            DgvPokemons.DataSource = listaPokemon;
            ocultarColumnas();

            if (listaPokemon.Count > 0)
            {
                DgvPokemons.ClearSelection();
                DgvPokemons.Rows[listaPokemon.Count - 1].Selected = true; // selecciona el último
                cargarImagen(listaPokemon[listaPokemon.Count - 1].URLIMAGEN);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (DgvPokemons.CurrentRow == null || DgvPokemons.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("No hay ningún Pokémon seleccionado.");
                return;
            }

            Pokemon seleccionado = (Pokemon)DgvPokemons.CurrentRow.DataBoundItem;

            FrmAltaPokemon modificar = new FrmAltaPokemon(seleccionado);
            modificar.ShowDialog();
            cargar();

            ocultarColumnas();
        }

        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            eliminar();
        }

        private void btnEliminarLogico_Click(object sender, EventArgs e)
        {
            eliminar(true);
        }

        private void eliminar(bool logico = false)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            Pokemon seleccionado;
            try
            {
                DialogResult respuesta = MessageBox.Show("¿Desea eliminar pokemon?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Pokemon)DgvPokemons.CurrentRow.DataBoundItem;

                    if (logico)
                        negocio.EliminarLogico(seleccionado.ID);
                    else
                        negocio.Eliminar(seleccionado.ID);

                    cargar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarFiltro()
        {
            if (cboCampo.SelectedIndex < 0 || cboCampo.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccioná un *campo* para filtrar.");
                return true;
            }

            if (cboCriterio.SelectedIndex < 0 || cboCriterio.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccioná un *criterio* para filtrar.");
                return true;
            }

            if (string.IsNullOrWhiteSpace(txtFiltroAvanzado.Text))
            {
                MessageBox.Show("El filtro no puede estar vacío.");
                return true;
            }

            if (cboCampo.SelectedItem.ToString() == "Número")
            {
                if (!soloNumeros(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Solo números permitidos para filtrar por campo numérico.");
                    return true;
                }
            }

            return false;
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
            return true;
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            PokemonNegocio negocio = new PokemonNegocio();

            try
            {
                if (validarFiltro())
                    return;

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;

                // Trazas para depurar
                Console.WriteLine($"Campo: {campo} | Criterio: {criterio} | Filtro: {filtro}");

                var listaFiltrada = negocio.filtrar(campo, criterio, filtro);

                if (listaFiltrada == null || listaFiltrada.Count == 0)
                {
                    MessageBox.Show("No se encontraron resultados con ese filtro.");
                }

                DgvPokemons.DataSource = listaFiltrada;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al aplicar el filtro: " + ex.Message);
            }
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            try
            {
                // Validaciones de UI
                if (cboCampo.SelectedItem == null)
                {
                    MessageBox.Show("Seleccioná un campo para filtrar.");
                    return;
                }

                if (cboCriterio.SelectedItem == null)
                {
                    MessageBox.Show("Seleccioná un criterio para filtrar.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Completá el valor para filtrar.");
                    return;
                }

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;

                // Validación adicional si el campo es numérico
                if (campo == "Número")
                {
                    if (!int.TryParse(filtro, out int numero))
                    {
                        MessageBox.Show("El filtro debe ser un número válido.");
                        return;
                    }
                }

                DgvPokemons.DataSource = negocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar: " + ex.Message);
            }
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Pokemon> listaFiltrada;
            string filtro = txtFiltro.Text;

            if (filtro != "")
            {
                listaFiltrada = listaPokemon.FindAll(x => x.NOMBRE.ToUpper().Contains(filtro.ToUpper()) || x.TIPO.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                listaFiltrada = listaPokemon;
            }

            DgvPokemons.DataSource = null;
            DgvPokemons.DataSource = listaFiltrada;
            ocultarColumnas();
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();
            if(opcion == "Numero")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a..");
                cboCriterio.Items.Add("Menor a..");
                cboCriterio.Items.Add("Igual a..");
            }
            else
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con..");
                cboCriterio.Items.Add("Termina con..");
                cboCriterio.Items.Add("Contiene..");
            }
        }
    }
}
