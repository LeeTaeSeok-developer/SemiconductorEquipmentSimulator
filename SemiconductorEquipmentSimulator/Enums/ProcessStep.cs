using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiconductorEquipmentSimulator.Enums;

//공정 단계
public enum ProcessStep
{
    //대기
    Idle,

    //온도 안정
    Heating,

    //압력 안정
    PressureControl,

    //안정
    Stable,

    //웨이퍼 공정 수행중
    Processing,

    //완료
    Complete
}