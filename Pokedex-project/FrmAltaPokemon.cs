using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using Dominio;
using Negocio;
using System.Configuration;

namespace Pokedex_project
{
    public partial class FrmAltaPokemon : Form
    {
        private Pokemon pokemon = null;
        private OpenFileDialog archivo = null;
        public FrmAltaPokemon()
        {
            InitializeComponent();
        }

        public FrmAltaPokemon(Pokemon pokemon)
        {
            InitializeComponent();
            this.pokemon = pokemon;
            Text = "Modificar Pokemon";
        }

        private void FrmAltaPokemon_Load(object sender, EventArgs e)
        {
            ElementoNegocio elementoNegocio = new ElementoNegocio();
            try
            {
                List<Elemento> lista = elementoNegocio.listar();

                cboTipo.DataSource = lista;
                cboTipo.DisplayMember = "Descripcion";
                cboTipo.ValueMember = "Id";

                cboDebilidad.DataSource = new List<Elemento>(lista); // Copia para evitar que se compartan referencias
                cboDebilidad.DisplayMember = "Descripcion";
                cboDebilidad.ValueMember = "Id";

                if (pokemon != null)
                {
                    txtNumero.Text = pokemon.NUMERO.ToString();
                    txtNombre.Text = pokemon.NOMBRE;
                    txtDescripcion.Text = pokemon.DESCRIPCION;
                    txtUrlimagen.Text = pokemon.URLIMAGEN;
                    cargarImagen(pokemon.URLIMAGEN);
                    cboTipo.SelectedValue = pokemon.TIPO.Id;
                    cboDebilidad.SelectedValue = pokemon.DEBILIDAD.Id;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar combos: " + ex.ToString());
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnAceptar_Click(object sender, EventArgs e)
        {
            PokemonNegocio negocio = new PokemonNegocio();

            try
            {
                if (pokemon == null)
                {
                    pokemon = new Pokemon();
                }

                // Cargar datos
                pokemon.NUMERO = int.Parse(txtNumero.Text);
                pokemon.NOMBRE = txtNombre.Text;
                pokemon.DESCRIPCION = txtDescripcion.Text;
                pokemon.URLIMAGEN = txtUrlimagen.Text;
                pokemon.TIPO = (Elemento)cboTipo.SelectedItem;
                pokemon.DEBILIDAD = (Elemento)cboDebilidad.SelectedItem;

                // Guardar en base
                if (pokemon.ID != 0)
                {
                    negocio.modificar(pokemon);
                    MessageBox.Show("Pokemon modificado exitosamente.");
                }
                else
                {
                    negocio.Agregar(pokemon);
                    MessageBox.Show("Pokemon agregado exitosamente.");
                }

                //Guardo imagen si la levantó localmente:
                if (archivo != null && !(txtUrlimagen.Text.ToUpper().Contains("HTTP")))
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar Pokemon: " + ex.ToString());
            }
        }

        private void txtUrlimagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtUrlimagen.Text);
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbxPokemons.Load(imagen);
            }
            catch (Exception ex)
            {
                pbxPokemons.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg;|png|*.png";
            if (archivo.ShowDialog() == DialogResult.OK)
            {
                txtUrlimagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);

                //guardo la imagen
                //File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);
            }
        }
    }
}
