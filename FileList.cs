using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public enum MODIFYTYPE
    {
        DELETE,
        RENAME
    }


    public class FileChanged
    {
        string _fileNameOld { get; set; }
        string _fileNameNew { get; set; }
        MODIFYTYPE _type { get; set; }

        public FileChanged(MODIFYTYPE type, string fileNameOld, string fileNameNew = "" ) {
            _type = type;
            _fileNameOld = fileNameOld;
            _fileNameNew = fileNameNew;            
        }                    

        public string getFileNameOld()
        {
            return _fileNameOld;
        }

        public string getFileNameNew()
        {
            return _fileNameNew;
        }

        public MODIFYTYPE getType()
        {
            return _type;
        }

    }
    
    public class FileList
    {
        public List<FileChanged> filesChanged { get; }

        public FileList() {
            filesChanged = new List<FileChanged>();
        }
        public void Add( FileChanged file )
        {  
            filesChanged.Add( file );
        }

        public void Remove( FileChanged file ) 
        { 
            filesChanged.Remove( file );
        }

        public int Count()
        {
            if (filesChanged == null ) {
                return 0;
            }
            return filesChanged.Count;
        }

        public List<FileChanged> GetList() { 
        return filesChanged;
        }

    }
}
