using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.utils
{
    public class MqttHostedService : IHostedService
    {
        private readonly MqttClientservices mqttClientservices;


        public MqttHostedService(MqttClientservices mqttClientservices)
        {
            this.mqttClientservices = mqttClientservices;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return mqttClientservices.StartAync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return mqttClientservices.StopAsync();
        }
    }
}