using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        public MainViewModel()
        {
            DisplayName = "SmartHome Monitoring v2.0";
        }
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (Commons.MQTT_CLIENT.IsConnected)
            {
                Commons.MQTT_CLIENT.Disconnect();
                Commons.MQTT_CLIENT = null;
            }
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        public void LoadHistoryView()
        {
            ActivateItemAsync(new HistoryViewModel());
        }
        public void LoadDataBaseView()
        {
            ActivateItemAsync(new DataBaseViewModel());
        }
        public void LoadRealTimeView()
        {
            ActivateItemAsync(new RealTimeViewModel());
        }
        public void ExitProgram()
        {
            Environment.Exit(0);  // 프로그램 종료
        }
        public void ExitToolBar()
        {
            Environment.Exit(0);  // 프로그램 종료
        }

        // Start 메뉴, 아이콘 눌렀을때 처리할 이벤트
        public void PopInfoDialog()
        {
            TaskPopUp();
        }
        
        public void StartSubscribe()
        {
            TaskPopUp();
        }
        public void StopSubscribe()
        {
            if(this.ActiveItem is DataBaseViewModel)
            {
                DeactivateItemAsync(this.ActiveItem, true);
            }
        }
        private void TaskPopUp()
        {
            // CustomPopupView
            var winManager = new WindowManager();
            var result = winManager.ShowDialogAsync(new CustomPopupViewModel("New Broker"));

            if (result.Result == true)
            {
                ActivateItemAsync(new DataBaseViewModel());
            }
        }
        public void PopInfoView()
        {
            var winManager = new WindowManager();
            winManager.ShowDialogAsync(new CustomInfoViewModel("About"));
        }
    }
}
