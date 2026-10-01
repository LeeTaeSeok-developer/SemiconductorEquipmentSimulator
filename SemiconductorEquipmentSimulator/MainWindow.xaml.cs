using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SemiconductorEquipmentSimulator.Enums;
using SemiconductorEquipmentSimulator.Services;

namespace SemiconductorEquipmentSimulator
{

    /// <summary>
    /// 메인 장비 제어 화면
    /// </summary>
    public partial class MainWindow : Window
    {
        // 온도와 압력을 변화시키기 위한 타이머
        private DispatcherTimer timer = new DispatcherTimer();

        // 현재 온도
        private int temperature = 25;

        // 현재 압력
        private double pressure = 760;

        //현재 장비 상태
        private EquipmentState currentState = EquipmentState.Stopped;

        // 현재 공정 단계
        private ProcessStep currentProcessStep = ProcessStep.Idle;

        // 현재 발생한 경보
        private AlarmType currentAlarm = AlarmType.None;

        // 장비 로그 저장 및 불러오기 담당
        private readonly EquipmentLogger equipmentLogger =
            new EquipmentLogger();


        // 생산 완료 횟수
        private int cycleCount = 0;


        // 공정 진행 시간
        private int processingSeconds = 0;

        public MainWindow()
        {
            InitializeComponent();

            //로그 호출
            LoadLogs();

            // 타이머 실행 간격
            timer.Interval = TimeSpan.FromSeconds(1);

            // 타이머가 1초마다 동작할 때 Timer_Tick 실행
            timer.Tick += Timer_Tick;
        }

        // 저장된 로그를 화면에 표시
        private void LoadLogs()
        {
            foreach (string logLine in equipmentLogger.ReadAll())   //저장된 로그를 하나씩 꺼냄
            {
                LogList.Items.Add(logLine);
            }
        }

        // 로그를 파일과 화면에 동시에 추가
        private void AddLog(string message)
        {
            WriteLog("EQUIPMENT", message);
        }


        // 알람 전용 로그
        private void AddAlarmLog(string message)
        {
            WriteLog("ALARM", message);
        }

        // 생산 전용 로그
        private void AddProductionLog(string message)
        {
            WriteLog("PRODUCTION", message);
        }

        private void WriteLog(string category, string message)
        {
            string logLine = equipmentLogger.Write(
                $"[{category}] {message}"
            );

            LogList.Items.Add(logLine);
            LogList.ScrollIntoView(logLine);    //자동 스크롤
        }

        // 장비 시작
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // 정지 상태가 아니면 시작할 수 없음
            if (currentState != EquipmentState.Stopped)
            {
                AddLog(
                    $"시작 불가 : 현재 상태 {currentState}"
                );

                return;
            }

            currentState = EquipmentState.Running;
            currentProcessStep = ProcessStep.Heating;
            StatusText.Text = "상태 : 가동 중";
            ProcessStepText.Text = "공정 단계 : Heating";

            timer.Start();

            AddLog("장비 시작");
            AddLog("Heating 시작");
        }

        // 공정 단계에 따라 장비 상태 처리
        private void Timer_Tick(object? sender, EventArgs e)
        {
            // 1단계 : Heating
            if (currentProcessStep == ProcessStep.Heating)
            {
                if (temperature < 80)
                {
                    temperature++;

                    TemperatureText.Text =
                        $"온도 : {temperature} °C";
                }

                if (temperature >= 80)
                {
                    currentProcessStep = ProcessStep.PressureControl;

                    ProcessStepText.Text = "공정 단계 : Pressure Control";

                    AddLog("목표 온도 도달");
                    AddLog("Pressure Control 시작");
                }
            }

            // 2단계 : Pressure Control
            else if (currentProcessStep == ProcessStep.PressureControl)
            {
                if (pressure > 1)
                {
                    pressure *= 0.9;

                    if (pressure < 1)
                    {
                        pressure = 1;
                    }

                    PressureText.Text =
                        $"압력 : {pressure:F1} Torr";
                }

                if (pressure <= 1)
                {
                    currentProcessStep = ProcessStep.Stable;
                    currentState = EquipmentState.Stable;

                    StatusText.Text = "상태 : 안정";
                    ProcessStepText.Text = "공정 단계 : Stable";

                    timer.Stop();

                    AddLog("목표 압력 도달");
                    AddLog("장비 안정 상태 도달");
                }
            }

            else if (currentProcessStep == ProcessStep.Processing)
            {
                processingSeconds++;

                AddProductionLog($"공정 진행 : {processingSeconds}초");

                if (processingSeconds >= 5)
                {
                    currentProcessStep = ProcessStep.Complete;
                    cycleCount++;

                    ProcessStepText.Text = "공정 단계 : Complete";

                    timer.Stop();

                    AddProductionLog("공정 완료");
                    AddProductionLog($"생산 횟수 : {cycleCount}");
                    CycleCountText.Text = $"생산 횟수 : {cycleCount}";
                }
            }

            CheckInterlock();
        }

        // 과열 상태 검사 및 장비 자동 정지
        private void CheckInterlock()
        {
            if (temperature >= 90)
            {
                RaiseAlarm(
                    AlarmType.OverTemperature,
                    "과열"
                );
            }
        }

        private void PressureErrorTestButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            RaiseAlarm(
                AlarmType.PressureError,
                "압력 이상"
            );
        }


        private void SensorErrorTestButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            RaiseAlarm(
                AlarmType.SensorError,
                "센서 이상"
            );
        }


        private void RaiseAlarm(
            AlarmType alarmType,
            string alarmMessage)
        {
            // 이미 알람 상태라면 중복 발생 방지
            if (currentState == EquipmentState.Alarm)
            {
                return;
            }

            currentAlarm = alarmType;
            currentState = EquipmentState.Alarm;

            timer.Stop();

            StatusText.Text = $"상태 : 경보 - {alarmMessage}";

            AddAlarmLog(
                $"경보 발생 : {currentAlarm}"
            );
        }

        // 과열 상황 테스트
        private void OverheatTestButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            temperature = 95;

            TemperatureText.Text =
                $"온도 : {temperature} °C";

            CheckInterlock();
        }

        // 장비 초기화
        // 경보 상태 초기화
        private void ResetButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            // 경보 상태에서만 초기화 가능
            if (currentState != EquipmentState.Alarm)
            {
                AddLog("초기화 불가 : 경보 상태가 아님");
                return;
            }

            timer.Stop();

            temperature = 25;
            pressure = 760;

            currentAlarm = AlarmType.None;
            currentState = EquipmentState.Stopped;
            currentProcessStep = ProcessStep.Idle;

            TemperatureText.Text =
                $"온도 : {temperature} °C";

            PressureText.Text =
                $"압력 : {pressure:F1} Torr";

            StatusText.Text = "상태 : 정지";
            ProcessStepText.Text = "공정 단계 : Idle";

            AddLog("장비 초기화");
        }

        // 장비 정지
        // 장비 정지
        private void StopButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            // 이미 정지 상태라면 다시 정지하지 않음
            if (currentState == EquipmentState.Stopped)
            {
                AddLog(
                    $"정지 불가 : 이미 정지 상태"
                );

                return;
            }

            // 경보 상태에서는 초기화 버튼을 사용해야 함
            if (currentState == EquipmentState.Alarm)
            {
                AddLog(
                    $"정지 불가 : 먼저 초기화 필요"
                );

                return;
            }

            timer.Stop();

            currentState = EquipmentState.Stopped;
            currentProcessStep = ProcessStep.Idle;
            StatusText.Text = "상태 : 정지";
            ProcessStepText.Text = "공정 단계 : Idle";


            AddLog(
                $"장비 정지"
            );
        }


        // 실제 공정 시작
        private void ProcessStartButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            // Stable 상태가 아니면 공정 시작 불가
            if (currentState != EquipmentState.Stable)
            {
                AddLog("공정 시작 불가 : 장비가 안정 상태가 아님");
                return;
            }


            //현재 상태 = 웨이퍼 공정 시행중
            currentProcessStep = ProcessStep.Processing;
            processingSeconds = 0;

            ProcessStepText.Text = "공정 단계 : Processing";

            AddProductionLog("공정 시작");

            timer.Start();
        }
    }
}