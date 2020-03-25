using Confluent.Kafka;
using Starwars.Core.Config;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Starwars.Consumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var typeName = typeof(Program).Assembly.GetName();
            Log($"Start {typeName.Name} (v{typeName.Version})");

            var currentConfig = CurrentConfiguration.Build();

            // Create the consumer configuration
            var config = new ConsumerConfig
            {
                GroupId = "starwars-consumer-group",
                BootstrapServers = currentConfig.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };


            Log($"Connect consumer to kafka > {currentConfig.BootstrapServers}");

            // Create the consumer
            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                // Subscribe to the Kafka topic
                consumer.Subscribe(new List<string>() { currentConfig.Topic });

                // Handle Cancel Keypress 
                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };

                Log("Press Ctrl+C to exit");

                // Poll for messages
                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = consumer.Consume(cts.Token);
                            Log($"  >> Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                        }
                        catch (ConsumeException e)
                        {
                            Log($"  >> Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    consumer.Close();
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