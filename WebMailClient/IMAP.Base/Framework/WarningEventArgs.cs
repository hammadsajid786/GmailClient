using System;

namespace IMAP.Base.Framework
{
	public class WarningEventArgs : EventArgs {
		public string Message { get; set; }
		public MailMessage MailMessage { get; set; }
	}
}
