using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public class FileHeader
    {
        private string _name;
        private DateTime _dateModified;
        private long _size;
        private string _checksumAlgorithm;
        private string _checksum;
        private readonly string _dateFormat = "g";//"yyyy/MM/dd HH:mm:ss"; //TODO check the different formats and which to use. D f g etc.
        public FileHeader()
        {

        }

        public void setFileHeader(string fileHeader)
        {
            //string[] header = fileHeader.Split(":");
            string[] header = fileHeader.Split(Config.unitSeperator);
            _name = header[1];
            _dateModified = DateTime.Parse(header[2]);// DateTime.ParseExact(header[2], _dateFormat, Config.cultureInfo);
            _size = long.Parse(header[3]);
            _checksumAlgorithm = header[4];
            _checksum = header[5];

        }

        public string getFileHeader(string filePath)
        {
            _name = new FileInfo(filePath).Name;
            _dateModified = File.GetLastWriteTime(filePath);
            _size = new FileInfo(filePath).Length;
            _checksumAlgorithm = Config.checkSumAlgo;
            _checksum = FileHelper.CalculateCheckSum(filePath);
            return ToString();
        }


        public override string ToString()
        {
            //return "FileHeader:" + _name +
            //    ":" + _dateModified + //TODO change to date modified or add it 
            //    ":" + _size +
            //    ":" + _checksumAlgorithm +
            //    ":" + _checksum;

            return "FileHeader" + Config.unitSeperator +
                _name + Config.unitSeperator + 
                _dateModified + Config.unitSeperator + //TODO change to date modified or add it 
                _size + Config.unitSeperator +
                _checksumAlgorithm + Config.unitSeperator +
               _checksum;
        }

        public string getName()
        {
            return _name;
        }

        public DateTime getDateModified()
        {
            return _dateModified;
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
