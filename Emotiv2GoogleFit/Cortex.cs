using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emotiv2GoogleFit
{
    public class Cortex
    {


        [Serializable]
        public class HelloInfo
        {
            public string hello = "world";
        }
        [Serializable]
        public class LoginParams
        {
            public string username = "";
            public string password = "";
            public string client_id = "";
            public string client_secret = "";
        }
        [Serializable]
        public class RequestAccessParams
        {
            public string clientId = "";
            public string clientSecret = "";
        }
        [Serializable]
        public class LogoutParams
        {
            public string username = "";
        }

        [Serializable]
        public class AuthorizeParams
        {
            public string clientId = "";
            public string clientSecret = "";
        }
        [Serializable]
        public class SubscribeParams
        {
            public string cortexToken = "";
            public string session = null;
            public string[] streams = null;
            public bool replay = false;
        }
        [Serializable]
        public class CreateSessionParams
        {
            public string cortexToken = "";
            public string headset = "";
            public string status = "open";
        }
        [Serializable]
        public class UserInfo
        {
            public string currentOSUId = "";
            public string currentOSUsername = "";
            public string loggedInOSUId = "";
            public string loggedInOSUsername = "";
            public string username = "";
        }

        [Serializable]
        public class RequestAccessInfo
        {
            public bool accessGranted = false;
            public string message = "";
        }
        [Serializable]
        public class AuthorizeResultType
        {
            public string cortexToken = "";
            public WarningType warning = null;
        }

        [Serializable]
        public class WarningType
        {
            public int code = 0;
            public string message = "";
            public string licenseUrl = "";
        }
        [Serializable]
        public class CreateSessionResultType
        {
            public string appId = "";
            public HeadsetInfo headset = new HeadsetInfo();
            public string id = "";
            public string license = "";
            public string owner = "";
            public string[] recordIds = new string[0];
            public bool recording = false;
            public DateTimeOffset started = DateTimeOffset.MinValue;
            public string status = "";
            public string stopped = "";
            public string[] streams = new string[0];
        }

        public class Request
        {

            [Serializable]
            public class General
            {
                public string jsonrpc = "2.0";
                public int id;
                public string method;
            }

            [Serializable]
            public class GetCortexInfo : General
            {
                public GetCortexInfo()
                {
                    method = "getCortexInfo";
                }
            }

            [Serializable]
            public class Hello : General
            {
                public Hello()
                {
                    method = "hello";
                }
                public HelloInfo __params__ = new HelloInfo();
            }
            [Serializable]
            public class Login : General
            {
                public Login()
                {
                    method = "login";
                }
                public LoginParams __params__ = new LoginParams();
            }
            [Serializable]
            public class GetUserLogin : General
            {
                public const string Method = "getUserLogin";
                public GetUserLogin()
                {
                    method = Method;
                }
            }
            [Serializable]
            public class RequestAccess : General
            {
                public const string Method = "requestAccess";
                public RequestAccess()
                {
                    method = Method;
                }
                public RequestAccessParams __params__ = new RequestAccessParams();
            }



            [Serializable]
            public class QueryHeadsets : General
            {
                public const string Method = "queryHeadsets";
                public QueryHeadsets()
                {
                    method = Method;
                }
            }
            [Serializable]
            public class Logout : General
            {
                public Logout()
                {
                    method = "logout";
                }
                public LogoutParams __params__ = new LogoutParams();
            }
            [Serializable]
            public class Authorize : General
            {
                public const string Method = "authorize";
                public Authorize()
                {
                    method = Method;
                }
                public AuthorizeParams __params__ = new AuthorizeParams();
            }
            [Serializable]
            public class Subscribe : General
            {
                public const string Method = "subscribe";
                public Subscribe()
                {
                    method = Method;
                }
                public SubscribeParams __params__ = new SubscribeParams();
            }
            [Serializable]
            public class CreateSession : General
            {
                public const string Method = "createSession";
                public CreateSession()
                {
                    method = Method;
                }
                public CreateSessionParams __params__ = new CreateSessionParams();
            }

        }
        public enum ResponseTypeEnum
        {
            Hello,
            Login,
            Logout,
            GetUserLogin,
            Authorize,
            GenerateNewToken,
            AcceptLicense,
            GetLicenseInfo,
            QueryHeadsets,
            ControlBluetoothHeadset,
            SessionWithHeadsetInfo,
            SessionWithoutHeadsetInfo,
            Subscribe,
            Unsubscribe,
            InjectMarker,
            GetDetectionInfo,
            Training,
            GetTrainingTime,
            GetTrainedSignatureActions,
            MentalCommandGetSkillRating,
            MentalCommandActionSensitivity,
            MentalCommandActionLevel,
            MentalCommandActiveAction,
            FacialExpressionSignatureType,
            FacialExpressionThreshold,
            MentalCommandTrainingThreshold,
            MentalCommandBrainMap,
            QueryProfile,
            SetupProfile,
            GetCurrentProfile,
            DecryptData,
            ConfigMapping,
            CreateSession,
            StreamData,
            RequestAccess
        }
        public class Response
        {
            [Serializable]
            public class General
            {
                public string jsonrpc;
                public int id;
                public ErrorCl warning;
                public ErrorCl error;
                public virtual ResponseTypeEnum ResponseType { get; }
            }


            [Serializable]
            public class Hello : General
            {
                public string result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.Hello; } }
            }
            [Serializable]
            public class Login : General
            {
                public string result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.Login; } }
            }
            [Serializable]
            public class Logout : General
            {
                public string result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.Logout; } }
            }
            [Serializable]
            public class GetUserLogin : General
            {
                public UserInfo[] result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.GetUserLogin; } }
            }
            [Serializable]
            public class RequestAccess : General
            {
                public RequestAccessInfo result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.RequestAccess; } }
            }


            [Serializable]
            public class Authorize : General
            {
                public AuthorizeResultType result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.Authorize; } }
            }
            [Serializable]
            public class CreateSession : General
            {
                public CreateSessionResultType result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.CreateSession; } }
            }
            [Serializable]
            public class GenerateNewToken : General
            {
                public Auth result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.GenerateNewToken; } }
            }
            [Serializable]
            public class AcceptLicense : General
            {
                public Auth result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.AcceptLicense; } }
            }
            [Serializable]
            public class GetLicenseInfo : General
            {
                public LicenseInfo result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.GetLicenseInfo; } }
            }

            [Serializable]
            public class QueryHeadsets : General
            {
                public HeadsetInfo[] result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.QueryHeadsets; } }
            }

            [Serializable]
            public class ControlBluetoothHeadset : General
            {
                public string result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.ControlBluetoothHeadset; } }
            }

            [Serializable]
            public class SessionWithHeadsetInfo : General
            {
                public SessionInfoWithHeadset result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.SessionWithHeadsetInfo; } }
            }
            [Serializable]
            public class SessionWithoutHeadsetInfo : General
            {
                public SessionInfoWithoutHeadset result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.SessionWithoutHeadsetInfo; } }
            }


            [Serializable]
            public class Subscribe : General
            {
                public SubscribeInfo result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.Subscribe; } }
            }

            [Serializable]
            public class Unsubscribe : General
            {
                public MessageInfo[] message;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.Unsubscribe; } }
            }
            [Serializable]
            public class InjectMarker : General
            {
                public string appId;
                public HeadsetInfo headset;
                public string licence;
                public LogsInfo[] logs;
                public MarkerInfo[] markers;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.InjectMarker; } }
            }

            [Serializable]
            public class GetDetectionInfo : General
            {
                public DetectionInfo result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.GetDetectionInfo; } }
            }

            [Serializable]
            public class Training : General
            {
                public string result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.Training; } }
            }
            [Serializable]
            public class GetTrainingTime : General
            {
                public int result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.GetTrainingTime; } }
            }

            [Serializable]
            public class GetTrainedSignatureActions : General
            {
                public TrainedActionsInfo result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.GetTrainedSignatureActions; } }
            }
            [Serializable]
            public class MentalCommandGetSkillRating : General
            {
                public int result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.MentalCommandGetSkillRating; } }
            }
            [Serializable]
            public class MentalCommandActionSensitivity : General
            {
                public int[] result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.MentalCommandActionSensitivity; } }
            }
            [Serializable]
            public class MentalCommandActionLevel : General
            {
                public int result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.MentalCommandActionLevel; } }
            }

            [Serializable]
            public class MentalCommandActiveAction : General
            {
                public string[] result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.MentalCommandActiveAction; } }
            }

            [Serializable]
            public class FacialExpressionSignatureType : General
            {
                public FacialExpressionSignatureTypeInfo result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.FacialExpressionSignatureType; } }
            }
            [Serializable]
            public class FacialExpressionThreshold : General
            {
                public int result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.FacialExpressionThreshold; } }
            }
            [Serializable]
            public class MentalCommandTrainingThreshold : General
            {
                public MentalCommandTrainingThresholdInfo result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.MentalCommandTrainingThreshold; } }
            }
            [Serializable]
            public class MentalCommandBrainMap : General
            {
                public BrainMap[] result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.MentalCommandBrainMap; } }
            }

            [Serializable]
            public class QueryProfile : General
            {
                public ProfileInfo[] result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.QueryProfile; } }
            }

            [Serializable]
            public class SetupProfile : General
            {
                public string result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.SetupProfile; } }
            }
            [Serializable]
            public class GetCurrentProfile : General
            {
                public string result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.GetCurrentProfile; } }
            }
            [Serializable]

            public class DecryptData : General
            {
                public string result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.DecryptData; } }
            }
            [Serializable]
            public class ConfigMapping : General
            {
                public string result;
                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.ConfigMapping; } }
            }
            public enum MentalData
            {
                Interest,
                Stress,
                Relaxation,
                Excitement,
                Engagement,
                LongTermExcitement,
                Focus

            }
            [Serializable]
            public class StreamData : General
            {

                public object[] eeg;
                public object[] mot;
                public object[] com;
                public object[] fac;
                public string[] met;
                public object[] dev;
                public object[] pow;
                public object[] sys;

                public string sid;

                public string message;

                public string[] actions;
                public string[] controls;
                public string[] events;

                public override ResponseTypeEnum ResponseType { get { return ResponseTypeEnum.StreamData; } }

                public Dictionary<MentalData, decimal> GetMentalData()
                {
                    var ret = new Dictionary<MentalData, decimal>();
                    if (met != null)
                    {
                        if (met.Length == 7)
                        {
                            ret = new Dictionary<MentalData, decimal>()
                            {
                                [MentalData.Interest] = Decimal.Parse(met[0], CultureInfo.InvariantCulture),
                                [MentalData.Stress] = Decimal.Parse(met[1], CultureInfo.InvariantCulture),
                                [MentalData.Relaxation] = Decimal.Parse(met[2], CultureInfo.InvariantCulture),
                                [MentalData.Excitement] = Decimal.Parse(met[3], CultureInfo.InvariantCulture),
                                [MentalData.Engagement] = Decimal.Parse(met[4], CultureInfo.InvariantCulture),
                                [MentalData.LongTermExcitement] = Decimal.Parse(met[5], CultureInfo.InvariantCulture),
                                [MentalData.Focus] = Decimal.Parse(met[6], CultureInfo.InvariantCulture)
                            };
                        }


                    }
                    return ret;
                }


            }

        }


        [Serializable]
        public class ErrorCl
        {
            public string code;
            public string message;
        }
        [Serializable]
        public class ResultCl
        {
            public decimal[] eeg;
            public decimal[] mot;
            public string[] com;
            public string[] fac;
            public decimal[] met;
            public object[] dev;
            public decimal[] pow;
            public string[] sys;

            public string sid;

            public string message;

            public string[] actions;
            public string[] controls;
            public string[] events;
        }
        [Serializable]
        public class Auth
        {
            public string _auth;
        }
        [Serializable]
        public class LicenseInfo
        {
            public bool? is_commercial;
            public string license_id;
            public string max_record;
            public int? recording_count;
            public string[] scopes;
            public int? seat_count;
            public DateTimeOffset? valid_from;
            public DateTimeOffset? valid_to;
        }
        [Serializable]

        public class DeviceSettings
        {
            public decimal? eegRate;
            public decimal? eegRes;
            public decimal? memsRate;
            public decimal? memsRes;
            public string mode;
        }
        [Serializable]
        public class HeadsetInfo
        {
            public string connectedBy;
            public string dongle;
            public string firmware;
            public string id;
            public string label;
            public string[] sensors;
            public DeviceSettings settings;
            public string status;
        }
        [Serializable]
        public class MarkerInfo
        {
            public int code;
            public string[] enums;
            public string[][] events;
            public string label;
            public string port;

        }

        [Serializable]
        public class SessionInfo
        {
            public string appId;
            public string owner;
            public string profile;
            public string project;
            public string title;
            public bool recording;
            public string client;
            public string id;
            public string license;
            public LogsInfo logs;
            public MarkerInfo[] markers;
            public DateTimeOffset? started;
            public string status;
            public DateTimeOffset? stopped;
            public string[] streams;
            public string subject;
            public string[] tags;
        }
        [Serializable]
        public class SessionInfoWithHeadset : SessionInfo
        {
            public HeadsetInfo headset;
        }
        [Serializable]
        public class SessionInfoWithoutHeadset : SessionInfo
        {
            public string headset;
        }


        [Serializable]
        public class SubscriptionStatusErrorInfo
        {
            public int code = 0;
            public string message = "";
            public string streamName = "";
        }
        [Serializable]
        public class SubscriptionStatusInfo
        {
            public object[] cols = new object[0];
            public string sid = "";
            public string streamName = "";
        }
        [Serializable]
        public class SubscribeInfo
        {
            public SubscriptionStatusErrorInfo[] failure = null;
            public SubscriptionStatusInfo[] success = null;
        }

        [Serializable]
        public class MotInfo : SubscribeInfo
        {
            public string[] cols;
            public string sid;
        }


        [Serializable]
        public class MessageInfo
        {
            public string message;
        }

        [Serializable]
        public class LogsInfo
        {
            public RecordInfo recordInfos;

        }
        [Serializable]
        public class RecordInfo
        {
            public string name;
            public string notes;
            public string recordId;
            public int[] sampleMarkerAFF;
            public int[] sampleMarkerEEG;
            public DateTimeOffset? startMarkerRecording;
            public DateTimeOffset? stopMarkerRecording;
            public string subject;
        }

        [Serializable]
        public class DetectionInfo
        {
            public string[] actions;
            public string[] controls;
            public string[] events;
        }
        [Serializable]
        public class ActionInfo
        {
            public string action;
            public int times;
        }
        [Serializable]
        public class TrainedActionsInfo
        {
            public ActionInfo[] trained_actions;
            public int total_times_training;
        }
        [Serializable]
        public class FacialExpressionSignatureTypeInfo
        {
            public string currentSig;
            public string[] availableSig;
        }
        [Serializable]
        public class MentalCommandTrainingThresholdInfo
        {
            public decimal? currentThreshold;
            public decimal? lastTrainingScore;
        }
        [Serializable]
        public class BrainMap
        {
            public string action;
            public decimal[] coordinates;
        }
        [Serializable]
        public class MetaInfo
        {
            public DateTimeOffset? creation_time;
        }
        [Serializable]
        public class ProfileInfo
        {
            public string name;
            public MetaInfo meta;

        }

    }
}
