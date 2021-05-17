using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Common
{
    public class FileModel
    {
        public string Id { get; set; }
        public string FileType { get; set; }
        public byte[] Data { get; set; }
        public string FileName { get; set; }
    }
}
