using UnityEngine;
using System.Collections;
using System;
using System.Net;

#if !UNITY_WEBPLAYER
using System.Net.Mail;
using System.IO;
using System.Net.Mime;
#endif
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class Email : Singleton<Email>
{

//Email.Instance.sendEmail("parthp@usc.edu","Subject Test","Testing sending emails\nchecking if new lines works");	
	public void sendEmail (string to, string subject, string body, string file)
	{
#if !UNITY_WEBPLAYER
		MailMessage mail = new MailMessage ();
 
		mail.From = new MailAddress ("SocialCluesAnalytics@gmail.com");
		mail.To.Add (to);
		mail.Subject = subject;
		mail.Body = body;
		

#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		file = Application.persistentDataPath +"/" + file;
#endif
		
		Attachment att = new Attachment (file, MediaTypeNames.Application.Octet);
		// Add time stamp information for the file.
		/*
		 * ContentDisposition disposition = att.ContentDisposition;
		disposition.CreationDate = System.IO.File.GetCreationTime (file);
		disposition.ModificationDate = System.IO.File.GetLastWriteTime (file);
		disposition.ReadDate = System.IO.File.GetLastAccessTime (file);
		*/
		mail.Attachments.Add (att);
		
		SmtpClient smtpServer = new SmtpClient ("smtp.gmail.com");
		smtpServer.Port = 587;
		smtpServer.Credentials = new System.Net.NetworkCredential ("SocialCluesAnalytics@gmail.com", "SocialClues@USC") as ICredentialsByHost;
		smtpServer.EnableSsl = true;
		ServicePointManager.ServerCertificateValidationCallback =
delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true; 
		};
		smtpServer.Send (mail);
		att.Dispose ();
#endif
	}
}
