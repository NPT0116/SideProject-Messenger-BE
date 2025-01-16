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
        public Guid Id { get; private set; }
        public FileType Type { get; private set; }
        public string FilePath { get; private set; }
        public DateTime UploadedAt { get; private set; }

    }
}
