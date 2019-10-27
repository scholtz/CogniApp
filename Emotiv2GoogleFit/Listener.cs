using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;

namespace Emotiv2GoogleFit
{
    public class Listener
    {
        private readonly string webSocketUri;
        private WebSocket webSocket;
        private AutoResetEvent messageReceiveEvent = new AutoResetEvent(false);
        private string lastMessageReceived;
        private readonly string login;
        private readonly string password;
        private int _nextRequestId; // Unique id for each request
        private readonly string clientId;
        private readonly string clientSecret;
        public ConcurrentDictionary<int, string> Request2Method = new ConcurrentDictionary<int, string>();
        public ConcurrentDictionary<string, Cortex.SubscriptionStatusInfo> SubsriptionDetails = new ConcurrentDictionary<string, Cortex.SubscriptionStatusInfo>();

        public event EventHandler<Dictionary<string, Dictionary<string, object>>> OnStreamDataReceived;
        public event EventHandler<Struct.Device> OnDeviceChange;
        public event EventHandler<string> OnMessage;
        public event EventHandler<string> OnError;

        public Listener(string login, string password, string webSocketUri = "wss://localhost:6868")
        {
            this.login = login;
            this.password = password;
            this.webSocketUri = webSocketUri;
            this.clientId = System.Configuration.ConfigurationManager.AppSettings["emotivClientId"];
            this.clientSecret = System.Configuration.ConfigurationManager.AppSettings["emotivClientSecret"];

        }
        public void Start()
        {
            webSocket = new WebSocket(webSocketUri);
            webSocket.Opened += new EventHandler(websocket_Opened);
            webSocket.Closed += new EventHandler(websocket_Closed);
            webSocket.Error += new EventHandler<ErrorEventArgs>(websocket_Error);
            webSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
            OnMessage?.Invoke(this, "Openning session to Emotiv headset");
            webSocket.Open();
            _nextRequestId = 1;
            while (webSocket.State == WebSocketState.Connecting) { };
            if (webSocket.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not opened.");
            }
            OnMessage?.Invoke(this, "We are connected.. Sending requests: GetCortexInfo, GetUserLogin, RequestAccess");


            SendReqest(new Cortex.Request.GetCortexInfo());
            SendReqest(new Cortex.Request.GetUserLogin());
            var request = new Cortex.Request.RequestAccess();
            request.__params__.clientId = clientId;
            request.__params__.clientSecret = clientSecret;
            SendReqest(request);

            //var msgIsLoggedIn = new Cortex.Request.GetUserLogin();
            //SendReqest(msgIsLoggedIn);

            /*
            var msgAuthorizeLoggedIn = new Cortex.Request.Authorize();
            msgAuthorizeLoggedIn.__params__.license = "";
            msgAuthorizeLoggedIn.__params__.client_id = login;
            msgAuthorizeLoggedIn.__params__.client_secret = password;
            
            SendReqest(msgAuthorizeLoggedIn);
            /**/
        }

        public int GenerateRequestedID(int streamType, int requestType)
        {
            return _nextRequestId++;
        }

        internal void SendReqest(Cortex.Request.General msg, int streamType = 0, int requestType = 0)
        {
            msg.id = GenerateRequestedID(streamType, requestType);
            Request2Method[msg.id] = msg.method;
            var text = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
            text = text.Replace("__params__", "params");
            text = text.Replace("\"session\":\"\",", "");
            webSocket.Send(text);
        }


        public void Close()
        {
            OnMessage?.Invoke(this, "Closing websocket...");
            webSocket.Close();
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            OnMessage?.Invoke(this, "Websocket is opened...");
        }
        private void websocket_Error(object sender, ErrorEventArgs e)
        {
            OnError?.Invoke(this, e.Exception.Message);
        }
        private void websocket_Closed(object sender, EventArgs e)
        {
            OnMessage?.Invoke(this, "Websocket is closed.");
        }

        public Cortex.Response.GetUserLogin user = null;
        public Cortex.Response.RequestAccess access = null;
        public Cortex.Response.Authorize auth = null;
        public Cortex.Response.CreateSession session = null;
        public Cortex.Response.QueryHeadsets devices = null;
        public Cortex.Response.Subscribe subscription = null;
        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {

                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(e.Message, typeof(Cortex.Response.General)) as Cortex.Response.General;
                if (obj.error != null)
                {
                    string add = "";
                    if (Request2Method.ContainsKey(obj.id)){
                        add = Request2Method[obj.id]+": ";
                    }

                    OnError?.Invoke(this, add+obj.error.message);
                    return;
                }
                if (Request2Method.ContainsKey(obj.id))
                {
                    switch (Request2Method[obj.id])
                    {
                        case Cortex.Request.Subscribe.Method:
                            subscription = JsonConvert.DeserializeObject(e.Message, typeof(Cortex.Response.Subscribe)) as Cortex.Response.Subscribe;
                            OnMessage?.Invoke(this, $"Subscription processed.. receiving data for {string.Join(",", subscription.result?.success?.Select(s => s.streamName))}");
                            foreach (var item in subscription.result?.success)
                            {
                                SubsriptionDetails[item.streamName] = item;
                            }

                            break;
                        case Cortex.Request.CreateSession.Method:
                            session = JsonConvert.DeserializeObject(e.Message, typeof(Cortex.Response.CreateSession)) as Cortex.Response.CreateSession;

                            var subscribeRequest = new Cortex.Request.Subscribe();
                            subscribeRequest.__params__.cortexToken = auth.result.cortexToken;
                            subscribeRequest.__params__.session = session.result.id;
                            subscribeRequest.__params__.streams = new string[] { "eeg", "mot", "dev", "pow", "met", "com", "fac", "sys" };

                            SendReqest(subscribeRequest);

                            break;
                        case Cortex.Request.QueryHeadsets.Method:

                            devices = JsonConvert.DeserializeObject(e.Message, typeof(Cortex.Response.QueryHeadsets)) as Cortex.Response.QueryHeadsets;


                            if (devices.result.Length > 0)
                            {
                                OnDeviceChange?.Invoke(this, new Struct.Device()
                                {
                                    Manufacturer = "Emotiv",
                                    Model = devices.result[0].id + "-" + devices.result[0].firmware,
                                    Uid = devices.result[0].id,
                                });
                            }

                            OnMessage?.Invoke(this, "Hadsets information received");

                            if (!string.IsNullOrEmpty(auth.result?.cortexToken))
                            {
                                OnMessage?.Invoke(this, "Creating session...");
                                var request = new Cortex.Request.CreateSession();
                                request.__params__.cortexToken = auth.result.cortexToken;
                                request.__params__.headset = devices.result.FirstOrDefault()?.id;

                                SendReqest(request);
                            }
                            break;
                        case Cortex.Request.Authorize.Method:

                            auth = JsonConvert.DeserializeObject(e.Message, typeof(Cortex.Response.Authorize)) as Cortex.Response.Authorize;
                            OnMessage?.Invoke(this, "Authorized");
                            if (!string.IsNullOrEmpty(auth.result?.cortexToken))
                            {
                                OnMessage?.Invoke(this, "Getting headset information...");
                                SendReqest(new Cortex.Request.QueryHeadsets());
                            }

                            break;
                        case Cortex.Request.GetUserLogin.Method:
                            OnMessage?.Invoke(this, "Received user information");
                            user = JsonConvert.DeserializeObject(e.Message, typeof(Cortex.Response.GetUserLogin)) as Cortex.Response.GetUserLogin;
                            break;
                        case Cortex.Request.RequestAccess.Method:
                            access = JsonConvert.DeserializeObject(e.Message, typeof(Cortex.Response.RequestAccess)) as Cortex.Response.RequestAccess;

                            OnMessage?.Invoke(this, "Request Access Message received");
                            if (access.result?.accessGranted == true)
                            {
                                OnMessage?.Invoke(this, "Access granted. Going to authorize...");
                                var request = new Cortex.Request.Authorize();
                                request.__params__.clientId = clientId;
                                request.__params__.clientSecret = clientSecret;
                                SendReqest(request);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(access.result.message))
                                {
                                    OnError?.Invoke(this, "Access not granted: " + access.result.message);
                                }
                                else
                                {
                                    OnError?.Invoke(this, "Access not granted. No access data received");
                                }
                            }
                            break;
                    }
                }
                else
                {
                    // received the subscription data
                    var data = JsonConvert.DeserializeObject(e.Message, typeof(Cortex.Response.StreamData)) as Cortex.Response.StreamData;
                    try
                    {
                        Dictionary<string, Dictionary<string, object>> ret = ProcessToStringValue(data);
                        OnStreamDataReceived?.Invoke(null, ret);

                    }
                    catch (Exception exc)
                    {
                        OnError?.Invoke(this, exc.Message);
                    }
                }

                //var response = JsonUtility.FromJson<Cortex.Response.General>(reply);

                lastMessageReceived = e.Message;
                messageReceiveEvent.Set();
            }
            catch (Exception exc)
            {
                OnError?.Invoke(this, exc.Message);
            }
        }
        private Dictionary<string, Dictionary<string, object>> ProcessToStringValue(Cortex.Response.StreamData data)
        {
            var ret = new Dictionary<string, Dictionary<string, object>>();
            var col = "com";
            if (data.com != null && SubsriptionDetails.ContainsKey(col))
            {
                ret[col] = new Dictionary<string, object>();
                if (SubsriptionDetails[col].cols.Length == data.com.Length)
                {
                    for (int i = 0; i < SubsriptionDetails[col].cols.Length; i++)
                    {
                        ret[col][SubsriptionDetails[col].cols[i].ToString()] = data.com[i];
                    }
                }
            }
            col = "actions";
            if (data.actions != null && SubsriptionDetails.ContainsKey(col))
            {
                ret[col] = new Dictionary<string, object>();
                if (SubsriptionDetails[col].cols.Length == data.actions.Length)
                {
                    for (int i = 0; i < SubsriptionDetails[col].cols.Length; i++)
                    {
                        ret[col][SubsriptionDetails[col].cols[i].ToString()] = data.actions[i];
                    }
                }
            }
            col = "controls";
            if (data.controls != null && SubsriptionDetails.ContainsKey(col))
            {
                ret[col] = new Dictionary<string, object>();
                if (SubsriptionDetails[col].cols.Length == data.controls.Length)
                {
                    for (int i = 0; i < SubsriptionDetails[col].cols.Length; i++)
                    {
                        ret[col][SubsriptionDetails[col].cols[i].ToString()] = data.controls[i];
                    }
                }
            }
            col = "dev";
            if (data.dev != null && SubsriptionDetails.ContainsKey(col))
            {
                ret[col] = new Dictionary<string, object>();
                if (SubsriptionDetails[col].cols.Length == data.dev.Length)
                {
                    for (int i = 0; i < SubsriptionDetails[col].cols.Length; i++)
                    {
                        ret[col][SubsriptionDetails[col].cols[i].ToString()] = data.dev[i];
                    }
                }
            }
            col = "eeg";
            if (data.eeg != null && SubsriptionDetails.ContainsKey(col))
            {
                ret[col] = new Dictionary<string, object>();
                if (SubsriptionDetails[col].cols.Length == data.eeg.Length)
                {
                    for (int i = 0; i < SubsriptionDetails[col].cols.Length; i++)
                    {
                        ret[col][SubsriptionDetails[col].cols[i].ToString()] = data.eeg[i];
                    }
                }
            }

            col = "events";
            if (data.events != null && SubsriptionDetails.ContainsKey(col))
            {
                ret[col] = new Dictionary<string, object>();
                if (SubsriptionDetails[col].cols.Length == data.events.Length)
                {
                    for (int i = 0; i < SubsriptionDetails[col].cols.Length; i++)
                    {
                        ret[col][SubsriptionDetails[col].cols[i].ToString()] = data.events[i];
                    }
                }
            }

            col = "fac";
            if (data.fac != null && SubsriptionDetails.ContainsKey(col))
            {
                ret[col] = new Dictionary<string, object>();
                if (SubsriptionDetails[col].cols.Length == data.fac.Length)
                {
                    for (int i = 0; i < SubsriptionDetails[col].cols.Length; i++)
                    {
                        ret[col][SubsriptionDetails[col].cols[i].ToString()] = data.fac[i];
                    }
                }
            }
            col = "met";
            if (data.met != null && SubsriptionDetails.ContainsKey(col))
            {
                ret[col] = new Dictionary<string, object>();
                if (SubsriptionDetails[col].cols.Length == data.met.Length)
                {
                    for (int i = 0; i < SubsriptionDetails[col].cols.Length; i++)
                    {
                        ret[col][SubsriptionDetails[col].cols[i].ToString()] = data.met[i];
                    }
                }
            }
            col = "mot";
            if (data.mot != null && SubsriptionDetails.ContainsKey(col))
            {
                ret[col] = new Dictionary<string, object>();
                if (SubsriptionDetails[col].cols.Length == data.mot.Length)
                {
                    for (int i = 0; i < SubsriptionDetails[col].cols.Length; i++)
                    {
                        ret[col][SubsriptionDetails[col].cols[i].ToString()] = data.mot[i];
                    }
                }
            }
            col = "pow";
            if (data.pow != null && SubsriptionDetails.ContainsKey(col))
            {
                ret[col] = new Dictionary<string, object>();
                if (SubsriptionDetails[col].cols.Length == data.pow.Length)
                {
                    for (int i = 0; i < SubsriptionDetails[col].cols.Length; i++)
                    {
                        ret[col][SubsriptionDetails[col].cols[i].ToString()] = data.pow[i];
                    }
                }
            }
            col = "sid";
            if (data.sid != null && SubsriptionDetails.ContainsKey(col))
            {
                ret[col] = new Dictionary<string, object>();
                if (SubsriptionDetails[col].cols.Length == data.sid.Length)
                {
                    for (int i = 0; i < SubsriptionDetails[col].cols.Length; i++)
                    {
                        ret[col][SubsriptionDetails[col].cols[i].ToString()] = data.sid[i];
                    }
                }
            }
            col = "sys";
            if (data.sys != null && SubsriptionDetails.ContainsKey(col))
            {
                ret[col] = new Dictionary<string, object>();
                if (SubsriptionDetails[col].cols.Length == data.sys.Length)
                {
                    for (int i = 0; i < SubsriptionDetails[col].cols.Length; i++)
                    {
                        ret[col][SubsriptionDetails[col].cols[i].ToString()] = data.sys[i];
                    }
                }
            }
            return ret;
        }
    }
}
