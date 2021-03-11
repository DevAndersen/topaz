using DevAndersen.Topaz.Common;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;

namespace DevAndersen.Topaz.Client
{
    class Client
    {
        static void Main(string[] args)
        {
            // If args[0] does not start with '#', base64 encode it. If not, move on with args[0] (skipping the '#'), under the assumption that it is a base64 encoded string.
            string rawInput = args.Length > 0 ? args[0] : string.Empty;
            string input = rawInput.StartsWith('#')
                ? rawInput[1..]
                : Convert.ToBase64String(Encoding.UTF8.GetBytes(rawInput));

            using NamedPipeClientStream pipe = new NamedPipeClientStream(General.defaultPipeName);
            try
            {
                pipe.Connect(100);
                using StreamReader reader = new StreamReader(pipe);
                using StreamWriter writer = new StreamWriter(pipe);
                writer.WriteLine(input);
                writer.Flush();
                Console.WriteLine(reader.ReadLine());
            }
            catch (TimeoutException)
            {
                Console.WriteLine(JsonSerializer.Serialize(new Message
                {
                    MessageType = MessageType.Error,
                    Data = "Server not running."
                }));
            }
            catch (Exception e)
            {
                Console.WriteLine(JsonSerializer.Serialize(new Message
                {
                    MessageType = MessageType.Error,
                    Data = e.Message
                }));
            }
        }
    }
}
