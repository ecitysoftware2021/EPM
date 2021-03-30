using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEmpresaEPM.Classes
{
    public enum EResponseRedenam
    {
        Aprobada = 0,
        Declinada = 1,
        PinIncorrecto = 2,
        ClaveSupervisorErrada = 3,
        EntidadNoResponse = 4,
        ErrorGeneral = 99
    }

    public enum ELogType
    {
        General = 0,
        Error = 1,
        Device = 2
    }

    public enum ETypeTransaction
    {
        PagoFactura = 29,
        FacturaPrepago = 27,
        PagoMedida = 28
    }

    public enum ETypeSearch
    {
        ReferenteDePago = 1,
        NumeroDeContrato = 0
    }

    public enum EBackground
    {
        Identificate = 0,
        Autenticate = 1,
        Productos = 2,
        Paga = 3,
        Generico = 4
    }

    public enum EModalType
    {
        Cancell = 0,
        NotExistAccount = 1,
        Error = 2,
        MaxAmount = 3,
        Information = 4,
        Preload = 5,
        NoPaper = 6
    }

    public enum EError
    {
        Printer = 1,
        Nopapper = 2,
        Device = 3,
        Aplication = 5,
        Api = 6,
        Customer = 7,
        Internet = 8
    }

    public enum ELevelError
    {
        Mild = 3,
        Medium = 2,
        Strong = 1,
    }

    public enum UserControlView
    {
        Login,
        Config,
        Admin,
        Main,
        Menu,
        ConsultPagoFactura,
        ConsultPagoMedida,
        PaySuccess,
        Pay,
        ReturnMony,
        DetailsPagoFactura,
        DetailsPagoMedida,
        ConsultPagoPrepago,
        DetailsPagoPrepago,
        InvoiceListPagoFactura,
    }

    public enum ETransactionState
    {
        Initial = 1,
        Success = 2,
        CancelError = 6,
        Cancel = 3,
        Error = 5,
        ErrorService = 4
    }

    public enum ETransactionType
    {
        Withdrawal = 15,
        Payment = 3,
        ConsultName = 1,
        ConsultTransact = 2,
    }

    public enum ETypeAdministrator
    {
        Balancing = 1,
        Upload = 2,
        Finished = 3,
        ReUploat = 4
    }

    public enum ETypeOption
    {
        Compra = 1,
        Venta = 2,
    }

    public enum ResponseCode
    {
        Error = 300,
        NotFound = 404,
        OK = 200
    }
}
