using System;
using System.Collections.Generic;
using System.ComponentModel;
using WPFEmpresaEPM.Classes;

namespace WPFEmpresaEPM.Services.ObjectIntegration
{
    public class Response
    {
        public ResponseCode ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public object ResponseData { get; set; }
    }
}

