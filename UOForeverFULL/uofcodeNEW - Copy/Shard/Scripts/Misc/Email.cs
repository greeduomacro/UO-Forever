using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Server;
using Server.Accounting;
using System.IO;

namespace Server.Misc
{
	public class Email
	{
		/* In order to support emailing, fill in EmailServer:
		 * Example:
		 *  public static readonly string EmailServer = "mail.domain.com";
		 *
		 * If you want to add crash reporting emailing, fill in CrashAddresses:
		 * Example:
		 *  public static readonly string CrashAddresses = "first@email.here;second@email.here;third@email.here";
		 *
		 * If you want to add speech log page emailing, fill in SpeechLogPageAddresses:
		 * Example:
		 *  public static readonly string SpeechLogPageAddresses = "first@email.here;second@email.here;third@email.here";
		 */


        // Alan Mod: use config file to add these settings
        // EmailConfig.txt should have the following format:
        /*
        
        EmailServer: smtp.gmail.com
        Port: 587
        EnableSSL: true
        CrashAddresses: <put the recipient addresses here>
        Username: yourusername@gmail.com
        Password: yourpassword         
        */
		
		[CallPriority(Int32.MinValue)]
        public static void Configure()
        {
            if (!File.Exists("Data/EmailConfig.txt"))
            {
                Console.WriteLine("E-mail disabled (Data\\EmailConfig.txt not found)");
                return;
            }

			try
			{
				string[] lines = File.ReadAllLines("Data/EmailConfig.txt");
				string username = null;
				string password = null;

				foreach (string line in lines)
				{
					string trimmed = line.Trim();
					string lower = trimmed.ToLower();

					if (lower.StartsWith("emailserver:"))
					{
						EmailServer = trimmed.Substring(12).Trim();
					}
					else if (lower.StartsWith("port:"))
					{
						Port = Int32.Parse(trimmed.Substring(5));
					}
					else if (lower.StartsWith("enablessl:"))
					{
						EnableSSL = trimmed.Substring(10).Trim() == "true";
					}
					else if (lower.StartsWith("crashaddresses:"))
					{
						CrashAddresses = trimmed.Substring(15).Trim();
					}
					else if (lower.StartsWith("phonealertaddresses:"))
					{
						PhoneAlertAddresses = trimmed.Substring(20).Trim();
					}
					else if (lower.StartsWith("username:"))
					{
						username = trimmed.Substring(9).Trim();
					}
					else if (lower.StartsWith("password:"))
					{
						password = trimmed.Substring(9).Trim();
					}
				}

				if (username == null || password == null)
				{
					throw new Exception("Data\\EmailConfig.txt did not contains username or password! These are required!");
				}

				m_SmtpCredentials = new NetworkCredential(username, password);

				if (!String.IsNullOrEmpty(EmailServer))
				{
					_Client = new SmtpClient(EmailServer, Port) {
						UseDefaultCredentials = false,
						EnableSsl = EnableSSL,
						Timeout = 30000
					};

					ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
				}

				Console.WriteLine("Email configuration complete from Data\\EmailConfig.txt");

				if (!TestCenter.Enabled)
				{
					CrashGuard.CSOptions.Email = true;
					CrashGuard.CSOptions.ReportAttach = true;
					CrashGuard.CSOptions.EmailOptions.Host = EmailServer;
					CrashGuard.CSOptions.EmailOptions.Port = Port;
					CrashGuard.CSOptions.EmailOptions.Auth = true;
					CrashGuard.CSOptions.EmailOptions.User = username;
					CrashGuard.CSOptions.EmailOptions.Pass = password;
					CrashGuard.CSOptions.EmailOptions.From = "UO Server Watch <uo.server.watch@gmail.com>";
					CrashGuard.CSOptions.EmailOptions.To = CrashAddresses + ";" + PhoneAlertAddresses;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Loading Data/EmailConfig.txt failed!: " + e.Message + "\n" + e.StackTrace);
			}
        }
        // end Alan Mod

		public static string EmailServer = "";
		public static int Port = 25;
		public static bool EnableSSL = true;

		public static string CrashAddresses = "";
        public static string PhoneAlertAddresses = "";
		public static string SpeechLogPageAddresses = "";
		private static NetworkCredential m_SmtpCredentials = new NetworkCredential( "", "auth54123" );

		private static Regex _pattern = new Regex( @"^[a-z0-9.+_-]+@([a-z0-9-]+.)+[a-z]+$", RegexOptions.IgnoreCase );

		public static bool IsAvailable( string address )
		{
			address = address.Trim().ToLower();

			foreach ( Account acct in Accounts.GetAccounts() )
				if ( acct.AccessLevel == AccessLevel.Player && acct.Email == address )
					return false;

			return true;
		}

		public static bool IsValid( string address )
		{
			if ( address == null || address.Length > 320 )
				return false;

			return _pattern.IsMatch( address );
		}

		private static SmtpClient _Client;
		
		public static bool Send( MailMessage message )
		{
			return Send( message, m_SmtpCredentials );
		}

		public static bool Send( MailMessage message, ICredentialsByHost credentials )
		{
			try
			{
				lock ( _Client )
				{
					//If this is not specified, we would like to keep it that way.
					_Client.Credentials = credentials;
					_Client.Send( message );
				}
			}
			catch ( Exception e )
			{
				Console.WriteLine( "Email Send: Failure: {0}", e );
				return false;
			}

			return true;
		}

		public static void AsyncSend( MailMessage message )
		{
			AsyncSend( message, m_SmtpCredentials );
		}

		public static void AsyncSend( MailMessage message, ICredentialsByHost credentials )
		{
			ThreadPool.QueueUserWorkItem( new WaitCallback( SendCallback ), new object[]{ message, credentials } );
		}

		private static void SendCallback( object state )
		{
			object[] states = (object[])state;

			MailMessage message = states[0] as MailMessage;
			ICredentialsByHost credentials = states[1] as ICredentialsByHost;

			if ( !Send( message, credentials ) )
			//	Console.WriteLine( "Sent e-mail '{0}' to '{1}'.", message.Subject, message.To );
			//else
				Console.WriteLine( "Failure sending e-mail '{0}' to '{1}'.", message.Subject, message.To );
		}
	}
}