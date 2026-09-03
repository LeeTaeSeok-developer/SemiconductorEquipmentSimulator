using System.IO;
using System.Text;

namespace SemiconductorEquipmentSimulator.Services;

public sealed class EquipmentLogger
{
    // 로그 파일이 저장될 전체 경로
    private readonly string logFilePath;

    public EquipmentLogger()
    {
        // Windows의 사용자별 로컬 데이터 폴더에 로그 폴더 생성
        string logDirectory = Path.Combine( //여기서 경로를 이어붙임 (가져온 경로 \\ SemiconductorEquipmentSimulator \\ Logs)
            Environment.GetFolderPath(  //()안에서 지정한 경로를 문자열로 변환
                Environment.SpecialFolder.LocalApplicationData ), //윈도우 안의 특정 사용자 파일 위치
            "SemiconductorEquipmentSimulator",
            "Logs"
        );

        // 로그 폴더가 없으면 새로 생성
        Directory.CreateDirectory(logDirectory);

        // 실제 로그 파일 경로 설정
        logFilePath = Path.Combine(
            logDirectory,
            "장비로그.txt"
        );
    }

    /// <summary>
    /// 전달받은 한국어 메시지를 로그 파일에 저장하고 저장되는 로그를 반환
    /// </summary>
    /// <param name="message">저장할 로그 내용</param>
    /// <returns>날짜와 시간이 포함된 로그 한 줄</returns>
    public string Write(string message)
    {
        // 로그가 발생한 날짜와 시간을 메시지 앞에 추가
        string logLine =
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

        // 기존 로그를 지우지 않고 파일 마지막에 추가
        File.AppendAllText(
            logFilePath,    //추가할 경로
            logLine + Environment.NewLine,  //추가할 내용
            Encoding.UTF8   //인코딩
        );

        // 화면에도 같은 내용을 표시할 수 있도록 반환
        return logLine;
    }

    /// <summary>
    /// 이전에 저장된 장비 로그를 모두 불러온다.
    /// </summary>
    /// <returns>저장된 로그 목록</returns>
    public IReadOnlyList<string> ReadAll()
    {
        // 로그 파일이 아직 없으면 빈 목록 반환
        if (!File.Exists(logFilePath))
        {
            return Array.Empty<string>();
        }

        // UTF-8 형식으로 한국어 로그를 읽어서 반환
        return File.ReadAllLines(   //경로안의 내용을 한줄씩 읽어서 String 리스트로 반환
            logFilePath,
            Encoding.UTF8
        );
    }
}