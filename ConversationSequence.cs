using Microsoft.Rtc.Collaboration;
using System;


namespace BlendedConvDemo
{
    public class ConversationSequence
    {
        private LocalEndpoint EndPoint { get; set; }
        private string DestinationSIP { get; set; }
        private bool IncludeContext { get; set; }
        private InstantMessagingCall CallOne { get; set; }
        private InstantMessagingCall CallTwo { get; set; }


        public void StartConversationSequence(string destinationSIP, bool includeContextChannel, LocalEndpoint endpoint)
        {
            EndPoint = endpoint;
            DestinationSIP = destinationSIP;
            IncludeContext = includeContextChannel;
            try
            {
                //Start 1st Conversation
                Console.Write("Establishing 1st Conversation...");
                Conversation firstConversation = new Conversation(EndPoint, new ConversationSettings() { Subject = "First Conversation" });
                firstConversation.StateChanged += ConversationStateChanged;
                CallOne = new InstantMessagingCall(firstConversation);
                CallOne.BeginEstablish(DestinationSIP, new CallEstablishOptions(), EndEstablishFirstCall, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("StartConversationSequence:{0}", ex.Message));
            }

        }

      private void EndEstablishFirstCall(IAsyncResult ar)
        {
            try
            {
                CallOne.EndEstablish(ar);
                Console.WriteLine(string.Format("Established. Conversation ID = {0}", CallOne.Conversation.Id));
                if (IncludeContext)
                {
                    //if sending context, do this first, then send IM later
                    Console.Write("Sending context to 1st conversation...");
                    var _contextChannel = new ConversationContextChannel(CallOne.Conversation, CallOne.RemoteEndpoint);
                    _contextChannel.BeginEstablish(Guid.Empty, new ConversationContextChannelEstablishOptions(), EndFirstContextChannelEstablish, _contextChannel);
                }
                else
                {
                    //not sending context, just send IM
                    CallOne.Flow.BeginSendInstantMessage(string.Format("{0}: Sending message to 1st conversation", DateTime.Now.ToLongTimeString()), EndSendFirstMessage, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("EndEstablishFirstCall:{0}", ex.Message));
            }

        }

        private void EndFirstContextChannelEstablish(IAsyncResult ar)
        {
            try
            {
                ConversationContextChannel contextChannel = (ConversationContextChannel)ar.AsyncState;
                contextChannel.EndEstablish(ar);
                Console.WriteLine("done.");
                //now send the IM
                CallOne.Flow.BeginSendInstantMessage(string.Format("{0}: Sending message to 1st conversation", DateTime.Now.ToLongTimeString()), EndSendFirstMessage, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("EndFirstContextChannelEstablish:{0}", ex.Message));
            }
        }

        private void EndSendFirstMessage(IAsyncResult ar)
        {
            try
            {
                CallOne.Flow.EndSendInstantMessage(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("EndSendFirstMessage:{0}", ex.Message));
            }


            //brief wait to simulate a more realistic sitation...
            Console.Write("(waiting for 5 seconds..");
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine("done.)");

            //Now start 2nd Conversation
            try
            {
                Console.Write("Establishing 2nd Conversation...");
                Conversation secondConversation = new Conversation(EndPoint, new ConversationSettings() { Subject = "Second Conversation" });
                secondConversation.StateChanged += ConversationStateChanged;
                CallTwo = new InstantMessagingCall(secondConversation);
                CallTwo.BeginEstablish(DestinationSIP, new CallEstablishOptions(), EndEstablishSecondCall, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Establishing second Call:{0}", ex.Message));
            }
        }

        private void EndEstablishSecondCall(IAsyncResult ar)
        {
            try
            {
                CallTwo.EndEstablish(ar);
                Console.WriteLine(string.Format("Established. Conversation ID = {0}",CallTwo.Conversation.Id));
                if (IncludeContext)
                {
                    //if sending context, do this first, then send IM later
                    Console.Write("Sending context to 2nd conversation...");
                    var _contextChannel = new ConversationContextChannel(CallTwo.Conversation, CallTwo.RemoteEndpoint);
                    _contextChannel.BeginEstablish(Guid.Empty, new ConversationContextChannelEstablishOptions(), EndSecondContextChannelEstablish, _contextChannel);
                }
                else
                {
                    //not sending context, just send IM
                    CallTwo.Flow.BeginSendInstantMessage(string.Format("{0}: Sending message to 2nd conversation", DateTime.Now.ToLongTimeString()), EndSendSecondMessage, null);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("EndEstablishSecondCall:{0}", ex.Message));
            }
        }

        private void EndSecondContextChannelEstablish(IAsyncResult ar)
        {
            try
            {
                ConversationContextChannel contextChannel = (ConversationContextChannel)ar.AsyncState;
                contextChannel.EndEstablish(ar);
                Console.WriteLine("done.");
                //now send the IM
                CallTwo.Flow.BeginSendInstantMessage(string.Format("{0}: Sending message to 2nd conversation", DateTime.Now.ToLongTimeString()), EndSendSecondMessage, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("EndFirstContextChannelEstablish:{0}", ex.Message));
            }
        }

        private void EndSendSecondMessage(IAsyncResult ar)
        {
            try
            {
                CallTwo.Flow.EndSendInstantMessage(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("EndSendSecondMessage:{0}", ex.Message));
            }
        }


        private void ConversationStateChanged(object sender, Microsoft.Rtc.Signaling.StateChangedEventArgs<ConversationState> e)
        {
            if (e.State == ConversationState.Terminated)
            {
                Conversation conv = (Conversation)sender;
                Console.WriteLine("Conversation ID {0} has been terminated", conv.Id);
            }
        }
        


    }
}
