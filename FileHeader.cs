using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public class FileHeader
    {
        private string _name;
        private string _extension;
        private long _size;
        private string _checksumAlgorithm;
        private string _checksum;

        public FileHeader()
        {

        }

        public void setFileHeader(string fileHeader)
        {
            string[] header = fileHeader.Split(":");
            _name = header[1];
            _extension = header[2];
            _size = long.Parse(header[3]);
            _checksumAlgorithm = header[4];
            _checksum = header[5];

        }

        public string getFileHeader(string filePath)
        {
            _name = new FileInfo(filePath).Name;
            _extension = Path.GetExtension(filePath).Replace(".", "");
            _size = new FileInfo(filePath).Length;
            _checksumAlgorithm = Config.checkSumAlgo;
            _checksum = FileHelper.CalculateCheckSum(filePath);
            return ToString();
        }


        public override string ToString()
        {
            return "FileHeader:" + _name +
                ":" + _extension + //TODO change to date modified or add it 
                ":" + _size +
                ":" + _checksumAlgorithm +
                ":" + _checksum;
        }

        public string getName()
        {
            return _name;
        }

        public string getExtension()
        {
            return _extension;
        }

        public long getSize()
        {
            return _size;
        }

        public string getCheckSum()
        {
            return _checksum;
        }

        public string getCheckSumAlgo()
        {
            return _checksumAlgorithm;
        }


    }

}
