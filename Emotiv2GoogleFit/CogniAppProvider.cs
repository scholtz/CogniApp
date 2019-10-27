
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;

namespace Emotiv2GoogleFit
{

    public class CogniAppProvider
    {

        public event EventHandler<string> OnMessage;
        public event EventHandler<string> OnError;
        string webSocketUri;
        private WebSocket webSocket;
        public readonly int number;
        public CogniAppProvider(string webSocketUri)
        {
            this.webSocketUri = webSocketUri ?? "ws://scholtz.sk:5000";
            number = (new Random()).Next(10000, 99999);
        }
        public async Task NewDataPoint(Dictionary<string, Dictionary<string, object>> cogniData)
        {
            try
            {
                if (webSocket == null) return;

                var request = new CogniApp.Request.MentalStateData()
                {
                    engagement = decimal.Parse(cogniData["met"]["eng"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                    excitement = decimal.Parse(cogniData["met"]["exc"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                    focus = decimal.Parse(cogniData["met"]["foc"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                    interest = decimal.Parse(cogniData["met"]["int"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                    longtermexcitement = decimal.Parse(cogniData["met"]["lex"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                    relaxation = decimal.Parse(cogniData["met"]["rel"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                    stress = decimal.Parse(cogniData["met"]["str"].ToString(), System.Globalization.CultureInfo.InvariantCulture)
                };
                Send(request);
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine(exc.Message);
            }
        }
        public async Task start()
        {
            try
            {
                OnMessage?.Invoke(this, "Starting connection to CogniApp");

                webSocket = new WebSocket(webSocketUri);
                webSocket.Opened += delegate (object sender, EventArgs e)
                {
                    OnMessage?.Invoke(this, "Websocket is opened...");
                };
                webSocket.Closed += delegate (object sender, EventArgs e)
                {
                    OnMessage?.Invoke(this, "Websocket is closed..");
                };
                webSocket.Error += delegate (object sender, ErrorEventArgs e)
                {
                    OnError?.Invoke(this, "Websocket is closed..");
                };

                webSocket.MessageReceived += WebSocket_MessageReceived;
                OnMessage?.Invoke(this, "Openning session to Emotiv headset");
                webSocket.Open();

                while (webSocket.State == WebSocketState.Connecting) { };
                if (webSocket.State != WebSocketState.Open)
                {

                    throw new Exception("Connection is not opened.");
                }


                OnMessage?.Invoke(this, "We are connected.. Sending cognitive info to the game");


                Send(new CogniApp.Request.Hello() { message = "Hi"});
            }
            catch (Exception exc)
            {
                OnError?.Invoke(this, exc.Message);
            }
        }
        private int requestIter = 1;
        public void Send(CogniApp.Request.BaseRequest msg)
        {
            msg.requestId = requestIter++;
            msg.session = number;
            var send = JsonConvert.SerializeObject(msg);
            webSocket.Send(send);

        }
        private void WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            OnMessage?.Invoke(this, "New message from CogniApp:" + e.Message);
        }
    }
}
