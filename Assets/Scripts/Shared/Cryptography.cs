using DarkRift;
using System.IO;
using System.Text;

public class Cryptography
{
    static byte xorConstant = 0x53;
    public static string Encrypt(string input)
    {
        string output = "";

        byte[] data = Encoding.UTF8.GetBytes(input);
        for (int i = 0; i < data.Length; i++)
            data[i] = (byte)(data[i] ^ xorConstant);

        output = Encoding.UTF8.GetString(data);
        return output;
    }
    public static string Decrypt(string input)
    {
        string output = "";

        byte[] data = Encoding.UTF8.GetBytes(input);
        for (int i = 0; i < data.Length; i++)
            data[i] = (byte)(data[i] ^ xorConstant);

        output = Encoding.UTF8.GetString(data);
        return output;
    }
    public static void CryptFile(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        byte[] data = File.ReadAllBytes(filePath);
        for (int i = 0; i < data.Length; i++)
            data[i] = (byte)(data[i] ^ xorConstant);

        File.Delete(filePath);
        var file = File.Create(filePath);
        file.Close();
        File.WriteAllBytes(filePath, data);
    }
    public static byte[] CryptByteArray(byte[] data)
    {
        for (int i = 0; i < data.Length; i++)
            data[i] = (byte)(data[i] ^ xorConstant);

        return data;
    }

    public static DarkRiftWriter EncryptWriter(DarkRiftWriter writer)
    {
        Message msg = Message.Create(0, writer);
        DarkRiftReader reader = msg.GetReader();

        byte[] data = reader.ReadRaw(reader.Length);
        data = CryptByteArray(data);

        writer = DarkRiftWriter.Create();
        writer.WriteRaw(data, 0, data.Length);

        msg.Dispose();
        reader.Dispose();
        return writer;
    }
    public static DarkRiftReader DecryptReader(DarkRiftReader reader)
    {
        byte[] data = reader.ReadRaw(reader.Length);
        data = CryptByteArray(data);

        DarkRiftWriter writer = DarkRiftWriter.Create();
        writer.WriteRaw(data, 0, data.Length);

        Message msg = Message.Create(0, writer);
        DarkRiftReader newReader = msg.GetReader();

        msg.Dispose();
        writer.Dispose();
        reader.Dispose();

        return newReader;
    }
}