using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Attachment
    {
        public Guid Id { get;  set; }
        public FileType Type { get;  set; }
        public string FilePath { get;  set; }
        public DateTime UploadedAt { get;  set; }

    }
}
