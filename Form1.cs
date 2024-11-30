using Mqtt.Views;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace Mqtt
{
    public partial class Form1 : Form
    {
      

        public Form1()
        {
            InitializeComponent();
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Verificar se o painel já foi adicionado
            var painelExistente = panelView.Controls.OfType<Options>().FirstOrDefault();

            if (painelExistente == null) // Se não existir, criar e adicionar
            {
                Options painelConfig = new Options();
                panelView.Controls.Add(painelConfig);
                painelConfig.Dock = DockStyle.Top;
            }
            else
            {
                MessageBox.Show("O painel já está aberto.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
          
        }
    }
}
