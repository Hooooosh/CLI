
using System.Text.RegularExpressions;

namespace Usings
{
    //
    // commands
    //
    // ping - pings http address, checks for response
    // weevil - acorn weevil ascii
    //

    public enum Initiator
    {
        Ping,
        Cow,
        quest,

    }
    
    public abstract class Token
    {
        public abstract object Data { get; protected set; }
    }

    public class ReservedToken : Token
    {
        public override object Data { get; protected set; }
        public ReservedToken(string data)
        {
            this.Data = (Initiator)Enum.Parse<Initiator>(StringExtension.Capitalize(data));
        }
        public ReservedToken(Initiator init)
        {
            this.Data = init;
        }
    }

    public class RawToken : Token
    {
        public override object Data { get; protected set; }
        public RawToken(string data) => this.Data = data;
        public RawToken() => this.Data = string.Empty;
        public string RawAddress
        {
            get
            {
                Regex r = new Regex("https?://|/$");
                return r.Replace((string)Data, "");
            }
        }
    }
}
