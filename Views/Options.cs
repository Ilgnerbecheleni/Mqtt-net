using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mqtt.Views
{
   
    public partial class Options : UserControl
    {
        private IMqttClient _mqttClient;
        public Options()
        {

            InitializeComponent();
            SetupMqttClient();
        }

        private void SetupMqttClient()
        {
            // Configurar o cliente MQTT
            _mqttClient = new MqttFactory().CreateMqttClient();

            // Eventos
            _mqttClient.ConnectedAsync += async e =>
            {
                UpdateConnectionStatus(true);

            };

            _mqttClient.DisconnectedAsync += e =>
            {
                UpdateConnectionStatus(false);
                //  MessageBox.Show("Desconectado do servidor MQTT!");
                return System.Threading.Tasks.Task.CompletedTask;
            };

            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                // Use PayloadSegment em vez de Payload
                string mensagem = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.ToArray());
                AppendMessage($"{e.ApplicationMessage.Topic}\n : {mensagem}");
                return System.Threading.Tasks.Task.CompletedTask;
            };
        }



     

        private async Task PublishMessage(string topic, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(topic) || string.IsNullOrWhiteSpace(message))
                {
                    MessageBox.Show("Por favor, insira o tópico e a mensagem.");
                    return;
                }

                var mensagem = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(message)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                await _mqttClient.PublishAsync(mensagem);
                //   MessageBox.Show("Mensagem publicada com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao publicar: {ex.Message}");
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
          
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            // Obter o tópico e a mensagem dos campos
            string topic = txtTopic.Text;
            string message = txtMessage.Text;

            await PublishMessage(topic, message);
        }

        private void UpdateConnectionStatus(bool isConnected)
        {
            if (InvokeRequired)
            {
                // Garante que a atualização seja feita na thread de interface do usuário
                Invoke(new Action(() => UpdateConnectionStatus(isConnected)));
                return;
            }

            // Atualiza o campo de status no thread correto
            if (isConnected)
            {
                txtStatus.BackColor = Color.Green;
                txtStatus.Text = "Conectado";
            }
            else
            {
                txtStatus.BackColor = Color.Red;
                txtStatus.Text = "Desconectado";
            }
        }
        private void AppendMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendMessage(message)));
                return;
            }

            // Adiciona a mensagem com uma nova linha no final
            txtMessages.AppendText($"{message}{Environment.NewLine}{Environment.NewLine}");
        }

        private async void btnDesconectar_Click(object sender, EventArgs e)
        {
            try
            {
                // Desconectar do servidor MQTT
                if (_mqttClient != null && _mqttClient.IsConnected)
                {
                    await _mqttClient.DisconnectAsync();
                    //  MessageBox.Show("Desconectado com sucesso!");

                    // Alterar interface para o botão "Conectar"
                    btnConectar.BringToFront();
                    btnConectar.Visible = true;

                    AppendMessage("Desconectado do servidor MQTT!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao desconectar: {ex.Message}");
            }
        }

        private  async void btnConectar_Click(object sender, EventArgs e)
        {

            try
            {
                if (_mqttClient.IsConnected) // Verifica se já está conectado
                {
                    // Desconectar do servidor MQTT
                    await _mqttClient.DisconnectAsync();
                    AppendMessage("Desconectado com sucesso!");
                    btnConectar.Text = "Conectar"; // Alterar o texto do botão
                }
                else
                {
                    // Validação de entrada
                    if (string.IsNullOrWhiteSpace(txtServer.Text) || string.IsNullOrWhiteSpace(txtPort.Text))
                    {
                        MessageBox.Show("Por favor, insira o servidor e a porta.");
                        return;
                    }

                    // Obter os dados do formulário
                    string server = txtServer.Text;
                    int port = int.Parse(txtPort.Text);

                    // Configuração do cliente MQTT
                    var options = new MqttClientOptionsBuilder()
                        .WithClientId("WinFormsClient")
                        .WithTcpServer(server, port)
                        .WithCredentials("teste", "123456") // Substitua por credenciais reais, se necessário
                        .WithCleanSession()
                        .Build();

                    // Conectar ao servidor MQTT
                    await _mqttClient.ConnectAsync(options);
                    AppendMessage("Conectado com sucesso!");
                    btnConectar.Text = "Desconectar"; // Alterar o texto do botão

                    // Inscrever-se no tópico fornecido
                    string topic = txtTopic.Text; // Campo onde o tópico é especificado
                    if (!string.IsNullOrWhiteSpace(topic))
                    {
                        await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                            .WithTopic(topic)
                            .WithAtLeastOnceQoS()
                            .Build());
                        AppendMessage($"Inscrito no tópico: {topic}");
                    }
                    else
                    {
                        AppendMessage("Tópico não fornecido! Inscrição não realizada.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}");
            }

        }
    }
}
