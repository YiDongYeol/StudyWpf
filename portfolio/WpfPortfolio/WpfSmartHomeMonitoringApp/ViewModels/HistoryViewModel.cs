﻿using Caliburn.Micro;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfSmartHomeMonitoringApp.Helpers;
using WpfSmartHomeMonitoringApp.Models;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    public class HistoryViewModel : Screen
    {
        private BindableCollection<DivisionModel> divisions;
        private DivisionModel selectedDivision;
        private string startDate;
        private string initStartDate;
        private string endDate;
        private string initEndDate;
        private int totalCount;
        private PlotModel smartHomeModel;   //OxyPlot
        /*
             * Division
             * DivisionVal
             * SelectedDivision
             * StartDate
             * InitStartDate
             * EndDate
             * InitEndDate
             * TotalCount
             * SearchIotData()
             * SmartHomeModel
        */
        public BindableCollection<DivisionModel> Divisions
        {
            get => divisions; set
            {
                divisions = value;
                NotifyOfPropertyChange(() => Divisions);
            }
        }
        public DivisionModel SelectedDivision
        {
            get => selectedDivision; set
            {
                selectedDivision = value;
                NotifyOfPropertyChange(() => SelectedDivision);
            }
        }
        public string StartDate
        {
            get => startDate; set
            {
                startDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }
        public string InitStartDate
        {
            get => initStartDate; set
            {
                initStartDate = value;
                NotifyOfPropertyChange(() => InitStartDate);
            }
        }
        public string EndDate
        {
            get => endDate; set
            {
                endDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }
        public string InitEndDate
        {
            get => initEndDate; set
            {
                initEndDate = value;
                NotifyOfPropertyChange(() => InitEndDate);
            }
        }
        public int TotalCount
        {
            get => totalCount; set
            {
                totalCount = value;
                NotifyOfPropertyChange(() => TotalCount);
            }
        }
        public PlotModel SmartHomeModel
        {
            get => smartHomeModel; set
            {
                smartHomeModel = value;
                NotifyOfPropertyChange(() => SmartHomeModel);
            }
        }


        public HistoryViewModel()
        {
            Commons.CONNSTRING = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";
            InitControl();
        }

        private void InitControl()
        {
            Divisions = new BindableCollection<DivisionModel>   // 콤보박스용 데이터 생성
            {
                new DivisionModel { KeyVal = 0, DivisionVal = "-- Select --"},
                new DivisionModel { KeyVal = 1, DivisionVal = "Dining Room"},
                new DivisionModel { KeyVal = 2, DivisionVal = "Living Room"},
                new DivisionModel { KeyVal = 3, DivisionVal = "BathRoom"},
                new DivisionModel { KeyVal = 4, DivisionVal = "BedRoom"}
            };
            // Select를 선택해서 초기화
            SelectedDivision = Divisions.Where(v => v.DivisionVal.Contains("Select")).FirstOrDefault();

            InitStartDate = DateTime.Now.ToShortDateString();
            InitEndDate = DateTime.Now.AddDays(1).ToShortDateString();
        }

        // 검색 메서드
        public void SearchIoTData()
        {
            if (SelectedDivision.KeyVal == 0) // Select
            {
                MessageBox.Show("검색할 방을 선택하세요.");
                return;
            }

            if (DateTime.Parse(StartDate) > DateTime.Parse(EndDate))
            {
                MessageBox.Show("시작일이 종료일보다 최신일 수 없습니다.");
                return;
            }
            using(SqlConnection conn = new SqlConnection(Commons.CONNSTRING))
            {
                string strQuery = @"SELECT Id, CurrTime, Temp, Humid 
                                         FROM TblSmartHome
                                        WHERE DevId = @DevId
                                          AND CurrTime BETWEEN @StartDate AND @EndDate
                                        ORDER BY Id ASC";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(strQuery, conn);
                    string DivisionStr;
                        switch (SelectedDivision.DivisionVal)
                        {
                            case "Dining Room":
                                DivisionStr = "DNR";
                                break;
                            case "Living Room":
                                DivisionStr = "LVR";
                                break;
                            case "BathRoom":
                                DivisionStr = "BTR";
                                break;
                            case "BedRoom":
                                DivisionStr = "BDR";
                                break;
                            default:
                                DivisionStr = string.Empty;
                                break;
                        }
                    SqlParameter parmDevId = new SqlParameter("@DevId", DivisionStr);
                    SqlParameter parmStartDate = new SqlParameter("@StartDate", StartDate);
                    SqlParameter parmEndDate = new SqlParameter("@EndDate", EndDate);
                    cmd.Parameters.Add(parmDevId);
                    cmd.Parameters.Add(parmStartDate);
                    cmd.Parameters.Add(parmEndDate);
                    SqlDataReader reader = cmd.ExecuteReader();

                    var i = 0;
                    while (reader.Read())
                    {
                        var temp = reader["Temp"];

                        i++;
                    }

                    TotalCount = i;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error {ex.Message}");
                    return;
                }
            }
        }
    }
}
