using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WpfSmartHomeMonitoringApp.Helpers;
using WpfSmartHomeMonitoringApp.Models;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    class DataBaseViewModel : Screen
    {
        private string brokerUrl;
        private string topic;
        private string connString;
        private string dbLog;
        private bool isConnected;
        public string BrokerUrl
        {
            get { return brokerUrl; }
            set
            {
                brokerUrl = value;
                NotifyOfPropertyChange(() => BrokerUrl);
            }
        }

        public string Topic
        {
            get { return topic; }
            set
            {
                topic = value;
                NotifyOfPropertyChange(() => Topic);
            }
        }

        public string ConnString
        {
            get { return connString; }
            set
            {
                connString = value;
                NotifyOfPropertyChange(() => ConnString);
            }
        }

        public string DbLog
        {
            get { return dbLog; }
            set
            {
                dbLog = value;
                NotifyOfPropertyChange(() => DbLog);
            }
        }

        public bool IsConnected
        {
            get { return isConnected; }

            set
            {
                isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
            }
        }

        public DataBaseViewModel()
        {
            BrokerUrl = Commons.BROKERHOST = "127.0.0.1";   //MQTT Broker Ip 설정
            Topic = Commons.PUB_TOPIC = "home/device/#";  //Multiple Topic
            // Single Level Wildcard +
            // Multi Level Wildcard #
            ConnString = Commons.CONNSTRING = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";

            if (Commons.IS_CONNECT)
            {
                IsConnected = true;
                ConnectDb();
            }
        }

        /// <summary>
        /// DB연결 + MQTT Broker 접속
        /// </summary>
        public void ConnectDb()
        {
            if (IsConnected)
            {
                Commons.MQTT_CLIENT = new MqttClient(BrokerUrl);

                try
                {
                    if (Commons.MQTT_CLIENT.IsConnected != true)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Connect("MONITOR");
                        Commons.MQTT_CLIENT.Subscribe(new string[] { Commons.PUB_TOPIC },
                            new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                        UpdateText(">>> MQTT Broker Connected");
                        IsConnected = Commons.IS_CONNECT = true;
                    }
                }
                catch (Exception ex)
                {
                    // Pass;
                }
            }
            else    // 접속 끄기
            {
                try
                {
                    if (Commons.MQTT_CLIENT.IsConnected)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived -= MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Disconnect();
                        UpdateText(">>> MQTT Broker Disconnected...");
                        IsConnected = Commons.IS_CONNECT = false;
                    }
                }
                catch (Exception ex)
                {

                    // Pass
                }
            }

        }

        private void UpdateText(string message)
        {
            DbLog += $"{message}\n";
        }

        /// <summary>
        /// Subscribe한 메시지 처리해주는 이벤트핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MQTT_CLIENT_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            UpdateText(message);
            SetDataBase(message, e.Topic);
        }

        private void SetDataBase(string message, string topic)
        {
            var currDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
            var model = new SmartHomeModel();   // DB 모델 사용

            Debug.WriteLine(currDatas);
            model.DevId = currDatas["DevId"];
            model.CurrTime = DateTime.Parse(currDatas["CurrTime"]);
            model.Temp = double.Parse(currDatas["Temp"]);
            model.Humid = double.Parse(currDatas["Humid"]);

            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();
                // Verbatim string c#
                string strInQuery = @"INSERT INTO TblSmartHome
                                                (DevId,
                                                 CurrTime,
                                                 Temp,
                                                 Humid)
                                            VALUES
                                                (@DevId,
                                                 @CurrTime,
                                                 @Temp,
                                                 @Humid)";
                try
                {
                    SqlCommand cmd = new SqlCommand(strInQuery, conn);
                    SqlParameter parmDevId = new SqlParameter("@DevId", model.DevId);
                    SqlParameter parmCurrTime = new SqlParameter("@CurrTime", model.CurrTime);
                    SqlParameter parmTemp = new SqlParameter("@Temp", model.Temp);
                    SqlParameter parmHumid = new SqlParameter("@Humid", model.Humid);
                    cmd.Parameters.Add(parmDevId);
                    cmd.Parameters.Add(parmCurrTime);
                    cmd.Parameters.Add(parmTemp);
                    cmd.Parameters.Add(parmHumid);

                    if (cmd.ExecuteNonQuery() == 1)
                        UpdateText(">>> DB Inserted."); // 저장성공
                    else
                        UpdateText(">>> DB Failed!!!!!");   // 저장실패

                }
                catch (Exception ex)
                {
                    UpdateText($">>> DB ERROR! {ex.Message}"); // 예외
                }
            }
        }
    }
}