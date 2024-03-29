﻿using System.Windows;
using System.Windows.Input;
using Usings;
using System.Net.Http;
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Prefix.Text = currentDirectory.Path + ">";
        }
        /*
        private List<string> messageQueue = new List<string>();
        */
        private DirectoryHandler currentDirectory = new DirectoryHandler(Environment.CurrentDirectory);

        public bool IsReserved(string tokenName) => Enum.GetNames(typeof(Initiator)).Contains(StringExtension.Capitalize(tokenName));
        public Dictionary<string, string> helpMessages =
        new(){
            {
                "default",
                $"  The following commands can be used:" + '\n' +
                $"\t{ string.Join(", ", Enum.GetNames<Initiator>()).Replace(Usings.Initiator.QuestionMark.ToString() , "?").ToLower() }" + '\n' +
                $"  For help with a specific command, type command name after '?'." + '\n' +
                $"  Example: '? ping' - displays information about the 'ping' token"

            },
            {
                "ping", 
                "  ping [Uri] [Count?]" + '\n' +
                "  Sends a GET request to the specified Uri, and displays the request's status. " + '\n' +
                "  Optional Count token specifies how many requests should be sent. Count must be less than or equal to 10. Default: 1" + '\n' +
                "  Examples:" + '\n' +
                "\tping google.com" + '\n' +
                "\tping https://yahoo.com" + '\n' +
                "\tping https://youtube.com 5"
            },
            {
                "weevil",
                "  Displays ASCII art of an acorn weevil."
            }
        };

        private void KeyHandler(object sender, KeyEventArgs e)
        {
            string message = InputField.Text ;
            if (message.Trim() == string.Empty) return;
            switch (e.Key)
            {
                case Key.Enter:
                    MessageHandler(message);
                    /*
                    messageQueue.Add(message);
                    if (messageQueue.Count > 10) 
                        messageQueue.RemoveAt(0);
                    PushMessage($"{message} added. messageQueue: {string.Join(',', messageQueue)}");
                    */
                    Scroller.ScrollToBottom();
                    break;
                    /*
                case Key.Up:
                    if (messageQueue.Contains(message))
                    {
                        if(messageQueue.IndexOf(message) > 0) 
                            InputField.Text = messageQueue.ElementAt(messageQueue.IndexOf(message) - 1);
                    }
                    else if(messageQueue.Count > 0)
                    {
                        InputField.Text = messageQueue.Last();
                    }
                    break;
                case Key.Down:
                    if (messageQueue.Contains(message))
                    {
                        if (messageQueue.IndexOf(message) < messageQueue.Count - 1)
                            InputField.Text = messageQueue.ElementAt(messageQueue.IndexOf(message) + 1);
                        else
                            InputField.Text = string.Empty;
                    }
                    else if (messageQueue.Count > 0)
                    {
                        InputField.Text = string.Empty;
                    }
                    break;
                    */
                default:
                    break;
            }
            InputField.Text = InputField.Text.Trim();
        }

        private void MessageHandler(string message)
        {
            message = StringExtension.FormatTokenChain(message);
            string[] split = message.Split(' ');
            if (message == string.Empty) return;

            List<Token> tokenList = new();
            foreach (string s in split)
            {
                if (IsReserved(s))
                {
                    tokenList.Add(new ReservedToken(StringExtension.Capitalize(s)));
                } else if (s == "?")
                {
                    tokenList.Add(new ReservedToken(Initiator.QuestionMark));
                }
                else
                {
                    tokenList.Add(new RawToken(s));
                }
            }

            PushMessage($"{message}");
            RunTokenSequence(tokenList);

            Prefix.Text = currentDirectory.Path + ">";
            InputField.Text = "";
        }

        private async void RunTokenSequence(List<Token> tokens)
        {
            if (tokens.First() is not ReservedToken)
            {
                PushMessage($"Unrecognized token: '{tokens.First().Data}'");
                return;
            }
           
            Initiator initiator = (Initiator)tokens[0].Data;

            switch (initiator)
            {

                case Initiator.Ping:
                    if (tokens.Count == 1)
                    {
                        PushMessage("Must specify address");
                        return;
                    };
                    int? pingCount;
                    try
                    {
                        pingCount = int.Parse((string)tokens[2].Data);
                        if(pingCount < 0)
                        {
                            PushMessage("Ping amount must be positive integer");
                            return;
                        }
                        if(pingCount > 10)
                        {
                            PushMessage("Ping amount must be less than or equal to 10");
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        pingCount = null;
                    }
                    RawToken addressToken = (RawToken)tokens.ElementAt(1);
                    for (int pingNumber = 0; pingNumber < (pingCount??1); pingNumber++)
                    {
                        HttpClient client = new HttpClient();
                        try
                        {
                            var checkingResponse = await client.GetAsync(
                                addressToken.Data.ToString()!.Contains("http") ?
                                (string)addressToken.Data :
                                "http://" + addressToken.Data
                            );
                            if (checkingResponse.IsSuccessStatusCode && checkingResponse != null)
                            {
                                PushMessage($"{addressToken.RawAddress} responded with {checkingResponse.StatusCode}");
                            }
                            else
                            {
                                PushMessage($"Failed to ping {addressToken.RawAddress}");
                            }
                        }
                        catch (HttpRequestException)
                        {
                            PushMessage("Address unreachable");
                        }
                    }
                    break;


                case Initiator.Weevil:
                    PushMessage("\r\n                                       .-\r\n                        ..-=+-:--::     -:\r\n                   ..:..:--:--=*-=-==: .++.\r\n                  .:::  ::::::=::+@@##-=%%:\r\n                 .  ....:-=:--=%%%@%@@=%@@  :.\r\n                ..::::::-=+++=+%%**%@@+##:.+#%+\r\n                 .:-::-=+###%###@@%#*@*+.-*# -@#\r\n              ==-:--++==+*%@@#@%@@@@%@%%%@%=  *@=\r\n              .:--*++=--+%@@@@@@%@%@@%@@@#--   -@:\r\n              .+.:+*@#*++@@@@@@%#@@@@@@%.       +@:\r\n               *#:-+#@@+=@%%@@%%@@@@@@=          %@:\r\n               .@*.-=#*#=%%@@#%#*#@%*+.:.  ..    +%*\r\n                -- .-+:+=##@##%@@%##*###+++--=+@+ :---=-.\r\n             .::   .##-..++.(¤).:      .-==*+.=@+     .%#=-\r\n            ...-+#%#.(¤).+# ::                *%\r\n         +*--++=-::-**-. -@=.                 -@\r\n         :@+  ..:=+#%-...-@%                  -@:\r\n          .#*+. -*%*      *@:                 :@:\r\n           :- =#%%-        %%                 :@:\r\n          :.-=%#*.         .@=                :%:\r\n         -.=*#%@=           =@-               =@:\r\n       .:   .-#@@@:          *@:              -@+\r\n      :.        :%##+-        *@.               :*+\r\n     -           : .-*%#+-...  -.                .+#-.\r\n   --          :*:     :-+#@+                     :=+-\r\n  =*         ::..          =*                       .\r\n  .                        :+\r\n                           -%\r\n                          +*-\r\n                         +");
                    break;
                case Initiator.QuestionMark:
                    if (tokens.Count == 1)
                    {
                        PushMessage(helpMessages["default"]);
                        return;
                    }
                    string key = tokens[1].Data.ToString()!.ToLower();
                    if (helpMessages.ContainsKey(key))
                    {
                        PushMessage(helpMessages[key]);
                        return;
                    }
                    PushMessage($"Token '{key}' does not exist.");
                    break;

                case Initiator.Cd:
                    if(tokens.Count == 1)
                    {
                        currentDirectory.Path = new DirectoryInfo(Environment.CurrentDirectory);
                        break;
                    } else if(tokens.Count == 2)
                    {
                        currentDirectory.MoveIn(tokens.ElementAt(2).Data.ToString()??"");
                    }
                    break;

                default:
                    break;
            }

        }

        private void PushMessage(string message)
        {
            Main.Text += $"\n{message}";
        }
    }
}