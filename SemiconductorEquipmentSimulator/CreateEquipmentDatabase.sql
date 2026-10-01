--아래 쿼리를 실행할 데이터베이스 지정
USE EquipmentSimulatorDB;

--묶는 명령어
GO

CREATE TABLE EquipmentLog
(
    --id
    Id INT IDENTITY(1,1) PRIMARY KEY,
    --log가 만들어진 시간
    LogTime DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    -- 상태
    EquipmentState NVARCHAR(20) NOT NULL,
    -- 메시지
    Message NVARCHAR(200) NOT NULL
);
