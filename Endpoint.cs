using Microsoft.Rtc.Collaboration;
using System;

namespace BlendedConvDemo
{
    class EndpointObject
    {

        UserEndpoint _endpoint { get; set; }
        Conversation _conversation { get; set; }
        CollaborationPlatform _collabPlatform { get; set; }
        ConversationContextChannel _channel { get; set; }
        string _sipaddress { get; set; }
        InstantMessagingFlow _flow { get; set; }


        private string Username { get; set; }
        private string Password { get; set; }
        private string Domain { get; set; }
        private string Server { get; set; }
        private string DestinationSipAddress { get; set; }
        private bool IncludeContextChannel { get; set; }

        public EndpointObject(string sipaddress, string username, string password, string domain, string servername, string destinationSipAddress, bool includeContextChannel, CollaborationPlatform platform)
        {
            _collabPlatform = platform;
            _sipaddress = sipaddress;
            Username = username;
            Password = password;
            Domain = domain;
            Server = servername;
            DestinationSipAddress = destinationSipAddress;
            IncludeContextChannel = includeContextChannel;
        }


        public void Establish()
        {
            try
            {
                UserEndpointSettings settings;
                if (Server != null)
                    settings = new UserEndpointSettings(_sipaddress, Server);
                else
                    settings = new UserEndpointSettings(_sipaddress);

                settings.Credential = new System.Net.NetworkCredential(Username, Password, Domain);

                _endpoint = new UserEndpoint(_collabPlatform, settings);
                _endpoint.BeginEstablish(UserEndpointEstablishCompleted, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Establish:{0}", ex.Message));
            }
        }

        private void UserEndpointEstablishCompleted(IAsyncResult ar)
        {
            try
            {
                _endpoint.EndEstablish(ar);

                Console.WriteLine("Endpoint Ready");
                ConversationSequence conversations = new ConversationSequence();
                conversations.StartConversationSequence(DestinationSipAddress, IncludeContextChannel, _endpoint);
            }

            catch (Exception ex)
            {
                Console.WriteLine(String.Format("BeginPublishPresence:{0}", ex.Message));                
            }
        }














    }
}
