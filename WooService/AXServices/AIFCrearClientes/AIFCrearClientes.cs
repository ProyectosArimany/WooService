﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WooService.AXServices.AIFCrearClientes
{
    using System.Runtime.Serialization;


    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CallContext", Namespace = "http://schemas.microsoft.com/dynamics/2010/01/datacontracts")]
    public partial class CallContext : object
    {

        private string CompanyField;

        private string LanguageField;

        private string LogonAsUserField;

        private string MessageIdField;

        private string PartitionKeyField;

        private System.Collections.Generic.Dictionary<string, string> PropertyBagField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Company
        {
            get
            {
                return this.CompanyField;
            }
            set
            {
                this.CompanyField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Language
        {
            get
            {
                return this.LanguageField;
            }
            set
            {
                this.LanguageField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LogonAsUser
        {
            get
            {
                return this.LogonAsUserField;
            }
            set
            {
                this.LogonAsUserField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MessageId
        {
            get
            {
                return this.MessageIdField;
            }
            set
            {
                this.MessageIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PartitionKey
        {
            get
            {
                return this.PartitionKeyField;
            }
            set
            {
                this.PartitionKeyField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<string, string> PropertyBag
        {
            get
            {
                return this.PropertyBagField;
            }
            set
            {
                this.PropertyBagField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "AifFault", Namespace = "http://schemas.microsoft.com/dynamics/2008/01/documents/Fault")]
    public partial class AifFault : object
    {

        private string CustomDetailXmlField;

        private WooService.AXServices.AIFCrearClientes.FaultMessageList[] FaultMessageListArrayField;

        private WooService.AXServices.AIFCrearClientes.InfologMessage[] InfologMessageListField;

        private string StackTraceField;

        private int XppExceptionTypeField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CustomDetailXml
        {
            get
            {
                return this.CustomDetailXmlField;
            }
            set
            {
                this.CustomDetailXmlField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public WooService.AXServices.AIFCrearClientes.FaultMessageList[] FaultMessageListArray
        {
            get
            {
                return this.FaultMessageListArrayField;
            }
            set
            {
                this.FaultMessageListArrayField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public WooService.AXServices.AIFCrearClientes.InfologMessage[] InfologMessageList
        {
            get
            {
                return this.InfologMessageListField;
            }
            set
            {
                this.InfologMessageListField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StackTrace
        {
            get
            {
                return this.StackTraceField;
            }
            set
            {
                this.StackTraceField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int XppExceptionType
        {
            get
            {
                return this.XppExceptionTypeField;
            }
            set
            {
                this.XppExceptionTypeField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "FaultMessageList", Namespace = "http://schemas.microsoft.com/dynamics/2008/01/documents/Fault")]
    public partial class FaultMessageList : object
    {

        private string DocumentField;

        private string DocumentOperationField;

        private WooService.AXServices.AIFCrearClientes.FaultMessage[] FaultMessageArrayField;

        private string FieldField;

        private string ServiceField;

        private string ServiceOperationField;

        private string ServiceOperationParameterField;

        private string XPathField;

        private string XmlLineField;

        private string XmlPositionField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Document
        {
            get
            {
                return this.DocumentField;
            }
            set
            {
                this.DocumentField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DocumentOperation
        {
            get
            {
                return this.DocumentOperationField;
            }
            set
            {
                this.DocumentOperationField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public WooService.AXServices.AIFCrearClientes.FaultMessage[] FaultMessageArray
        {
            get
            {
                return this.FaultMessageArrayField;
            }
            set
            {
                this.FaultMessageArrayField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Field
        {
            get
            {
                return this.FieldField;
            }
            set
            {
                this.FieldField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Service
        {
            get
            {
                return this.ServiceField;
            }
            set
            {
                this.ServiceField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ServiceOperation
        {
            get
            {
                return this.ServiceOperationField;
            }
            set
            {
                this.ServiceOperationField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ServiceOperationParameter
        {
            get
            {
                return this.ServiceOperationParameterField;
            }
            set
            {
                this.ServiceOperationParameterField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string XPath
        {
            get
            {
                return this.XPathField;
            }
            set
            {
                this.XPathField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string XmlLine
        {
            get
            {
                return this.XmlLineField;
            }
            set
            {
                this.XmlLineField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string XmlPosition
        {
            get
            {
                return this.XmlPositionField;
            }
            set
            {
                this.XmlPositionField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "InfologMessage", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Dynamics.AX.Framework.Services")]
    public partial class InfologMessage : object
    {

        private WooService.AXServices.AIFCrearClientes.InfologMessageType InfologMessageTypeField;

        private string MessageField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public WooService.AXServices.AIFCrearClientes.InfologMessageType InfologMessageType
        {
            get
            {
                return this.InfologMessageTypeField;
            }
            set
            {
                this.InfologMessageTypeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                this.MessageField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "FaultMessage", Namespace = "http://schemas.microsoft.com/dynamics/2008/01/documents/Fault")]
    public partial class FaultMessage : object
    {

        private string CodeField;

        private string MessageField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Code
        {
            get
            {
                return this.CodeField;
            }
            set
            {
                this.CodeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                this.MessageField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "InfologMessageType", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Dynamics.AX.Framework.Services")]
    public enum InfologMessageType : int
    {

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Info = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Warning = 1,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Error = 2,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://tempuri.org", ConfigurationName = "WooService.AXServices.AIFCrearClientes.triAIFCrearClienteService")]
    public interface triAIFCrearClienteService
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/triAIFCrearClienteService/AIFCrearContacto", ReplyAction = "http://tempuri.org/triAIFCrearClienteService/AIFCrearContactoResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(WooService.AXServices.AIFCrearClientes.AifFault), Action = "http://tempuri.org/triAIFCrearClienteService/AIFCrearContactoAifFaultFault", Name = "AifFault", Namespace = "http://schemas.microsoft.com/dynamics/2008/01/documents/Fault")]
        System.Threading.Tasks.Task<WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearContactoResponse> AIFCrearContactoAsync(WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearContactoRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/triAIFCrearClienteService/AIFCrearDireccion", ReplyAction = "http://tempuri.org/triAIFCrearClienteService/AIFCrearDireccionResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(WooService.AXServices.AIFCrearClientes.AifFault), Action = "http://tempuri.org/triAIFCrearClienteService/AIFCrearDireccionAifFaultFault", Name = "AifFault", Namespace = "http://schemas.microsoft.com/dynamics/2008/01/documents/Fault")]
        System.Threading.Tasks.Task<WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearDireccionResponse> AIFCrearDireccionAsync(WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearDireccionRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/triAIFCrearClienteService/AIFActualizarCoordenasCliente", ReplyAction = "http://tempuri.org/triAIFCrearClienteService/AIFActualizarCoordenasClienteRespons" +
            "e")]
        [System.ServiceModel.FaultContractAttribute(typeof(WooService.AXServices.AIFCrearClientes.AifFault), Action = "http://tempuri.org/triAIFCrearClienteService/AIFActualizarCoordenasClienteAifFaul" +
            "tFault", Name = "AifFault", Namespace = "http://schemas.microsoft.com/dynamics/2008/01/documents/Fault")]
        System.Threading.Tasks.Task<WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFActualizarCoordenasClienteResponse> AIFActualizarCoordenasClienteAsync(WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFActualizarCoordenasClienteRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/triAIFCrearClienteService/AIFCrearCliente", ReplyAction = "http://tempuri.org/triAIFCrearClienteService/AIFCrearClienteResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(WooService.AXServices.AIFCrearClientes.AifFault), Action = "http://tempuri.org/triAIFCrearClienteService/AIFCrearClienteAifFaultFault", Name = "AifFault", Namespace = "http://schemas.microsoft.com/dynamics/2008/01/documents/Fault")]
        System.Threading.Tasks.Task<WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearClienteResponse> AIFCrearClienteAsync(WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearClienteRequest request);
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "triAIFCrearClienteServiceAIFCrearContactoRequest", WrapperNamespace = "http://tempuri.org", IsWrapped = true)]
    public partial class triAIFCrearClienteServiceAIFCrearContactoRequest
    {

        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://schemas.microsoft.com/dynamics/2010/01/datacontracts")]
        public WooService.AXServices.AIFCrearClientes.CallContext CallContext;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://tempuri.org", Order = 0)]
        public string _JSONContacto;

        public triAIFCrearClienteServiceAIFCrearContactoRequest()
        {
        }

        public triAIFCrearClienteServiceAIFCrearContactoRequest(WooService.AXServices.AIFCrearClientes.CallContext CallContext, string _JSONContacto)
        {
            this.CallContext = CallContext;
            this._JSONContacto = _JSONContacto;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "triAIFCrearClienteServiceAIFCrearContactoResponse", WrapperNamespace = "http://tempuri.org", IsWrapped = true)]
    public partial class triAIFCrearClienteServiceAIFCrearContactoResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://tempuri.org", Order = 0)]
        public string response;

        public triAIFCrearClienteServiceAIFCrearContactoResponse()
        {
        }

        public triAIFCrearClienteServiceAIFCrearContactoResponse(string response)
        {
            this.response = response;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "triAIFCrearClienteServiceAIFCrearDireccionRequest", WrapperNamespace = "http://tempuri.org", IsWrapped = true)]
    public partial class triAIFCrearClienteServiceAIFCrearDireccionRequest
    {

        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://schemas.microsoft.com/dynamics/2010/01/datacontracts")]
        public WooService.AXServices.AIFCrearClientes.CallContext CallContext;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://tempuri.org", Order = 0)]
        public string _JSONDireccion;

        public triAIFCrearClienteServiceAIFCrearDireccionRequest()
        {
        }

        public triAIFCrearClienteServiceAIFCrearDireccionRequest(WooService.AXServices.AIFCrearClientes.CallContext CallContext, string _JSONDireccion)
        {
            this.CallContext = CallContext;
            this._JSONDireccion = _JSONDireccion;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "triAIFCrearClienteServiceAIFCrearDireccionResponse", WrapperNamespace = "http://tempuri.org", IsWrapped = true)]
    public partial class triAIFCrearClienteServiceAIFCrearDireccionResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://tempuri.org", Order = 0)]
        public string response;

        public triAIFCrearClienteServiceAIFCrearDireccionResponse()
        {
        }

        public triAIFCrearClienteServiceAIFCrearDireccionResponse(string response)
        {
            this.response = response;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "triAIFCrearClienteServiceAIFActualizarCoordenasClienteRequest", WrapperNamespace = "http://tempuri.org", IsWrapped = true)]
    public partial class triAIFCrearClienteServiceAIFActualizarCoordenasClienteRequest
    {

        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://schemas.microsoft.com/dynamics/2010/01/datacontracts")]
        public WooService.AXServices.AIFCrearClientes.CallContext CallContext;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://tempuri.org", Order = 0)]
        public string _cliente;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://tempuri.org", Order = 1)]
        public long _postalAddressRecid;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://tempuri.org", Order = 2)]
        public decimal _latitude;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://tempuri.org", Order = 3)]
        public decimal _longitude;

        public triAIFCrearClienteServiceAIFActualizarCoordenasClienteRequest()
        {
        }

        public triAIFCrearClienteServiceAIFActualizarCoordenasClienteRequest(WooService.AXServices.AIFCrearClientes.CallContext CallContext, string _cliente, long _postalAddressRecid, decimal _latitude, decimal _longitude)
        {
            this.CallContext = CallContext;
            this._cliente = _cliente;
            this._postalAddressRecid = _postalAddressRecid;
            this._latitude = _latitude;
            this._longitude = _longitude;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "triAIFCrearClienteServiceAIFActualizarCoordenasClienteResponse", WrapperNamespace = "http://tempuri.org", IsWrapped = true)]
    public partial class triAIFCrearClienteServiceAIFActualizarCoordenasClienteResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://tempuri.org", Order = 0)]
        public string response;

        public triAIFCrearClienteServiceAIFActualizarCoordenasClienteResponse()
        {
        }

        public triAIFCrearClienteServiceAIFActualizarCoordenasClienteResponse(string response)
        {
            this.response = response;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "triAIFCrearClienteServiceAIFCrearClienteRequest", WrapperNamespace = "http://tempuri.org", IsWrapped = true)]
    public partial class triAIFCrearClienteServiceAIFCrearClienteRequest
    {

        [System.ServiceModel.MessageHeaderAttribute(Namespace = "http://schemas.microsoft.com/dynamics/2010/01/datacontracts")]
        public WooService.AXServices.AIFCrearClientes.CallContext CallContext;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://tempuri.org", Order = 0)]
        public string _JSONFichaCliente;

        public triAIFCrearClienteServiceAIFCrearClienteRequest()
        {
        }

        public triAIFCrearClienteServiceAIFCrearClienteRequest(WooService.AXServices.AIFCrearClientes.CallContext CallContext, string _JSONFichaCliente)
        {
            this.CallContext = CallContext;
            this._JSONFichaCliente = _JSONFichaCliente;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "triAIFCrearClienteServiceAIFCrearClienteResponse", WrapperNamespace = "http://tempuri.org", IsWrapped = true)]
    public partial class triAIFCrearClienteServiceAIFCrearClienteResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://tempuri.org", Order = 0)]
        public string response;

        public triAIFCrearClienteServiceAIFCrearClienteResponse()
        {
        }

        public triAIFCrearClienteServiceAIFCrearClienteResponse(string response)
        {
            this.response = response;
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    public interface triAIFCrearClienteServiceChannel : WooService.AXServices.AIFCrearClientes.triAIFCrearClienteService, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    public partial class triAIFCrearClienteServiceClient : System.ServiceModel.ClientBase<WooService.AXServices.AIFCrearClientes.triAIFCrearClienteService>, WooService.AXServices.AIFCrearClientes.triAIFCrearClienteService
    {

        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);

        public triAIFCrearClienteServiceClient() :
                base(triAIFCrearClienteServiceClient.GetDefaultBinding(), triAIFCrearClienteServiceClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.NetTcpBinding_triAIFCrearClienteService.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public triAIFCrearClienteServiceClient(EndpointConfiguration endpointConfiguration) :
                base(triAIFCrearClienteServiceClient.GetBindingForEndpoint(endpointConfiguration), triAIFCrearClienteServiceClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public triAIFCrearClienteServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) :
                base(triAIFCrearClienteServiceClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public triAIFCrearClienteServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) :
                base(triAIFCrearClienteServiceClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public triAIFCrearClienteServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearContactoResponse> WooService.AXServices.AIFCrearClientes.triAIFCrearClienteService.AIFCrearContactoAsync(WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearContactoRequest request)
        {
            return base.Channel.AIFCrearContactoAsync(request);
        }

        public System.Threading.Tasks.Task<WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearContactoResponse> AIFCrearContactoAsync(WooService.AXServices.AIFCrearClientes.CallContext CallContext, string _JSONContacto)
        {
            WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearContactoRequest inValue = new WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearContactoRequest();
            inValue.CallContext = CallContext;
            inValue._JSONContacto = _JSONContacto;
            return ((WooService.AXServices.AIFCrearClientes.triAIFCrearClienteService)(this)).AIFCrearContactoAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearDireccionResponse> WooService.AXServices.AIFCrearClientes.triAIFCrearClienteService.AIFCrearDireccionAsync(WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearDireccionRequest request)
        {
            return base.Channel.AIFCrearDireccionAsync(request);
        }

        public System.Threading.Tasks.Task<WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearDireccionResponse> AIFCrearDireccionAsync(WooService.AXServices.AIFCrearClientes.CallContext CallContext, string _JSONDireccion)
        {
            WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearDireccionRequest inValue = new WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearDireccionRequest();
            inValue.CallContext = CallContext;
            inValue._JSONDireccion = _JSONDireccion;
            return ((WooService.AXServices.AIFCrearClientes.triAIFCrearClienteService)(this)).AIFCrearDireccionAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFActualizarCoordenasClienteResponse> WooService.AXServices.AIFCrearClientes.triAIFCrearClienteService.AIFActualizarCoordenasClienteAsync(WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFActualizarCoordenasClienteRequest request)
        {
            return base.Channel.AIFActualizarCoordenasClienteAsync(request);
        }

        public System.Threading.Tasks.Task<WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFActualizarCoordenasClienteResponse> AIFActualizarCoordenasClienteAsync(WooService.AXServices.AIFCrearClientes.CallContext CallContext, string _cliente, long _postalAddressRecid, decimal _latitude, decimal _longitude)
        {
            WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFActualizarCoordenasClienteRequest inValue = new WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFActualizarCoordenasClienteRequest();
            inValue.CallContext = CallContext;
            inValue._cliente = _cliente;
            inValue._postalAddressRecid = _postalAddressRecid;
            inValue._latitude = _latitude;
            inValue._longitude = _longitude;
            return ((WooService.AXServices.AIFCrearClientes.triAIFCrearClienteService)(this)).AIFActualizarCoordenasClienteAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearClienteResponse> WooService.AXServices.AIFCrearClientes.triAIFCrearClienteService.AIFCrearClienteAsync(WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearClienteRequest request)
        {
            return base.Channel.AIFCrearClienteAsync(request);
        }

        public System.Threading.Tasks.Task<WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearClienteResponse> AIFCrearClienteAsync(WooService.AXServices.AIFCrearClientes.CallContext CallContext, string _JSONFichaCliente)
        {
            WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearClienteRequest inValue = new WooService.AXServices.AIFCrearClientes.triAIFCrearClienteServiceAIFCrearClienteRequest();
            inValue.CallContext = CallContext;
            inValue._JSONFichaCliente = _JSONFichaCliente;
            return ((WooService.AXServices.AIFCrearClientes.triAIFCrearClienteService)(this)).AIFCrearClienteAsync(inValue);
        }

        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }

        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.NetTcpBinding_triAIFCrearClienteService))
            {
                System.ServiceModel.NetTcpBinding result = new System.ServiceModel.NetTcpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }

        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.NetTcpBinding_triAIFCrearClienteService))
            {
                return new System.ServiceModel.EndpointAddress("net.tcp://srvaxaos:8201/DynamicsAx/Services/triAIFCreaClienteServiceGroup");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }

        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return triAIFCrearClienteServiceClient.GetBindingForEndpoint(EndpointConfiguration.NetTcpBinding_triAIFCrearClienteService);
        }

        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return triAIFCrearClienteServiceClient.GetEndpointAddress(EndpointConfiguration.NetTcpBinding_triAIFCrearClienteService);
        }

        public enum EndpointConfiguration
        {

            NetTcpBinding_triAIFCrearClienteService,
        }
    }
}