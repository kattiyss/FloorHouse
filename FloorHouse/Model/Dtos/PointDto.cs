using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloorHouse.Model.Dtos
{
    public struct PointDto
    {
        public int X, Y;

        public PointDto(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
