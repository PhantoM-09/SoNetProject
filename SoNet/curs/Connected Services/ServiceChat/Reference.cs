﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace curs.ServiceChat {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceChat.IServiceChat", CallbackContract=typeof(curs.ServiceChat.IServiceChatCallback))]
    public interface IServiceChat {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceChat/Connect", ReplyAction="http://tempuri.org/IServiceChat/ConnectResponse")]
        void Connect(string idUser, string idFriend);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceChat/Connect", ReplyAction="http://tempuri.org/IServiceChat/ConnectResponse")]
        System.Threading.Tasks.Task ConnectAsync(string idUser, string idFriend);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceChat/Disconnect", ReplyAction="http://tempuri.org/IServiceChat/DisconnectResponse")]
        void Disconnect(string idUser, string idFriend);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceChat/Disconnect", ReplyAction="http://tempuri.org/IServiceChat/DisconnectResponse")]
        System.Threading.Tasks.Task DisconnectAsync(string idUser, string idFriend);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServiceChat/SendMessageToChat")]
        void SendMessageToChat(string message, string idUser, string idFriend);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServiceChat/SendMessageToChat")]
        System.Threading.Tasks.Task SendMessageToChatAsync(string message, string idUser, string idFriend);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChatCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServiceChat/MessageCallBack")]
        void MessageCallBack(string message, string idUser);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChatChannel : curs.ServiceChat.IServiceChat, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceChatClient : System.ServiceModel.DuplexClientBase<curs.ServiceChat.IServiceChat>, curs.ServiceChat.IServiceChat {
        
        public ServiceChatClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public ServiceChatClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public ServiceChatClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceChatClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceChatClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void Connect(string idUser, string idFriend) {
            base.Channel.Connect(idUser, idFriend);
        }
        
        public System.Threading.Tasks.Task ConnectAsync(string idUser, string idFriend) {
            return base.Channel.ConnectAsync(idUser, idFriend);
        }
        
        public void Disconnect(string idUser, string idFriend) {
            base.Channel.Disconnect(idUser, idFriend);
        }
        
        public System.Threading.Tasks.Task DisconnectAsync(string idUser, string idFriend) {
            return base.Channel.DisconnectAsync(idUser, idFriend);
        }
        
        public void SendMessageToChat(string message, string idUser, string idFriend) {
            base.Channel.SendMessageToChat(message, idUser, idFriend);
        }
        
        public System.Threading.Tasks.Task SendMessageToChatAsync(string message, string idUser, string idFriend) {
            return base.Channel.SendMessageToChatAsync(message, idUser, idFriend);
        }
    }
}