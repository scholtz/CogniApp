using Google.Apis.Auth.OAuth2;
using Google.Apis.Fitness.v1;
using Google.Apis.Fitness.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emotiv2GoogleFit
{
    class GoogleFit
    {
        private static readonly DateTime unixEpochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly string clientId = System.Configuration.ConfigurationManager.AppSettings["googleClientId"];
        private readonly string clientSecret = System.Configuration.ConfigurationManager.AppSettings["googleClientSecret"];
        private const string userId = "me";
        private readonly string UserName;
        private readonly Struct.Device device;

        private DataSource dataSource;
        private string dataSourceId;
        private FitnessService service = null;
        public GoogleFit(string UserName, Struct.Device device)
        {
            this.UserName = UserName;
            this.device = device;

            Init();
        }
        public void Init()
        {
            try
            {
                string[] scopes = new string[]
                {
                FitnessService.Scope.FitnessBodyWrite,
                FitnessService.Scope.FitnessBodyRead,
                };

                // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync
                (
                    new ClientSecrets
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret
                    },
                    scopes,
                    UserName,
                    CancellationToken.None,
                    new FileDataStore("Google.Fitness.Auth", false)
                ).Result;


                service = new FitnessService(new BaseClientService.Initializer()
                {
                    ApplicationName = Assembly.GetExecutingAssembly().GetName().Name,
                    HttpClientInitializer = credential
                });


                dataSource = new DataSource()
                {
                    Type = "raw",
                    DataStreamName = "GoogleFitEmotivDataSource",
                    Application = new Application()
                    {
                        Name = service.ApplicationName,
                        Version = "1"
                    },
                    DataType = new DataType()
                    {
                        Name = "sk.scholtz.emotive.cognitivestate",
                        Field = new List<DataTypeField>()
                        {
                            new DataTypeField() { Name = "interest", Format = "floatPoint" },
                            new DataTypeField() { Name = "stress", Format = "floatPoint"},
                            new DataTypeField() { Name = "relaxation", Format = "floatPoint"},
                            new DataTypeField() { Name = "excitement", Format = "floatPoint"},
                            new DataTypeField() { Name = "engagement", Format = "floatPoint" },
                            new DataTypeField() { Name = "longtermexcitement", Format = "floatPoint" },
                            new DataTypeField() { Name = "focus", Format = "floatPoint"},
                        }
                    },
                    Device = new Device()
                    {
                        Manufacturer = device.Manufacturer,
                        Model = device.Model,
                        Type = device.Type,
                        Uid = device.Uid,
                        Version = device.Version
                    }
                };
                //dataSourceId = $"{dataSource.Type}:{dataSource.DataType.Name}:{userId}:{dataSource.Device.Manufacturer}:{dataSource.Device.Model}:{dataSource.Device.Uid}:{dataSource.DataStreamName}";
                //raw:sk.scholtz.emotive.cognitivestate:413883787851:Emotiv:RD-906:1000001:GoogleFitEmotivDataSource
                var dataSrcList = service.Users.DataSources.List(userId).ExecuteAsync().Result;
                dataSourceId = dataSrcList.DataSource.Select(s => s.DataStreamId).Where(
                    s =>
                    s != null
                    && s.Contains(dataSource.DataStreamName)
                    && s.Contains(dataSource.Device.Manufacturer)
                    && s.Contains(dataSource.Device.Uid)).FirstOrDefault();

                if (!string.IsNullOrEmpty(dataSourceId))
                {
                    dataSource = service.Users.DataSources.Get(userId, dataSourceId).Execute();
                }
                else
                {
                    dataSource = service.Users.DataSources.Create(dataSource, userId).Execute();
                }

            }
            catch (Exception exc)
            {
                Console.Error.WriteLine(exc.Message);
            }
        }
        public async Task NewDataPoint(Dictionary<string, Dictionary<string, object>> cogniData)
        {
            try
            {
                if (service == null) return;

                var postNanosec = GetUnixEpochNanoSeconds(DateTime.UtcNow);

                var widthDataSource = new Dataset()
                {
                    DataSourceId = dataSourceId,
                    MaxEndTimeNs = postNanosec,
                    MinStartTimeNs = postNanosec,
                    Point = new List<DataPoint>()
                {
                    new DataPoint()
                    {
                        DataTypeName = dataSource.DataType.Name,
                        StartTimeNanos = postNanosec,
                        EndTimeNanos = postNanosec,
                        Value = new List<Value>()
                        {
                            new Value()
                            {
                                FpVal = float.Parse(cogniData["met"]["int"].ToString(), System.Globalization.CultureInfo.InvariantCulture)
                            },
                            new Value()
                            {
                                FpVal = float.Parse(cogniData["met"]["str"].ToString(), System.Globalization.CultureInfo.InvariantCulture)
                            },
                            new Value()
                            {
                                FpVal = float.Parse(cogniData["met"]["rel"].ToString(), System.Globalization.CultureInfo.InvariantCulture)
                            },
                            new Value()
                            {
                                FpVal = float.Parse(cogniData["met"]["exc"].ToString(), System.Globalization.CultureInfo.InvariantCulture)
                            },
                            new Value()
                            {
                                FpVal = float.Parse(cogniData["met"]["eng"].ToString(), System.Globalization.CultureInfo.InvariantCulture)
                            },
                            new Value()
                            {
                                FpVal = float.Parse(cogniData["met"]["lex"].ToString(), System.Globalization.CultureInfo.InvariantCulture)
                            },
                            new Value()
                            {
                                FpVal = float.Parse(cogniData["met"]["foc"].ToString(), System.Globalization.CultureInfo.InvariantCulture)
                            },
                        }
                    }
                }
                };
                var dataSetId = $"{postNanosec}-{postNanosec}";
                await service.Users.DataSources.Datasets.Patch(widthDataSource, userId, dataSourceId, dataSetId).ExecuteAsync();
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine(exc.Message);
            }
        }

        public static long GetUnixEpochNanoSeconds(DateTime dt)
        {
            return (dt.Ticks - unixEpochStart.Ticks) * 100;
        }
    }
}
