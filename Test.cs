using NUnit.Framework;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;

namespace CSharpDemos.Cryptography
{
	[TestFixture ()]
	public class Test
	{
		[Test ()]
		public void EncryptDecryptPfx ()
		{
			//byte [] pfxCertificateBytes = ExtractResource ("MonoCryptography.certificate.pfx");
			//var cert = new X509Certificate2 (pfxCertificateBytes);

			var cert = new X509Certificate2 ("keys/certificate.pfx");


			var rawText = "hello cryptography";
			var encrypted = (cert.PublicKey.Key as RSACryptoServiceProvider).Encrypt (Encoding.ASCII.GetBytes (rawText), false);
			var decrypted = (cert.PrivateKey as RSACryptoServiceProvider).Decrypt (encrypted, false);

			Console.WriteLine ("Raw: {0}", rawText);
			Console.WriteLine ("Encrypted: {0}", Encoding.ASCII.GetString (encrypted));
			Console.WriteLine ("Decrypted: {0}", Encoding.ASCII.GetString (decrypted));

			Assert.AreEqual (rawText, Encoding.ASCII.GetString (decrypted));
		}

		byte [] ExtractResource (String filename)
		{
			System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly ();
			using (Stream resFilestream = a.GetManifestResourceStream (filename)) {
				if (resFilestream == null) return null;
				byte [] ba = new byte [resFilestream.Length];
				resFilestream.Read (ba, 0, ba.Length);
				return ba;
			}
		}
	}


}

