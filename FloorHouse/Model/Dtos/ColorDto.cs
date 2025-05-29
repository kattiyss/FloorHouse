namespace FloorHouse.Model.Dtos
{
    public struct ColorDto
    {
        public byte R, G, B, A;

        public ColorDto(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}
