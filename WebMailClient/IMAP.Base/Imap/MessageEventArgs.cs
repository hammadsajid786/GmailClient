using System;

namespace IMAP.Base.Imap
{
    public class MessageEventArgs : EventArgs {
        public virtual int MessageCount { get; set; }
        internal ImapBase Client { get; set; }
    }
}
