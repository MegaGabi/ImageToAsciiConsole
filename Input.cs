using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToConsoleASCIIart
{
    class Input
    {
        public string FileName { get; private set; }
        public command_type LastResult { get; private set; }
        public enum command_type {load, save, saveb, savec, size, exit, nonsence};
        public enum save_type { bw, colored };

        public int MaxSize = 0;

        public void ProcessInput(string command)
        {
            LastResult = CommandClassification(command);
        }

        command_type CommandClassification(string command)
        {
            command_type tcommand;

            if (command.StartsWith("load"))
            {
                tcommand = command_type.load;
                FileName = command.Substring(command.IndexOf(" ")+1);
            }
            else if (command.StartsWith("saveb"))
            {
                tcommand = command_type.saveb;
            }
            else if (command.StartsWith("savec"))
            {
                tcommand = command_type.savec;
            }
            else if (command.StartsWith("save"))
            {
                tcommand = command_type.save;                
            }
            else if (command.StartsWith("exit"))
            {
                tcommand = command_type.exit;
            }
            else if (command.StartsWith("size"))
            {
                tcommand = command_type.size;

                string buf = command.Substring(command.IndexOf(" ") + 1);

                try
                {
                    MaxSize = int.Parse(buf);
                }
                catch
                {
                    MaxSize = 0;
                }
            }
            else
                tcommand = command_type.nonsence;

            return tcommand;
        }
    }
}
