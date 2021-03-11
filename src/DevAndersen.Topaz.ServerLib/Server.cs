using DevAndersen.Topaz.Client;
using DevAndersen.Topaz.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DevAndersen.Topaz.ServerLib
{
    public class Server
    {
        public string PipeName { get; init; }
        public bool IsRunning { get; private set; }
        public MessageSender MessageSender { get; init; }

        private NamedPipeServerStream pipe;
        private StreamReader reader;
        private StreamWriter writer;

        private readonly List<Command> commandList;

        public Server(string pipeName)
        {
            PipeName = pipeName;
            MessageSender = new MessageSender(this);

            commandList = new List<Command>();
        }

        public Server() : this(General.defaultPipeName)
        {
        }

        public void RegisterCommands(params Command[] commands)
        {
            foreach (Command command in commands)
            {
                commandList.Add(command);
            }
        }

        // Todo: Rewrite to support cancellation of shortcut.
        public void Start()
        {
            IsRunning = true;
            using (pipe = new NamedPipeServerStream(PipeName))
            {
                WriteLog(LogContext.System, $"Starting server '{PipeName}'.");
                WriteLog(LogContext.System, $"Registered commands: {commandList.Count}.");

                reader = new StreamReader(pipe);
                writer = new StreamWriter(pipe);

                while (IsRunning)
                {
                    pipe.WaitForConnection();
                    string input = ReadSingle();

                    bool matchFound = false;
                    foreach (Command command in commandList)
                    {
                        Match match = Regex.Match(input, command.RegexPattern, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            matchFound = true;

                            bool sendExit = command.CommandFunc.Invoke(match);
                            if (IsRunning && sendExit)
                            {
                                SendMessagesWithoutRead(MessageType.Exit);
                            }
                            break;
                        }
                    }

                    if (!matchFound)
                    {
                        SendMessagesWithoutRead(new Message(MessageType.Error, "No matching command found."));
                    }
                }

                if (pipe.IsConnected)
                {
                    pipe.Disconnect();
                    writer?.Dispose();
                }
                reader?.Dispose();
            }
            WriteLog(LogContext.System, $"Server '{PipeName}' shutting down.");
        }

        public void Stop()
        {
            IsRunning = false;
            SendMessagesWithoutRead(MessageType.Exit);
            pipe.Close();
        }

        public string[] Read()
        {
            string[] inputs = reader.ReadLine()
                .Split('|')
                .Select(base64String => Encoding.UTF8.GetString(Convert.FromBase64String(base64String)))
                .ToArray();

            string logMessage = string.Join("' | '", inputs);
            WriteLog(LogContext.Client, $"'{logMessage}'");
            return inputs;
        }

        public string ReadSingle()
        {
            return Read().FirstOrDefault();
        }

        public void Write(string text)
        {
            writer.WriteLine(text);
            WriteLog(LogContext.Server, text);
        }

        // Todo: Implement a real logging system, this is just a placeholder for debugging purposes.
        public static void WriteLog(LogContext context, string input)
        {
            int lengthLimit = 256;
            if (input.Length > lengthLimit)
            {
                input = input.Substring(0, lengthLimit) + " [...]";
            }

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{context}] {input}");
        }

        // Todo: Cut down on SendMessage overloads.
        public string SendMessage(MessageType messageType, string data)
        {
            return SendMessages(new Message(messageType, data)).FirstOrDefault();
        }

        public string SendMessage(MessageType messageType, string data, string defaultValue)
        {
            return SendMessages(new Message(messageType, data, defaultValue)).FirstOrDefault();
        }

        public string SendMessage(Message message)
        {
            return SendMessages(message).FirstOrDefault();
        }

        public string[] SendMessages(params Message[] messages)
        {
            return SendMessages(messages, true);
        }

        public void SendMessagesWithoutRead(params Message[] messages)
        {
            SendMessages(messages, false);
        }

        private string[] SendMessages(Message[] messages, bool waitForResponse = true)
        {
            Write(JsonSerializer.Serialize(messages));

            try
            {
                writer.Flush();
                pipe.WaitForPipeDrain();
            }
            catch (IOException e)
            {
                WriteLog(LogContext.Error, e.Message);
            }
            finally
            {
                pipe.Disconnect();
            }

            if (waitForResponse)
            {
                pipe.WaitForConnection();
                return Read();
            }
            else
            {
                return null;
            }
        }
    }
}
