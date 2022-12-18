using System.Collections.Generic;
using System.IO;

namespace ExternalSort;

public class FileSplitter
{
    private readonly int fileSize;
    private readonly IFileNameProvider fileNameProvider;

    public FileSplitter(int fileSize, IFileNameProvider fileNameProvider)
    {
        this.fileSize = fileSize;
        this.fileNameProvider = fileNameProvider;
    }

    public string[] SplitFile(string filename)
    {
        using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);

        var currentFile = 0;
        var buffer = new byte[fileSize];
        var extraBuffer = new List<byte>(fileSize);
        var files = new List<string>();
        while (stream.Position < stream.Length)
        {
            var bytesRead = 0;
            while (bytesRead < fileSize)
            {
                var read = stream.Read(buffer, bytesRead, fileSize - bytesRead);
                if (read == 0)
                {
                    break;
                }

                bytesRead += read;
            }

            var extraByte = buffer[fileSize - 1];
            while (extraByte != '\n')
            {
                var read = stream.ReadByte();
                if (read == -1)
                {
                    break;
                }

                extraByte = (byte) read;
                if (extraByte != '\n' && extraByte != '\r')
                {
                    extraBuffer.Add(extraByte);
                }
            }

            var unsortedFileName = fileNameProvider.GetUnsortedFileName(currentFile++);
            using var unsortedFile = new FileStream(unsortedFileName, FileMode.OpenOrCreate, FileAccess.Write);
            unsortedFile.Write(buffer, 0, bytesRead);
            if (extraBuffer.Count > 0)
            {
                unsortedFile.Write(extraBuffer.ToArray(), 0, extraBuffer.Count);
            }

            files.Add(unsortedFileName);
            extraBuffer.Clear();
        }

        return files.ToArray();
    }
}
