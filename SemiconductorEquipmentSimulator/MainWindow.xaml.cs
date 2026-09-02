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

        public MainWindow()
        {
            InitializeComponent();

            // 타이머 실행 간격을 1초로 설정
            timer.Interval = TimeSpan.FromSeconds(1);

            // 타이머가 1초마다 동작할 때 Timer_Tick 실행
            timer.Tick += Timer_Tick;
        }

        // 장비 시작
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "상태 : 가동 중";

            timer.Start();

            LogList.Items.Add(
                $"{DateTime.Now:HH:mm:ss} - 장비 시작"
            );
        }

        // 1초마다 온도와 압력 변경
        private void Timer_Tick(object? sender, EventArgs e)
        {
            // 온도를 최대 80°C까지 증가
            if (temperature < 80)
            {
                temperature++;

                TemperatureText.Text =
                    $"온도 : {temperature} °C";
            }

            // 압력을 최소 1 Torr까지 감소
            if (pressure > 1)
            {
                pressure *= 0.9;

                // 압력이 1 Torr 아래로 내려가지 않도록 제한
                if (pressure < 1)
                {
                    pressure = 1;
                }

                PressureText.Text =
                    $"압력 : {pressure:F1} Torr";
            }

            // 목표 온도와 압력에 도달하면 장비 안정 상태
            if (temperature >= 80 && pressure <= 1)
            {
                StatusText.Text = "상태 : 안정";

                timer.Stop();

                LogList.Items.Add(
                    $"{DateTime.Now:HH:mm:ss} - 장비 안정 상태 도달"
                );
            }

            // 인터락 조건 검사
            CheckInterlock();
        }

        // 과열 상태 검사 및 장비 자동 정지
        private void CheckInterlock()
        {
            if (temperature >= 90)
            {
                StatusText.Text = "상태 : 경보 - 과열";

                timer.Stop();

                LogList.Items.Add(
                    $"{DateTime.Now:HH:mm:ss} - 과열 경보 발생"
                );
            }
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
        private void ResetButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            timer.Stop();

            temperature = 25;
            pressure = 760;

            TemperatureText.Text =
                $"온도 : {temperature} °C";

            PressureText.Text =
                $"압력 : {pressure:F1} Torr";

            StatusText.Text = "상태 : 정지";

            LogList.Items.Add(
                $"{DateTime.Now:HH:mm:ss} - 장비 초기화"
            );
        }

        // 장비 정지
        private void StopButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            StatusText.Text = "상태 : 정지";

            timer.Stop();

            LogList.Items.Add(
                $"{DateTime.Now:HH:mm:ss} - 장비 정지"
            );
        }
    }
}