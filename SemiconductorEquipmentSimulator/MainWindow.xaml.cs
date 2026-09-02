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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //온도를 올리기 위한 타이머 객체 생성
        private DispatcherTimer timer = new DispatcherTimer();

        //온도
        private int temperature = 25;

        //압력
        private double pressure = 760;

        public MainWindow()
        {
            InitializeComponent();

            //타이머 실행 간격 1초
            timer.Interval = TimeSpan.FromSeconds(1);

            //Tick => 타이머가 울릴 때 마다 실행할 이벤트
            //+=으로 쓰는 이유는 Tick이라는 이벤트가 이미 존재하고 Timer_Tick이라는 이벤트를 Tick에 추가하는 것이기 때문
            timer.Tick += Timer_Tick;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "STATUS : RUNNING";
            timer.Start();
        }

        //온도가 1씩 오르다가 80까지 올라가면 멈추는 코드
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (temperature < 80)
            {
                temperature++;
                TemperatureText.Text = $"TEMPERATURE : {temperature} °C";
            }

            //압력이 1 이하로 내려가지 않게 하기위함
            if (pressure > 1)
            {
                pressure *= 0.9;

                if (pressure < 1)
                {
                    pressure = 1;
                }

                PressureText.Text = $"PRESSURE : {pressure:F1} Torr";
            }

            //온도가 80이 되고 압력이 1이하면 텍스트 수정하고 timer를 멈춤
            if (temperature >= 80 && pressure <= 1)
            {
                StatusText.Text = "STATUS : STABLE";
                timer.Stop();
            }

            CheckInterlock();
        }

        //온도가 90이상이면 기계 자동 정지
        private void CheckInterlock()
        {
            if (temperature >= 90)
            {
                StatusText.Text = "STATUS : ALARM - OVERHEAT";
                timer.Stop();
            }
        }


        //온도를 95로 설정하고 기계자동정지 호출
        private void OverheatTestButton_Click(object sender, RoutedEventArgs e)
        {
            temperature = 95;
            TemperatureText.Text = $"TEMPERATURE : {temperature} °C";

            CheckInterlock();
        }


        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "STATUS : STOPPED";
            timer.Stop();
        }
    }
}