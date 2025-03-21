﻿using Telegram.Bot;
using TelegramBotBase.Builder;
using TelegramBotBase.Commands;

namespace FileWatcher
{
    internal class Program
    {
        public static Model.Config Config { get; set; }

        public static TelegramBotBase.BotBase Bot { get; set; }

        static void Main(string[] args)
        {

            Config = Model.Config.Load();

            if (string.IsNullOrEmpty(Config.APIKey))
            {
                Console.WriteLine("No API Key set");
                return;
            }

            if (string.IsNullOrEmpty(Config.DirectoryToWatch))
            {
                Console.WriteLine("No directory set");
                return;
            }

            FileSystemWatcher watcher = new FileSystemWatcher(Config.DirectoryToWatch);
            watcher.IncludeSubdirectories = false;

            Console.WriteLine($"Directory: {Config.DirectoryToWatch}");



            Bot = BotBaseBuilder.Create()
                .WithAPIKey(Config.APIKey)
                .DefaultMessageLoop()
                .WithStartForm<Forms.Start>()
                .NoProxy()
                .CustomCommands(a =>
                {
                    a.Start("Starts the bot");
                    a.Add("myid", "Whats my id?");

                })
                .NoSerialization()
                .UseGerman()
                .UseSingleThread()
                .Build();

            Bot.BotCommand += Bot_BotCommand;

            Bot.UploadBotCommands();

            Bot.Start();

            watcher.EnableRaisingEvents = true;

            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;

            Console.WriteLine("Bot started.");


            Console.ReadLine();

            watcher.EnableRaisingEvents = false;

            Bot.Stop();

        }



        private static async Task Bot_BotCommand(object sender, TelegramBotBase.Args.BotCommandEventArgs e)
        {
            switch (e.Command)
            {
                case "/myid":

                    await e.Device.Send($"Your ID is: {e.DeviceId}");

                    e.Handled = true;

                    break;


            }
        }

        private static async void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File '{e.Name}' changed");

            foreach (var device in Config.DeviceIds)
            {
                await Bot.Client.TelegramClient.SendTextMessageAsync(device, $"File '{e.Name}' changed");
            }
        }

        private static async void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File '{e.Name}' created");

            foreach (var device in Config.DeviceIds)
            {
                await Bot.Client.TelegramClient.SendTextMessageAsync(device, $"File '{e.Name}' created");
            }
        }

        private static async void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"File '{e.Name}' renamed");

            foreach (var device in Config.DeviceIds)
            {
                await Bot.Client.TelegramClient.SendTextMessageAsync(device, $"File '{e.Name}' renamed");
            }
        }
    }
}
