using IMAP.Base.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMailClient.viewmodel
{
    public class inboxviewmodel
    {
        public List<MailMessage> AllMessages { get; set; }
    }

    public class attachementviewmodel
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }

    public class messageinfoviewmodel
    {
        public string uid { get; set; }
    }
}