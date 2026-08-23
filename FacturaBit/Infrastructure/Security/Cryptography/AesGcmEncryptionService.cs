using System.Security.Cryptography;
using System.Text;
using FacturaBit.Application.Abstractions;

namespace FacturaBit.Infrastructure.Security.Cryptography;

public sealed class AesGcmEncryptionService : IAesEncryptionService
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private readonly byte[] _key;

    public AesGcmEncryptionService(IConfiguration configuration)
    {
        string secretPhrase = configuration["Cryptography:SecretPhrase"]
            ?? throw new InvalidOperationException("No se configuró Cryptography:SecretPhrase.");

        try
        {
            _key = Convert.FromBase64String(secretPhrase);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException("Cryptography:SecretPhrase debe estar codificada en Base64.", exception);
        }

        if (_key.Length != 32)
        {
            throw new InvalidOperationException("Cryptography:SecretPhrase debe representar una clave de 32 bytes para AES-256.");
        }
    }

    public string Cifrar(string plainText)
    {
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes = new byte[plainBytes.Length];
        byte[] tag = new byte[TagSize];

        using AesGcm aes = new(_key, TagSize);
        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

        byte[] payload = new byte[NonceSize + TagSize + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, payload, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, payload, NonceSize, TagSize);
        Buffer.BlockCopy(cipherBytes, 0, payload, NonceSize + TagSize, cipherBytes.Length);
        return Convert.ToBase64String(payload);
    }

    public string Descifrar(string encryptedText)
    {
        byte[] payload = Convert.FromBase64String(encryptedText);
        if (payload.Length < NonceSize + TagSize)
        {
            throw new CryptographicException("El valor cifrado no tiene un formato válido.");
        }

        ReadOnlySpan<byte> nonce = payload.AsSpan(0, NonceSize);
        ReadOnlySpan<byte> tag = payload.AsSpan(NonceSize, TagSize);
        ReadOnlySpan<byte> cipherBytes = payload.AsSpan(NonceSize + TagSize);
        byte[] plainBytes = new byte[cipherBytes.Length];

        using AesGcm aes = new(_key, TagSize);
        aes.Decrypt(nonce, cipherBytes, tag, plainBytes);
        return Encoding.UTF8.GetString(plainBytes);
    }
}
