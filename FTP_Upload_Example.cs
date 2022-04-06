using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FileSync
{
    public class FTPUpload
    {
        public static async Task Upload()
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://www.contoso.com/test.htm");
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");

            // Copy the contents of the file to the request stream.
            await using FileStream fileStream = File.Open("testfile.txt", FileMode.Open, FileAccess.Read);
            await using Stream requestStream = request.GetRequestStream();
            await fileStream.CopyToAsync(requestStream);

            using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
        }
    }
}