using Confluent.Kafka;
using Starwars.Core.Config;
using System;
using System.Threading.Tasks;

namespace Starwars.Producer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var typeName = typeof(Program).Assembly.GetName();
            Log($"Start {typeName.Name} (v{typeName.Version})");

            var currentConfig = CurrentConfiguration.Build();

            var config = new ProducerConfig
            {
                BootstrapServers = currentConfig.BootstrapServers
            };

            Log($"Connect to kafka > {currentConfig.BootstrapServers}");

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var topic = currentConfig.Topic;
                try
                {
                    for (int i = 0; i < 5; i++)
                    {
                        var message = $"May the Force be with you {i}";
                        
                        var deliveryResult = await producer.ProduceAsync(topic, new Message<Null, string> { 
                                                Value = message
                        });

                        Log($"  >> Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
                    }

                    // wait for up to 10 seconds for any inflight messages to be delivered.
                    producer.Flush(TimeSpan.FromSeconds(10));
                }
                catch (ProduceException<Null, string> e)
                {
                    Log($"  >> Delivery failed: {e.Error.Reason}");
                }

            }
        }

        private static void Log(string message)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
