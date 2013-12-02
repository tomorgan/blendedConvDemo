using Microsoft.Rtc.Collaboration;
using System;
using System.Linq;


namespace BlendedConvDemo
{
    class Program
    {
        static CollaborationPlatform _collabPlatform { get; set; }
        static EndpointObject endpoint;

        //THINGS YOU SHOULD CHANGE FOR YOUR ENVIRONMENT
        static string LoginSipAddress = "sip:user1@yourdomain.com";
        static string LoginUsername = "user1";
        static string LoginPassword = "pass@word1";
        static string LoginDomain = "yourdomain";
        static string ServerURI = null; //leave this null if using automatic discovery
        static string DestinationSipAddress = "sip:user2@yourdomain.com"; //the sip address to start the 2 conversations with
        static bool IncludeContextChannel = true;  //set to false to see two different conversations blended into the same window.

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            Startup();
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
        }

        private static void Startup()
        {

            string userAgent = "Example Demo";

            var platformSettings = new ClientPlatformSettings(userAgent, Microsoft.Rtc.Signaling.SipTransportType.Tls);

            _collabPlatform = new CollaborationPlatform(platformSettings);
            _collabPlatform.AllowedAuthenticationProtocol = Microsoft.Rtc.Signaling.SipAuthenticationProtocols.Ntlm;
            _collabPlatform.BeginStartup(EndCollabPlatformStartup, null);
            
            //shut down on key press
            Console.ReadKey();
            Console.WriteLine("Shutting Down...");
            _collabPlatform.BeginShutdown(EndShutdown, _collabPlatform);

        }

        private static void EndShutdown(IAsyncResult ar)
        {
            CollaborationPlatform _platform = (CollaborationPlatform)ar.AsyncState;
            _platform.EndShutdown(ar);
            Console.WriteLine("Platform has shut down. Good Bye.");
            Environment.Exit(0);
        }

        private static void EndCollabPlatformStartup(IAsyncResult ar)
        {
            _collabPlatform.EndStartup(ar);
            Console.WriteLine("Collab Platform Started. Press any key to shutdown.");
            endpoint = new EndpointObject(LoginSipAddress, LoginUsername, LoginPassword, LoginDomain, ServerURI, DestinationSipAddress, IncludeContextChannel, _collabPlatform);
            endpoint.Establish();
        }




    }
}
