﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Exceptions
{
    public class FileNotFoundexception:Exception
    {
        public string PropertyName { get; set; }
        public FileNotFoundexception(string propertyName, string? message) : base(message)
        {
            PropertyName = propertyName;
        }
    }
}
