using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMAP.Base.Framework;

namespace IMAP.Base
{
    public interface IImapBase:IDisposable
    {
        int GetTotalMessages();
        MailMessage GetMessage(int index, bool headersonly = false);
        MailMessage GetMessage(string uid, bool headersonly = false);
        void DeleteMessage(string uid);
        void DeleteMessage(MailMessage msg);
        void Disconnect();

        event EventHandler<WarningEventArgs> Warning;
        
    }
}
