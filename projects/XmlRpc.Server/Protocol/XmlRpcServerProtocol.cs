using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XmlRpc.Client.Attributes;
using XmlRpc.Client.DataTypes;
using XmlRpc.Client.Exceptions;
using XmlRpc.Client.Model;
using XmlRpc.Client.Serializer;
using XmlRpc.Client.Serializer.Request;
using XmlRpc.Client.Serializer.Response;

namespace XmlRpc.Server.Protocol
{
    public class XmlRpcServerProtocol : SystemMethodsBase
    {
        public Stream Invoke(Stream requestStream)
        {
            try
            {
                return TryInvoke(requestStream);
            }
            catch (Exception ex)
            {
                return CreateExceptionResponse(ex);
            }
        }

        Stream TryInvoke(Stream requestStream)
        {
            var deserializer = new XmlRpcRequestDeserializer();
            var serviceAttr = (XmlRpcServiceAttribute)Attribute.GetCustomAttribute(GetType(), typeof(XmlRpcServiceAttribute));

            if (serviceAttr != null)
                SetSerializerSettings(serviceAttr, deserializer);

            var xmlRpcReq = deserializer.DeserializeRequest(requestStream, GetType());

            XmlRpcResponse xmlRpcResp = null;
            if (xmlRpcReq.Method.Equals("system.multicall", StringComparison.OrdinalIgnoreCase))
            {
                var resultList = InvokeMulticall(xmlRpcReq);
                xmlRpcResp = new XmlRpcResponse(resultList.Where(x => x != null).ToArray());
            }
            else
            {
                var result = Invoke(xmlRpcReq);
                xmlRpcResp = new XmlRpcResponse(result);
            }

            var responseStream = new MemoryStream();
            var serializer = new XmlRpcResponseSerializer();
            serializer.SerializeResponse(responseStream, xmlRpcResp);
            responseStream.Seek(0, SeekOrigin.Begin);
            return responseStream;
        }

        void SetSerializerSettings(XmlRpcServiceAttribute serviceAttr, XmlRpcSerializer serializer)
        {
            if (serviceAttr.XmlEncoding != null)
                serializer.Configuration.XmlEncoding = Encoding.GetEncoding(serviceAttr.XmlEncoding);

            serializer.Configuration.UseIntTag = serviceAttr.UseIntTag;
            serializer.Configuration.UseStringTag = serviceAttr.UseStringTag;
            serializer.Configuration.UseIndentation = serviceAttr.UseIndentation;
            serializer.Configuration.Indentation = serviceAttr.Indentation;
        }

        Stream CreateExceptionResponse(Exception exception)
        {
            XmlRpcFaultException fex;
            if (exception is XmlRpcException)
                fex = new XmlRpcFaultException(0, ((XmlRpcException)exception).Message);
            else if (exception is XmlRpcFaultException)
                fex = (XmlRpcFaultException)exception;
            else
                fex = new XmlRpcFaultException(0, exception.Message);

            var serializer = new XmlRpcSerializer();
            var responseStream = new MemoryStream();
            serializer.SerializeFaultResponse(responseStream, fex);
            responseStream.Seek(0, SeekOrigin.Begin);
            return responseStream;
        }

        object Invoke(XmlRpcRequest request)
        {
            try
            {
                var methodInfo = request.MethodInfo ?? GetType().GetMethod(request.Method);
                return methodInfo.Invoke(this, request.Arguments);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;

                throw ex;
            }
        }

        List<object> InvokeMulticall(XmlRpcRequest multicallRequest)
        {
            var resultList = new List<object>();
            var arguments = multicallRequest.Arguments.First() as Array;
            var requests = arguments.Cast<XmlRpcStruct>().Select(x => new MulticallFunction { MehtodName = (string)x["methodName"], Params = (object[])x["params"] }).ToArray();

            foreach (var request in requests)
            {
                var singleRequest = new XmlRpcRequest(request.MehtodName, request.Params);
                var serviceInfo = XmlRpcServiceInfo.CreateServiceInfo(GetType());

                var possibleMethods = serviceInfo.GetMethodInfos(request.MehtodName);
                var method = possibleMethods.First(p => p.GetParameters().Length == request.Params.Length);
                singleRequest.MethodInfo = method;

                var result = Invoke(singleRequest);

                resultList.Add(result);
            }

            return resultList;
        }
    }
}
