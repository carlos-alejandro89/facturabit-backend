using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security;
using System.Text;

namespace FacturaBit.Infrastructure.Security.Cryptography
{
    public class OpenSSL
    {
        public CertificateInfo InfoCertificado(string certificatePath)
        {
            using X509Certificate2 objCert = X509CertificateLoader.LoadCertificateFromFile(certificatePath);
            return ObtenerInfoCertificado(objCert);
        }

        public CertificateInfo InfoCertificado(byte[] certificateData)
        {
            using X509Certificate2 objCert = X509CertificateLoader.LoadCertificate(certificateData);
            return ObtenerInfoCertificado(objCert);
        }

        public bool CertificadoYLlaveCorresponden(
            byte[] certificateData,
            byte[] encryptedKeyData,
            string password)
        {
            using X509Certificate2 certificate = X509CertificateLoader.LoadCertificate(certificateData);
            using RSA? publicKey = certificate.GetRSAPublicKey();
            using RSACryptoServiceProvider? privateKey = DecodePrivateKeyInfo(encryptedKeyData, password);

            if (publicKey is null || privateKey is null)
            {
                return false;
            }

            RSAParameters publicParameters = publicKey.ExportParameters(false);
            RSAParameters privateParameters = privateKey.ExportParameters(false);

            return publicParameters.Modulus is not null
                && privateParameters.Modulus is not null
                && publicParameters.Exponent is not null
                && privateParameters.Exponent is not null
                && CryptographicOperations.FixedTimeEquals(publicParameters.Modulus, privateParameters.Modulus)
                && CryptographicOperations.FixedTimeEquals(publicParameters.Exponent, privateParameters.Exponent);
        }

        private static CertificateInfo ObtenerInfoCertificado(X509Certificate2 objCert)
        {
            using RSA? publicKey = objCert.GetRSAPublicKey();

            CertificateExtensionInfo[] extensions = objCert.Extensions
                .Select(extension => new CertificateExtensionInfo(
                    extension.Oid?.FriendlyName,
                    extension.Oid?.Value,
                    extension.Critical,
                    extension.Format(false)))
                .ToArray();

            return new CertificateInfo(
                objCert.Subject,
                objCert.Issuer,
                objCert.NotBefore,
                objCert.NotAfter,
                publicKey?.KeySize,
                objCert.SerialNumber,
                objCert.Thumbprint,
                extensions);
        }

        /*===================================================================================*/

        public string GenerarPFX(string Certificado, string KeyFile, string Pwd, string RutaPFX)
        {

            var _Certificado = new X509Certificate2(Certificado, Pwd);
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)_Certificado.PrivateKey;

            byte[] certificateData = _Certificado.Export(X509ContentType.Pfx, Pwd);

            byte[] DatosEnc = rsa.Encrypt(certificateData, false);
            String DatosEncText = Convert.ToBase64String(DatosEnc);

            var rsa2 = OpenKeyFile(KeyFile, Pwd);

            // rsa.PersistKeyInCsp = true;
            _Certificado.PrivateKey = rsa;



            var base64String = Convert.ToBase64String(certificateData);


            File.WriteAllText(RutaPFX, base64String);
            ExportPrivateKey(rsa, TextWriter.Null);
            return Convert.ToBase64String(certificateData);
        }

        /*======================================================================================*/

        private static void ExportPrivateKey(RSACryptoServiceProvider csp, TextWriter outputStream)
        {
            if (csp.PublicOnly) throw new ArgumentException("CSP does not contain a private key", "csp");
            var parameters = csp.ExportParameters(true);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                    EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                    EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                    EncodeIntegerBigEndian(innerWriter, parameters.D);
                    EncodeIntegerBigEndian(innerWriter, parameters.P);
                    EncodeIntegerBigEndian(innerWriter, parameters.Q);
                    EncodeIntegerBigEndian(innerWriter, parameters.DP);
                    EncodeIntegerBigEndian(innerWriter, parameters.DQ);
                    EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                outputStream.WriteLine("-----BEGIN RSA PRIVATE KEY-----");
                // Output as Base64 with lines chopped at 64 characters
                for (var i = 0; i < base64.Length; i += 64)
                {
                    outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
                }
                outputStream.WriteLine("-----END RSA PRIVATE KEY-----");
            }
        }

        private static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }

        private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    // Add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }

        /*=====================================================================================*/
        public string SignString(string pKeyFile, string pPassword, string OriginalString)
        {
            string SignedString = "";
            string filename = pKeyFile;
            if (!File.Exists(filename))
            {
                return "Archivo .key no existe" + pKeyFile;
            }

            RSACryptoServiceProvider rsa = OpenKeyFile(filename, pPassword);
            if (rsa != null)
            {
                byte[] CO = Encoding.UTF8.GetBytes(OriginalString);
                //Para CFDI 3.2
                //byte[] SignedBytes=rsa.SignData(CO, new SHA1CryptoServiceProvider());

                //Para CFDI 3.3 Y 4.0
                byte[] SignedBytes = rsa.SignData(CO, new SHA256CryptoServiceProvider());

                SignedString = Convert.ToBase64String(SignedBytes);
            }
            return SignedString;
        }

        public RSACryptoServiceProvider OpenKeyFile(String filename, string pPassword)
        {
            RSACryptoServiceProvider rsa = null;
            byte[] keyblob = GetFileBytes(filename);
            if (keyblob == null)
                return null;

            rsa = DecodePrivateKeyInfo(keyblob, pPassword);	//PKCS #8 encrypted
            if (rsa != null)
            {

                return rsa;
            }
            return null;
        }

        public static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] encpkcs8, string pPassword)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            // this byte[] includes the sequence byte and terminal encoded null 
            byte[] OIDpkcs5PBES2 = { 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x05, 0x0D };
            byte[] OIDpkcs5PBKDF2 = { 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x05, 0x0C };
            byte[] OIDdesEDE3CBC = { 0x06, 0x08, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x03, 0x07 };
            byte[] seqdes = new byte[10];
            byte[] seq = new byte[11];
            byte[] salt;
            byte[] IV;
            byte[] encryptedpkcs8;
            byte[] pkcs8;

            int saltsize, ivsize, encblobsize;
            int iterations;

            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            MemoryStream mem = new MemoryStream(encpkcs8);
            int lenstream = (int)mem.Length;
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();	//inner sequence
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();


                seq = binr.ReadBytes(11);		//read the Sequence OID
                if (!CompareBytearrays(seq, OIDpkcs5PBES2))	//is it a OIDpkcs5PBES2 ?
                    return null;

                twobytes = binr.ReadUInt16();	//inner sequence for pswd salt
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();

                twobytes = binr.ReadUInt16();	//inner sequence for pswd salt
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();

                seq = binr.ReadBytes(11);		//read the Sequence OID
                if (!CompareBytearrays(seq, OIDpkcs5PBKDF2))	//is it a OIDpkcs5PBKDF2 ?
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();

                bt = binr.ReadByte();
                if (bt != 0x04)		//expect octet string for salt
                    return null;
                saltsize = binr.ReadByte();
                salt = binr.ReadBytes(saltsize);

                bt = binr.ReadByte();
                if (bt != 0x02) 	//expect an integer for PBKF2 interation count
                    return null;

                int itbytes = binr.ReadByte();	//PBKD2 iterations should fit in 2 bytes.
                if (itbytes == 1)
                    iterations = binr.ReadByte();
                else if (itbytes == 2)
                    iterations = 256 * binr.ReadByte() + binr.ReadByte();
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();


                seqdes = binr.ReadBytes(10);		//read the Sequence OID
                if (!CompareBytearrays(seqdes, OIDdesEDE3CBC))	//is it a OIDdes-EDE3-CBC ?
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x04)		//expect octet string for IV
                    return null;
                ivsize = binr.ReadByte();	// IV byte size should fit in one byte (24 expected for 3DES)
                IV = binr.ReadBytes(ivsize);

                bt = binr.ReadByte();
                if (bt != 0x04)		// expect octet string for encrypted PKCS8 data
                    return null;


                bt = binr.ReadByte();

                if (bt == 0x81)
                    encblobsize = binr.ReadByte();	// data size in next byte
                else if (bt == 0x82)
                    encblobsize = 256 * binr.ReadByte() + binr.ReadByte();
                else
                    encblobsize = bt;		// we already have the data size


                encryptedpkcs8 = binr.ReadBytes(encblobsize);
                SecureString secpswd = new SecureString();
                foreach (char c in pPassword)
                    secpswd.AppendChar(c);

                pkcs8 = DecryptPBDK2(encryptedpkcs8, salt, IV, secpswd, iterations);
                if (pkcs8 == null)	// probably a bad pswd entered.
                    return null;

                RSACryptoServiceProvider rsa = DecodePrivateKeyInfo(pkcs8);
                return rsa;
            }

            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }


        }

        public void CertificateData(string pCerFile, out string Certificate, out string CertificateNumber)
        {
            X509Certificate cert = new X509Certificate(pCerFile);
            byte[] strcert = cert.GetRawCertData();
            Certificate = Convert.ToBase64String(strcert);


            strcert = cert.GetSerialNumber();
            CertificateNumber = Reverse(System.Text.Encoding.UTF8.GetString(strcert));
        }

        public string Reverse(string Original)
        {
            string Reverse = "";
            for (int i = Original.Length - 1; i >= 0; i--)
                Reverse += Original.Substring(i, 1);
            return Reverse;
        }

        private static byte[] GetFileBytes(String filename)
        {
            if (!File.Exists(filename))
                return null;
            Stream stream = new FileStream(filename, FileMode.Open);
            int datalen = (int)stream.Length;
            byte[] filebytes = new byte[datalen];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(filebytes, 0, datalen);
            stream.Close();
            return filebytes;
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        public static byte[] DecryptPBDK2(byte[] edata, byte[] salt,
                  byte[] IV, SecureString secpswd, int iterations)
        {
            CryptoStream decrypt = null;

            IntPtr unmanagedPswd = IntPtr.Zero;
            byte[] psbytes = new byte[secpswd.Length];
            unmanagedPswd = Marshal.SecureStringToGlobalAllocAnsi(secpswd);
            Marshal.Copy(unmanagedPswd, psbytes, 0, psbytes.Length);
            Marshal.ZeroFreeGlobalAllocAnsi(unmanagedPswd);

            try
            {
                Rfc2898DeriveBytes kd = new Rfc2898DeriveBytes(psbytes, salt, iterations);
                TripleDES decAlg = TripleDES.Create();
                decAlg.Key = kd.GetBytes(24);
                decAlg.IV = IV;
                MemoryStream memstr = new MemoryStream();
                decrypt = new CryptoStream(memstr, decAlg.CreateDecryptor(), CryptoStreamMode.Write);
                decrypt.Write(edata, 0, edata.Length);
                decrypt.Flush();
                decrypt.Close();	// this is REQUIRED.
                byte[] cleartext = memstr.ToArray();
                return cleartext;
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem decrypting: {0}", e.Message);
                return null;
            }
        }

        public static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            // this byte[] includes the sequence byte and terminal encoded null 
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            MemoryStream mem = new MemoryStream(pkcs8);
            int lenstream = (int)mem.Length;
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;


                bt = binr.ReadByte();
                if (bt != 0x02)
                    return null;

                twobytes = binr.ReadUInt16();

                if (twobytes != 0x0001)
                    return null;

                seq = binr.ReadBytes(15);		//read the Sequence OID
                if (!CompareBytearrays(seq, SeqOID))	//make sure Sequence for OID is correct
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x04)	//expect an Octet string 
                    return null;

                bt = binr.ReadByte();		//read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
                if (bt == 0x81)
                    binr.ReadByte();
                else
                    if (bt == 0x82)
                    binr.ReadUInt16();
                //------ at this stage, the remaining sequence should be the RSA private key

                byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));
                RSACryptoServiceProvider rsacsp = DecodeRSAPrivateKey(rsaprivkey);
                return rsacsp;
            }

            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }
        }

        public static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)	//version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------  all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                Console.WriteLine("showing components ..");

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally { binr.Close(); }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)		//expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();	// data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }
            while (binr.ReadByte() == 0x00)
            {	//remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }
    }
}
