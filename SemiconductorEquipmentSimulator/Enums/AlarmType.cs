using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiconductorEquipmentSimulator.Enums;

public enum AlarmType
{
    None,

    //과열
    OverTemperature,

    //압력 오류
    PressureError,

    //센서 에러
    SensorError,

    //통신 에러
    CommunicationError
}
