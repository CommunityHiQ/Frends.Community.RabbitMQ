using System.IO;
using System;
using System.IO;
using System.IO.Compression;

namespace Frends.Community.RabbitMQ
{
    public class MessageCompresser
    {
        var compressedFileStream = new MemoryStream();
            using (compressedFileStream)
        {
            //compressedFileStream.Seek(0, SeekOrigin.Begin);
            using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
            {
                foreach (var caseAttachmentModel in archiveDataSet)
                {
                    //Create a zip entry for each attachment
                    var zipEntry = zipArchive.CreateEntry(caseAttachmentModel.name);

                    //Get the stream of the attachment
                    using (var originalFileStream = new MemoryStream(caseAttachmentModel.body))
                    {
                        using (var zipEntryStream = zipEntry.Open())
                        {
                            //Copy the attachment stream to the zip entry stream
                            originalFileStream.CopyTo(zipEntryStream);
                        }
                    }
                }
            }
        }
        zipthis = compressedFileStream.ToArray();
    }
}