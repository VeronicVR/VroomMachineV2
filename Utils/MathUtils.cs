using System;
using System.Collections.Generic;
using System.Text;

namespace VroomMachineV2.Utils
{
    class MathUtils
    {
        public static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
    }
}
