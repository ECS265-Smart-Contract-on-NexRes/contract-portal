using System.Security.Cryptography;
using System.Text;

public static class Signature
{
    public static string Sign(string key, string msg)
    {
        // Encode the message using UTF-8
        var data = Encoding.UTF8.GetBytes(msg);

        // Import the RSA key
        var rsa = new RSACryptoServiceProvider();
        rsa.ImportFromPem(key);

        // Create a PKCS#1 v1.5 signature
        var signer = new RSAPKCS1SignatureFormatter(rsa);
        signer.SetHashAlgorithm("SHA256");

        // Compute the SHA-256 hash of the data
        var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(data);

        // Sign the hash
        var signature = signer.CreateSignature(hash);

        // Return the signature as a base64-encoded string
        return Convert.ToBase64String(signature);
    }
}