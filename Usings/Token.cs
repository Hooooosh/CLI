﻿
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
        QuestionMark,
        Ping,
        Weevil,
        Cd
    }
    
    public abstract class Token
    {
        public abstract object Data { get; protected set; }
    }

    public class ReservedToken : Token
    {
        public Initiator data;
        public override object Data {
            get
            {
                return this.data;
            } 
            protected set
            {
                this.data = (Initiator)value;
            }
        }
        public ReservedToken(string data)
        {
            this.Data = Enum.Parse<Initiator>(StringExtension.Capitalize(data));
        }
        public ReservedToken(Initiator init)
        {
            this.Data = init;
        }
    }

    public class RawToken : Token
    {
        public string? data;
        public override object Data { 
            get {
                return data??"";
            } 
            protected set {
                data = (string)value;
            } 
        }
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
