﻿using System;
using System.Collections.Generic;
using Banks.UI.Commands;
using Shops.Commands;

namespace Banks
{
    internal class Program
    {
        private static Context _context = new Context();
        private static Dictionary<string, Command> _commands = new Dictionary<string, Command>();

        private static void Main()
        {
            _commands["bank"] = new BankCommand(_context);
            _commands["person"] = new PersonCommand(_context);
            _commands["quit"] = new QuitCommand();
            _commands["time"] = new TimeCommand(_context);

            while (true)
            {
                string[] args = ReadCommand();
                if (args.Length == 0)
                    continue;
                string commandName = args[0].ToLower();
                if (_commands.TryGetValue(commandName, out Command command))
                {
                    try
                    {
                        CommandResponse response = command.ProcessCommand(args);
                        foreach (string responseLine in response.Lines)
                            Console.WriteLine(responseLine);
                        if (response.ShouldExit)
                            break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Unknown command. Try to use one of them:");
                    foreach (string commandsKey in _commands.Keys)
                    {
                        Console.WriteLine(commandsKey);
                    }
                }
            }
        }

        private static string[] ReadCommand()
        {
            string line = null;
            while (line == null)
                line = Console.ReadLine();
            char[] chars = line.ToCharArray();

            var list = new List<string>();

            char[] charsCache = new char[chars.Length];
            int cacheIndex = 0;

            bool passSpace = false;

            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (c == ' ' && !passSpace)
                {
                    if (cacheIndex != 0)
                    {
                        list.Add(new string(charsCache, 0, cacheIndex));
                        cacheIndex = 0;
                    }
                }
                else if (c == '"')
                {
                    passSpace = !passSpace;
                }
                else
                {
                    charsCache[cacheIndex++] = c;
                }
            }

            if (cacheIndex != 0)
                list.Add(new string(charsCache, 0, cacheIndex));

            return list.ToArray();
        }
    }
}