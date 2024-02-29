using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;
using Usings;
using System.Reflection.Metadata;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Linq;
using System;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool IsReserved(string tokenName) => Enum.GetNames(typeof(Initiator)).Contains(StringExtension.Capitalize(tokenName));

        public MainWindow()
        {
            InitializeComponent();
        }

        private void KeyHandler(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                MessageHandler(input.Text);
                input.Text = "";
                scroller.ScrollToBottom();
            }
        }

        private void MessageHandler(string message)
        {
            //remove excess whitespace
            message.Trim();
            Regex regex = new Regex("[ ]{2,}");
            regex.Replace(message, " ");
            string[] split = message.Split(' ');

            List<Token> tokenList = new();
            foreach (string s in split)
            {
                if (IsReserved(s))
                {
                    tokenList.Add(new ReservedToken(StringExtension.Capitalize(s)));
                } else if (message == "?")
                {
                    tokenList.Add(new ReservedToken(Initiator.quest));
                }
                else
                {
                    tokenList.Add(new RawToken(s));
                }
            }

            PushMessage($"> {message}");
            RunTokenSequence(tokenList);

        }

        private async void RunTokenSequence(List<Token> tokens)
        {
            if (tokens.First() is not ReservedToken) return;
           
            Initiator initiator = (Initiator)tokens[0].Data;

            switch (initiator)
            {

                case Initiator.Ping:
                    if (tokens.Count < 2)
                    {
                        PushMessage("Must specify address");
                        return;
                    };
                    RawToken? token = (RawToken)tokens.ElementAt(1)??null;
                    for (int pingNumber = 0; pingNumber < 5; pingNumber++)
                    {
                        HttpClient client = new HttpClient();
                        try
                        {
                            var checkingResponse = await client.GetAsync(
                                token.Data.ToString()!.Contains("http") ?
                                token.Data.ToString() :
                                "http://" + token.Data.ToString()
                            );
                            if (checkingResponse.IsSuccessStatusCode)
                            {
                                PushMessage($"{token.RawAddress} responded with {checkingResponse.StatusCode}");
                            }
                            else
                            {
                                PushMessage($"Failed to ping {token.RawAddress}");
                            }
                        }
                        catch (Exception)
                        {
                            PushMessage($"Error");
                        }

                    }
                    break;

                case Initiator.Cow:
                    PushMessage("           __n__n__\r\n    .------`-\\00/-'\r\n   /  ##  ## (oo)\r\n  / \\## __   ./\r\n     |//YY \\|/\r\n     |||   |||");
                    break;

                case Initiator.quest:
                    PushMessage("helpmessage");
                    break;

                default:
                    break;
            }
        }

        private void PushMessage(string message)
        {
            main.Text += '\n' + message;
        }
    }
}