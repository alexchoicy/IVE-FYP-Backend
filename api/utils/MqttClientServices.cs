using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace api.utils
{
    public class MqttClientservices
    {
        public readonly IMqttClient mqttClient;
        private readonly MqttClientOptions mqttconfig;

        private readonly string ClientId;

        public MqttClientservices(IConfiguration configuration){
            ClientId = configuration.GetValue<string>("MqttClientOptions:ClientId") ?? Guid.NewGuid().ToString();

            mqttconfig = new MqttClientOptionsBuilder()
                .WithTcpServer(configuration.GetValue<string>("MqttClientOptions:ServerIP"), configuration.GetValue<int>("MqttClientOptions:Port"))
                .WithClientId(ClientId)
                .Build();

            mqttClient = new MqttFactory().CreateMqttClient();

            mqttClient.ApplicationMessageReceivedAsync += messageReceivedhandler;
        }

        public async Task StartAync()
        {
            MqttClientConnectResult results = await mqttClient.ConnectAsync(mqttconfig);
            if (results.ResultCode != MqttClientConnectResultCode.Success)
            {
                Console.WriteLine("Error while connecting to mqtt");
            } else {
                Console.WriteLine("Connected to mqtt");
            }

            MqttClientSubscribeOptions mqttSubscribeOptions = new MqttFactory().CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f => f.WithTopic("plate")
                )
                .Build();


            MqttClientSubscribeResult response = await mqttClient.SubscribeAsync(mqttSubscribeOptions);

            if (response.Items.Any(x => x.ResultCode == MqttClientSubscribeResultCode.GrantedQoS0))
            {
                Console.WriteLine("Subscribed to topic");
            }
            else
            {
                Console.WriteLine("Error while subscribing to topic");
            }

        }

        public async Task StopAsync()
        {
            await mqttClient.DisconnectAsync();
        }

        public Task messageReceivedhandler(MqttApplicationMessageReceivedEventArgs e)
        {
            // The client will listen to the message that send by itself
                Console.WriteLine("Received application message.");
                string topic = e.ApplicationMessage.Topic;
                string message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                Console.WriteLine(message);
                return Task.CompletedTask;
        }

        public async Task PublishAsync(string topic, string message){
            MqttApplicationMessage mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message)
                .Build();

            await mqttClient.PublishAsync(mqttMessage);
        }

    }
}