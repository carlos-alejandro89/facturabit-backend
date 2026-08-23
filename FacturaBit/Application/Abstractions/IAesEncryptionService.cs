namespace FacturaBit.Application.Abstractions;

public interface IAesEncryptionService
{
    string Cifrar(string plainText);
    string Descifrar(string encryptedText);
}
